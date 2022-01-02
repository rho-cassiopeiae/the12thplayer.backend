using System;
using System.Text.Json;

using EasyCompressor;

using Livescore.Application.Common.Interfaces;

namespace Livescore.Infrastructure.Serializer {
    public class JsonBrotliBase64Serializer : ISerializer {
        private readonly ICompressor _compressor;

        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public JsonBrotliBase64Serializer(ICompressor compressor) {
            _compressor = compressor;
            
            _jsonSerializerOptions = new JsonSerializerOptions {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public string Serialize<T>(T @object) {
            return Convert.ToBase64String(
                _compressor.Compress(
                    JsonSerializer.SerializeToUtf8Bytes(@object, _jsonSerializerOptions)
                )
            );
        }
    }
}
