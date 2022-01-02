using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;

using Livescore.Application.Common.Errors;
using Livescore.Application.Common.Results;
using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.FileUpload {
    internal class FileReceiver : IFileReceiver {
        private readonly MultipartRequestHelper _multipartRequestHelper;
        private readonly ImageFileValidator _imageFileValidator;
        private readonly VideoFileValidator _videoFileValidator;
        private readonly string _path;

        public FileReceiver(
            IConfiguration configuration,
            MultipartRequestHelper multipartRequestHelper,
            ImageFileValidator imageFileValidator,
            VideoFileValidator videoFileValidator
        ) {
            _multipartRequestHelper = multipartRequestHelper;
            _imageFileValidator = imageFileValidator;
            _videoFileValidator = videoFileValidator;

            _path = configuration["UserFiles:Path"];
        }

        public Task<Either<HandleError, FormCollection>> ReceiveImageAndFormValues(
            HttpRequest request, int maxSize, string filePrefix, string fileName = null
        ) {
            return _receiveFileAndFormValues(_imageFileValidator, request, maxSize, filePrefix, fileName);
        }

        public Task<Either<HandleError, FormCollection>> ReceiveVideoAndFormValues(
            HttpRequest request, int maxSize, string filePrefix, string fileName = null
        ) {
            return _receiveFileAndFormValues(_videoFileValidator, request, maxSize, filePrefix, fileName);
        }

        private async Task<Either<HandleError, FormCollection>> _receiveFileAndFormValues(
            IFileValidator validator, HttpRequest request, int maxSize, string filePrefix, string fileName
        ) {
            if (!_multipartRequestHelper.IsMultipartContentType(request.ContentType)) {
                return new ValidationError("multipart/form-data request expected");
            }

            string filePath = null;
            var formAccumulator = new KeyValueAccumulator();

            string boundary;
            try {
                boundary = _multipartRequestHelper.GetBoundary(
                    MediaTypeHeaderValue.Parse(request.ContentType),
                    FormOptions.DefaultMultipartBoundaryLengthLimit
                );
            } catch (InvalidDataException e) {
                return new ValidationError(e.Message);
            }

            var reader = new MultipartReader(boundary, request.Body);

            var section = await reader.ReadNextSectionAsync();
            while (section != null) {
                bool hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(
                    section.ContentDisposition, out var contentDisposition
                );

                if (hasContentDispositionHeader) {
                    if (_multipartRequestHelper.HasFileContentDisposition(contentDisposition)) {
                        (bool valid, string ext, byte[] header) =
                            await validator.ValidateFileExtensionAndSignature(section, contentDisposition);

                        if (!valid) {
                            return new ValidationError("Invalid file format");
                        }

                        fileName = (fileName ?? Path.GetRandomFileName()) + ext;
                        var dir = $"{_path}/{filePrefix}";
                        Directory.CreateDirectory(dir);
                        filePath = $"{dir}/{fileName}";

                        var maxSizeExceeded = false;
                        try {
                            var fileDisposed = false;
                            var fs = new FileStream(filePath, FileMode.CreateNew);
                            int bufferSize = 64 * 1024; // @@TODO: Config.
                            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
                            try {
                                await fs.WriteAsync(new ReadOnlyMemory<byte>(header));
                                while (true) {
                                    int n = await section.Body.ReadAsync(new Memory<byte>(buffer));
                                    if (n == 0) {
                                        break;
                                    }

                                    maxSize -= n;
                                    if (maxSize < 0) {
                                        maxSizeExceeded = true;
                                        break;
                                    }

                                    await fs.WriteAsync(new ReadOnlyMemory<byte>(buffer, 0, n));
                                }
                            } catch (Exception ex) {
                                fs.Dispose();
                                fileDisposed = true;
                                File.Delete(filePath);

                                return new FileError(ex.Message);
                            } finally {
                                if (!fileDisposed) {
                                    fs.Dispose();
                                }
                                ArrayPool<byte>.Shared.Return(buffer);
                            }
                        } catch (IOException) {
                            return new FileError("File uploading already in progress");
                        } catch (Exception ex) {
                            return new FileError(ex.Message);
                        }

                        if (maxSizeExceeded) {
                            File.Delete(filePath);
                            return new ValidationError("Max file size limit exceeded");
                        }
                    } else if (_multipartRequestHelper.HasFormDataContentDisposition(contentDisposition)) {
                        var key = HeaderUtilities.RemoveQuotes(contentDisposition.Name).Value;
                        var encoding = _getEncoding(section);

                        using (
                            var streamReader = new StreamReader(
                                section.Body, encoding,
                                detectEncodingFromByteOrderMarks: true,
                                bufferSize: 1024, leaveOpen: true
                            )
                        ) {
                            // @@NOTE: The value length limit is enforced by MultipartBodyLengthLimit.
                            var value = await streamReader.ReadToEndAsync();
                            if (string.Equals(value, "undefined", StringComparison.OrdinalIgnoreCase)) {
                                value = string.Empty;
                            }

                            formAccumulator.Append(key, value);
                        }
                    }
                }

                section = await reader.ReadNextSectionAsync();
            }

            if (filePath == null) {
                return new ValidationError("No file provided");
            }

            formAccumulator.Append("filePath", filePath);

            return new FormCollection(formAccumulator.GetResults());
        }

        private Encoding _getEncoding(MultipartSection section) {
            var hasContentTypeHeader = MediaTypeHeaderValue.TryParse(section.ContentType, out var contentType);
            // @@NOTE: UTF-7 is insecure and shouldn't be honored. UTF-8 succeeds in most cases.
#pragma warning disable SYSLIB0001
            if (!hasContentTypeHeader || Encoding.UTF7.Equals(contentType.Encoding)) {
                return Encoding.UTF8;
            }

            return contentType.Encoding;
        }

        public void DeleteFile(string filePath) {
            File.Delete(filePath); // no exception if does not exist
        }
    }
}
