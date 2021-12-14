using System;
using System.Security.Cryptography;
using System.Text;

using Microsoft.Extensions.Configuration;

using Admin.Application.Common.Interfaces;

namespace Admin.Infrastructure.Identity {
    public class SuperuserSignatureVerifier : ISuperuserSignatureVerifier {
        private readonly string _superuserPublicKeyXml;

        public SuperuserSignatureVerifier(IConfiguration configuration) {
            _superuserPublicKeyXml = configuration["Superuser:PublicKey"];
        }

        public bool Verify(string payload, string signature) {
            using var rsa = RSA.Create();
            rsa.FromXmlString(_superuserPublicKeyXml);

            return rsa.VerifyData(
                Encoding.UTF8.GetBytes(payload),
                Convert.FromBase64String(signature),
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );
        }
    }
}
