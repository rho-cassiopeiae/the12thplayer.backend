using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Feed.Infrastructure.FileUpload {
    internal class ImageFileValidator : IFileValidator {
        private static readonly HashSet<string> _permittedExtensions = new() { // @@TODO: Config.
            ".jpg", ".jpeg", ".png"
        };

        private static readonly Dictionary<string, List<byte[]>> _fileExtensionToSignatures = new() {
            {
                ".jpg", new List<byte[]> {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                }
            },
            {
                ".jpeg", new List<byte[]> {
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                    new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 }
                }
            },
            {
                ".png", new List<byte[]> {
                    new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                }
            }
        };

        public async Task<(bool Valid, string Ext, byte[] Header)> ValidateFileExtensionAndSignature(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition
        ) {
            var fileName = contentDisposition.FileName.Value;
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            if (
                string.IsNullOrEmpty(ext) ||
                !_permittedExtensions.Contains(ext) ||
                !_fileExtensionToSignatures.ContainsKey(ext)
            ) {
                return (Valid: false, Ext: null, Header: null);
            }

            var signatures = _fileExtensionToSignatures[ext];
            var header = new byte[signatures.Max(signature => signature.Length)];

            int bytesToRead = header.Length;
            int bytesRead = 0;
            while (bytesRead < bytesToRead) {
                int n = await section.Body.ReadAsync(header, bytesRead, bytesToRead - bytesRead); // @@TODO: Cancel.
                if (n == 0) {
                    return (Valid: false, Ext: null, Header: null);
                }
                bytesRead += n;
            }

            bool valid = signatures.Any(
                signature => header.Take(signature.Length).SequenceEqual(signature)
            );

            return (Valid: valid, Ext: ext, Header: header);
        }
    }
}
