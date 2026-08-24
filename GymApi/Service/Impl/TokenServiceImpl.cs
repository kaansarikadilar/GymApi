using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using GymApi.Models;
using GymApi.Service;
using Microsoft.IdentityModel.Tokens;

namespace GymApi.Services.Impl
{
    public class TokenServiceImpl : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly SymmetricSecurityKey _decryptionKey;

        public TokenServiceImpl(IConfiguration config)
        {
            _config = config;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
            _decryptionKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:DecryptionKey"]!));
        }

        public string CreateToken(AppUser user,string role)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Role, role) // Required for [Authorize(Roles = "...")]
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],
                
                // 1. Digital Signature
                SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha512Signature),
                
                // 2. JWE Encryption Layer
                EncryptingCredentials = new EncryptingCredentials(
                    _decryptionKey,
                    SecurityAlgorithms.Aes256KW, 
                    SecurityAlgorithms.Aes256CbcHmacSha512
                )
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}