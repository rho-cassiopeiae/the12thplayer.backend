using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Livescore.Infrastructure.FileUpload {
    internal class VideoFileValidator : IFileValidator {
        private static readonly string[] _mp4ftypSubtypes = {
            "avc1", "iso2", "isom", "mmp4", "mp41", "mp42",
            "mp71", "msnv", "ndas", "ndsc", "ndsh", "ndsm",
            "ndsp", "ndss", "ndxc", "ndxh", "ndxm", "ndxp",
            "ndxs", "MSNV"
        };

        private static readonly HashSet<string> _permittedExtensions = new() { // @@TODO: Config.
            ".mpg", ".mov", ".mp4"
        };

        private static readonly Dictionary<string, List<byte[]>> _fileExtensionToSignatures = new() {
            {
                ".mpg", new List<byte[]> {
                    new byte[] { 0x00, 0x00, 0x01, 0xB3 },
                    new byte[] { 0x00, 0x00, 0x01, 0xBA }
                }
            },
            {
                ".mov", new List<byte[]> {
                    new byte[] { 0x00, 0x00, 0x00, 0x00, 0x66, 0x74, 0x79, 0x70, 0x71, 0x74, 0x20, 0x20 }, // ftypqt
                    new byte[] { 0x6D, 0x6F, 0x6F, 0x76 }, // moov
                    new byte[] { 0x66, 0x72, 0x65, 0x65 }, // free
                    new byte[] { 0x6D, 0x64, 0x61, 0x74 }, // mdat
                    new byte[] { 0x77, 0x69, 0x64, 0x65 }, // wide
                    new byte[] { 0x70, 0x6E, 0x6F, 0x74 }, // pnot
                    new byte[] { 0x73, 0x6B, 0x69, 0x70 }  // skip
                }
            }
        };

        static VideoFileValidator() {
            var signatures = new List<byte[]>();
            foreach (var subtype in _mp4ftypSubtypes) {
                var subtypeBytes = Encoding.ASCII.GetBytes(subtype);
                var signature = new byte[8 + subtypeBytes.Length];
                signature[4] = 0x66;
                signature[5] = 0x74;
                signature[6] = 0x79;
                signature[7] = 0x70;
                for (int i = 8, j = 0; i < signature.Length; ++i, ++j) {
                    signature[i] = subtypeBytes[j];
                }
                signatures.Add(signature);
            }

            _fileExtensionToSignatures[".mp4"] = signatures;
        }

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

            bool valid = signatures.Any(signature => signature.Length > 4 ?
                header.Skip(4).SequenceEqual(signature.Skip(4)) :
                header.Take(signature.Length).SequenceEqual(signature)
            );

            return (Valid: valid, Ext: ext, Header: header);
        }
    }
}
