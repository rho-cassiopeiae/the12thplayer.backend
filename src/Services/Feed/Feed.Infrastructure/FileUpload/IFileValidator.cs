﻿using System.Threading.Tasks;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;

namespace Feed.Infrastructure.FileUpload {
    internal interface IFileValidator {
        Task<(bool Valid, string Ext, byte[] Header)> ValidateFileExtensionAndSignature(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition
        );
    }
}
