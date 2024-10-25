using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.Helpers;
using Moq;
using Xunit;

namespace MatchFinder.Application.Services.Tests
{
    public class VerificationServiceTests
    {
        private readonly Mock<ICryptographyHelper> _cryptographyHelperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VerificationService _verificationService;

        public VerificationServiceTests()
        {
            _cryptographyHelperMock = new Mock<ICryptographyHelper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _verificationService = new VerificationService(_cryptographyHelperMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GenerateTokenAsync_WhenCalled_ReturnsVerificationToken()
        {
            // Arrange
            var userId = 1;
            var salt = "testSalt";
            var hash = "testHash";

            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns(salt);
            _cryptographyHelperMock.Setup(x => x.GenerateHash(It.IsAny<string>())).Returns(hash);
            _unitOfWorkMock.Setup(x => x.VerificationRepository.AddAsync(It.IsAny<Verification>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _verificationService.GenerateTokenAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(salt, result.TokenSalt);
            Assert.Equal(hash, result.TokenHash);
            Assert.True(result.ExpireAt > DateTime.UtcNow);
            Assert.True(result.ExpireAt <= DateTime.UtcNow.AddMinutes(30));

            _unitOfWorkMock.Verify(x => x.VerificationRepository.AddAsync(It.IsAny<Verification>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }
    }
}