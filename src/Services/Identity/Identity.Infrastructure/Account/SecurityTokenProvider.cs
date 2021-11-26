using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using Identity.Application.Common.Interfaces;

namespace Identity.Infrastructure.Account {
    public class SecurityTokenProvider : ISecurityTokenProvider {
        private readonly SigningCredentials _credentials;
        private readonly RsaSecurityKey _publicKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiresInMin;

        public SecurityTokenProvider(IConfiguration configuration) {
            var jwtConfig = configuration.GetSection("Jwt");

            var rsaPrivateKey = RSA.Create(); // @@NOTE: Important to not dispose.
            rsaPrivateKey.FromXmlString(jwtConfig["PrivateKey"]);
            _credentials = new SigningCredentials(
                key: new RsaSecurityKey(rsaPrivateKey),
                algorithm: SecurityAlgorithms.RsaSha256
            );

            var rsaPublicKey = RSA.Create(); // @@NOTE: Important to not dispose.
            rsaPublicKey.FromXmlString(jwtConfig["PublicKey"]);
            _publicKey = new RsaSecurityKey(rsaPublicKey);

            _issuer = jwtConfig["Issuer"];
            _audience = jwtConfig["Audience"];
            _expiresInMin = jwtConfig.GetValue<int>("ExpiresInMin");
        }

        public string GenerateJwt(long sub, IList<Claim> claims = null) {
            claims = claims ?? new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, sub.ToString()));

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiresInMin),
                signingCredentials: _credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken() {
            var randomNumber = new byte[32];
            var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
