using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Superuser {
    class Program {
        public static void Main(string[] args) {
            var payload = args[0];
            var keyPath = args[1];

            using var fs = new FileStream(keyPath, FileMode.Open, FileAccess.Read);
            using var reader = new StreamReader(fs);
            using var rsa = RSA.Create();
            rsa.FromXmlString(reader.ReadToEnd());
            var signature = Convert.ToBase64String(
                rsa.SignData(Encoding.UTF8.GetBytes(payload), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1)
            );

            Console.WriteLine(signature);
        }
    }
}
