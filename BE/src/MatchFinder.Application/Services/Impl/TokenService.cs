using MatchFinder.Domain.Entities;
using MatchFinder.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MatchFinder.Application.Services.Impl
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly ICryptographyHelper _cryptographyHelper;

        public TokenService(IConfiguration configuration, ICryptographyHelper cryptographyHelper)
        {
            _configuration = configuration;
            _cryptographyHelper = cryptographyHelper;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role?.Name ?? "Player")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                TokenSalt = _cryptographyHelper.GenerateSalt(),
                TokenHash = _cryptographyHelper.GenerateHash(Guid.NewGuid().ToString()),
                CreatedAt = DateTime.UtcNow,
                ExpireAt = DateTime.UtcNow.AddDays(7)
            };

            return refreshToken;
        }
    }
}