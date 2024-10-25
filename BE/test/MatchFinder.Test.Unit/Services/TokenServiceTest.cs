using MatchFinder.Domain.Entities;
using MatchFinder.Infrastructure.Helpers;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace MatchFinder.Application.Services.Impl.Tests
{
    public class TokenServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ICryptographyHelper> _cryptographyHelperMock;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _cryptographyHelperMock = new Mock<ICryptographyHelper>();
            _tokenService = new TokenService(_configurationMock.Object, _cryptographyHelperMock.Object);
        }

        [Fact]
        public void GenerateToken_WhenCalled_ReturnsValidToken()
        {
            // Arrange
            var user = new User
            {
                Id = 1,
                Role = new Role { Name = "Admin" }
            };

            _configurationMock.Setup(x => x["Jwt:Key"]).Returns("This is a secret key for testing");
            _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");

            // Act
            var token = _tokenService.GenerateToken(user);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            Assert.NotNull(jsonToken);
            Assert.Equal("TestIssuer", jsonToken.Issuer);
            Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == "1");
            Assert.Contains(jsonToken.Claims, c => c.Type == ClaimTypes.Role && c.Value == "Admin");
        }

        [Fact]
        public void GenerateRefreshToken_WhenCalled_ReturnsValidRefreshToken()
        {
            // Arrange
            var salt = "testSalt";
            var hash = "testHash";
            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns(salt);
            _cryptographyHelperMock.Setup(x => x.GenerateHash(It.IsAny<string>())).Returns(hash);

            // Act
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Assert
            Assert.NotNull(refreshToken);
            Assert.Equal(salt, refreshToken.TokenSalt);
            Assert.Equal(hash, refreshToken.TokenHash);
            Assert.True(refreshToken.ExpireAt > DateTime.UtcNow);
            Assert.True(refreshToken.ExpireAt <= DateTime.UtcNow.AddDays(7));
        }
    }
}