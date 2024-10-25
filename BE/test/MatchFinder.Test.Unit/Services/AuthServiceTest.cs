using AutoMapper;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.Helpers;
using MatchFinder.Infrastructure.Services;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.Linq.Expressions;
using Xunit;
using static System.Net.WebRequestMethods;

namespace MatchFinder.Test.Unit.Services
{
    public class AuthServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly Mock<ICryptographyHelper> _cryptographyHelperMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IVerificationService> _verificationMock;

        private readonly AuthService _authService;

        public AuthServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _tokenServiceMock = new Mock<ITokenService>();
            _cryptographyHelperMock = new Mock<ICryptographyHelper>();
            _emailServiceMock = new Mock<IEmailService>();
            _mapperMock = new Mock<IMapper>();
            _verificationMock = new Mock<IVerificationService>();

            _authService = new AuthService(_unitOfWorkMock.Object, _tokenServiceMock.Object, _cryptographyHelperMock.Object, _emailServiceMock.Object, _verificationMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailExists_ThrowInvalidOperationException()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "123456";
            var confirmPassword = "123456";
            var username = "test";
            var phoneNumber = "123456";
            int role = 1;

            var user = new User
            {
                Email = email
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _authService.RegisterAsync(email, password, confirmPassword, username, phoneNumber, role);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task RegisterAsync_WhenRegistrationSuccessfulButEmailFails_ThrowConflictException()
        {
            // Arrange
            var email = "newuser@gmail.com";
            var password = "123456";
            var confirmPassword = "123456";
            var username = "newuser";
            var phoneNumber = "123456";
            int role = 1;
            var user = new User
            {
                Id = 1,
                Email = email,
                UserName = username,
                PasswordHash = "hashedPassword",
                PasswordSalt = "salt",
                Status = UserStatus.IN_ACTIVE,
                RoleId = role,
                PhoneNumber = phoneNumber,
            };
            var otp = new Verification { TokenHash = "testTokenHash" };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User)null);
            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns("salt");
            _cryptographyHelperMock.Setup(x => x.HashPassword(password, "salt")).Returns("hashedPassword");
            _unitOfWorkMock.Setup(x => x.UserRepository.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _verificationMock.Setup(x => x.GenerateTokenAsync(It.IsAny<int>())).ReturnsAsync(otp);
            _emailServiceMock.Setup(x => x.SendEmailAsync(email, EmailConstants.SUBJECT_ACTIVE_ACCOUNT, It.IsAny<string>())).ReturnsAsync(false);

            // Act
            Func<Task> act = async () => await _authService.RegisterAsync(email, password, confirmPassword, username, phoneNumber, role);

            // Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Failed to send email", exception.Message);

            _verificationMock.Verify(x => x.GenerateTokenAsync(It.IsAny<int>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(email, EmailConstants.SUBJECT_ACTIVE_ACCOUNT, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenRegistrationAndEmailAreSuccessful_ReturnUserResponse()
        {
            // Arrange
            var email = "newuser@gmail.com";
            var password = "123456";
            var confirmPassword = "123456";
            var username = "newuser";
            var phoneNumber = "123456";
            int role = 3;
            var user = new User
            {
                Id = 1, 
                Email = email,
                UserName = username,
                PasswordHash = "hashedPassword",
                PasswordSalt = "salt",
                Status = UserStatus.IN_ACTIVE,
                RoleId = RoleConstant.PLAYER_ID,
                PhoneNumber = phoneNumber,
            };
            var userResponse = new UserResponse
            {
                Email = email,
                UserName = username
            };
            var otp = new Verification { TokenHash = "testTokenHash" }; 

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User)null);
            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns("salt");
            _cryptographyHelperMock.Setup(x => x.HashPassword(password, "salt")).Returns("hashedPassword");
            _unitOfWorkMock.Setup(x => x.UserRepository.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _emailServiceMock.Setup(x => x.SendEmailAsync(email, It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
            _mapperMock.Setup(x => x.Map<UserResponse>(It.IsAny<User>())).Returns(userResponse);

            // Add these lines to mock _verificationService
            _verificationMock.Setup(x => x.GenerateTokenAsync(It.IsAny<int>())).ReturnsAsync(otp);

            // Act
            var result = await _authService.RegisterAsync(email, password, confirmPassword, username, phoneNumber, role);

            // Assert
            Assert.Equal(userResponse, result);
            _verificationMock.Verify(x => x.GenerateTokenAsync(It.IsAny<int>()), Times.Once);
            _emailServiceMock.Verify(x => x.SendEmailAsync(email, EmailConstants.SUBJECT_ACTIVE_ACCOUNT, It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "nonexistent@gmail.com";
            var password = "password";
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(email, password);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "wrongpassword";
            var user = new User
            {
                Email = email,
                PasswordHash = "correcthash",
                PasswordSalt = "salt",
                Status = UserStatus.ACTIVE
            };
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.VerifyPassword(password, user.PasswordHash, user.PasswordSalt)).Returns(false);

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(email, password);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task LoginAsync_WhenUserAccountIsNotActivated_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "password";
            var user = new User
            {
                Email = email,
                PasswordHash = "correcthash",
                PasswordSalt = "salt",
                Status = UserStatus.IN_ACTIVE
            };
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.VerifyPassword(password, user.PasswordHash, user.PasswordSalt)).Returns(true);

            // Act
            Func<Task> act = async () => await _authService.LoginAsync(email, password);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task LoginAsync_WhenLoginIsSuccessful_ReturnsTokenRefreshTokenAndUserResponse()
        {
            // Arrange
            var email = "test@gmail.com";
            var password = "password";
            var user = new User
            {
                Email = email,
                PasswordHash = "correcthash",
                PasswordSalt = "salt",
                Status = UserStatus.ACTIVE,
                Role = new Role { Id = 1, Name = "Player" }
            };
            var token = "generatedToken";
            var refreshToken = new RefreshToken
            {
                TokenHash = "refreshTokenHash",
                UserId = user.Id
            };
            var userResponse = new UserResponse
            {
                Email = email,
                UserName = "test"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>())).ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.VerifyPassword(password, user.PasswordHash, user.PasswordSalt)).Returns(true);
            _tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns(token);
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns(refreshToken);
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.AddAsync(refreshToken)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(userResponse);

            // Act
            var result = await _authService.LoginAsync(email, password);

            // Assert
            Assert.Equal((token, refreshToken.TokenHash, userResponse), result);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenDoesNotExist_ThrowsSecurityTokenException()
        {
            // Arrange
            var refreshToken = "nonexistentToken";
            var userId = 1;
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync((RefreshToken)null);

            // Act
            Func<Task> act = async () => await _authService.RefreshTokenAsync(refreshToken, userId);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenIsExpired_ThrowsSecurityTokenException()
        {
            // Arrange
            var refreshToken = "expiredToken";
            var userId = 1;
            var expiredToken = new RefreshToken
            {
                TokenHash = refreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(-1)
            };
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(expiredToken);

            // Act
            Func<Task> act = async () => await _authService.RefreshTokenAsync(refreshToken, userId);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var refreshToken = "validToken";
            var userId = 1;
            var validToken = new RefreshToken
            {
                TokenHash = refreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(1),
                UserId = 1
            };
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(validToken);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.RefreshTokenAsync(refreshToken, userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenRefreshIsSuccessful_ReturnsNewTokenRefreshTokenAndUserResponse()
        {
            // Arrange
            var refreshToken = "validToken";
            var userId = 1;
            var validToken = new RefreshToken
            {
                TokenHash = refreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(1),
                UserId = 1
            };
            var user = new User
            {
                Id = 1,
                Email = "test@gmail.com",
                UserName = "test",
                Role = new Role { Id = 1, Name = "Player" }
            };
            var newJwtToken = "newJwtToken";
            var newRefreshToken = new RefreshToken
            {
                TokenHash = "newRefreshToken",
                UserId = user.Id
            };
            var userResponse = new UserResponse
            {
                Email = user.Email,
                UserName = user.UserName
            };

            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(validToken);
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);
            _tokenServiceMock.Setup(x => x.GenerateToken(user)).Returns(newJwtToken);
            _tokenServiceMock.Setup(x => x.GenerateRefreshToken()).Returns(newRefreshToken);
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.Delete(validToken)).Verifiable();
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.AddAsync(newRefreshToken)).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);
            _mapperMock.Setup(x => x.Map<UserResponse>(user)).Returns(userResponse);

            // Act
            var result = await _authService.RefreshTokenAsync(refreshToken, userId);

            // Assert
            Assert.Equal((newJwtToken, newRefreshToken.TokenHash), result);
        }

        [Fact]
        public async Task LogoutAsync_WhenUserHasNoTokens_ReturnsZero()
        {
            // Arrange
            int userId = 1;
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetAllAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(new List<RefreshToken>());

            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(0);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task LogoutAsync_WhenUserHasTokens_RemovesTokensAndReturnsCommitCount()
        {
            // Arrange
            int userId = 1;
            var tokens = new List<RefreshToken>
        {
            new RefreshToken { UserId = userId, TokenHash = "token1" },
            new RefreshToken { UserId = userId, TokenHash = "token2" }
        };
            _unitOfWorkMock.Setup(x => x.RefreshTokenRepository.GetAllAsync(It.IsAny<Expression<Func<RefreshToken, bool>>>()))
                .ReturnsAsync(tokens);

            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.LogoutAsync(userId);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenPasswordsDoNotMatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = "test@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "differentpassword";

            // Act
            Func<Task> act = async () => await _authService.ResetPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var email = "nonexistent@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.ResetPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task ResetPasswordAsync_WhenPasswordResetIsSuccessful_ReturnsCommitCount()
        {
            // Arrange
            var email = "test@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";
            var user = new User
            {
                Email = email,
                PasswordHash = "oldhash",
                PasswordSalt = "oldsalt"
            };
            var newSalt = "newsalt";
            var newHash = "newhash";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns(newSalt);
            _cryptographyHelperMock.Setup(x => x.HashPassword(newPassword, newSalt)).Returns(newHash);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.ResetPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenPasswordsDoNotMatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = "test@gmail.com";
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmPassword = "differentpassword";

            // Act
            Func<Task> act = async () => await _authService.ChangePasswordAsync(email, oldPassword, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var email = "nonexistent@gmail.com";
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.ChangePasswordAsync(email, oldPassword, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenOldPasswordIsIncorrect_ThrowsUnauthorizedAccessException()
        {
            // Arrange
            var email = "test@gmail.com";
            var oldPassword = "wrongpassword";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";
            var user = new User
            {
                Email = email,
                PasswordHash = "correcthash",
                PasswordSalt = "salt"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.VerifyPassword(oldPassword, user.PasswordHash, user.PasswordSalt)).Returns(false);

            // Act
            Func<Task> act = async () => await _authService.ChangePasswordAsync(email, oldPassword, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenPasswordChangeIsSuccessful_ReturnsCommitCount()
        {
            // Arrange
            var email = "test@gmail.com";
            var oldPassword = "oldpassword";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";
            var user = new User
            {
                Email = email,
                PasswordHash = "oldhash",
                PasswordSalt = "oldsalt"
            };
            var newSalt = "newsalt";
            var newHash = "newhash";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.VerifyPassword(oldPassword, user.PasswordHash, user.PasswordSalt)).Returns(true);
            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns(newSalt);
            _cryptographyHelperMock.Setup(x => x.HashPassword(newPassword, newSalt)).Returns(newHash);
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.ChangePasswordAsync(email, oldPassword, newPassword, confirmPassword);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task NewPasswordAsync_WhenPasswordsDoNotMatch_ThrowsInvalidOperationException()
        {
            // Arrange
            var email = "test@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "differentpassword";

            // Act
            Func<Task> act = async () => await _authService.NewPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task NewPasswordAsync_WhenUserNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var email = "nonexistent@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.NewPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task NewPasswordAsync_WhenVerificationNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var email = "test@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";
            var user = new User
            {
                Email = email,
                Verifications = new List<Verification>()
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);

            // Act
            Func<Task> act = async () => await _authService.NewPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task NewPasswordAsync_WhenPasswordChangeIsSuccessful_ReturnsCommitCount()
        {
            // Arrange
            var email = "test@gmail.com";
            var newPassword = "newpassword";
            var confirmPassword = "newpassword";
            var user = new User
            {
                Email = email,
                Verifications = new List<Verification>
        {
            new Verification
            {
                IsVerified = true,
                ExpireAt = DateTime.UtcNow.AddDays(1),
                CreatedAt = DateTime.UtcNow
            }
        }
            };
            var newSalt = "newsalt";
            var newHash = "newhash";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);
            _cryptographyHelperMock.Setup(x => x.GenerateSalt()).Returns(newSalt);
            _cryptographyHelperMock.Setup(x => x.HashPassword(newPassword, newSalt)).Returns(newHash);

            var verificationRepositoryMock = new Mock<IVerificationRepository>();
            _unitOfWorkMock.Setup(x => x.VerificationRepository).Returns(verificationRepositoryMock.Object);

            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.NewPasswordAsync(email, newPassword, confirmPassword);

            // Assert
            Assert.Equal(1, result);
            verificationRepositoryMock.Verify(x => x.Update(It.IsAny<Verification>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task VerifyOTPAsync_WhenTokenIsNullOrEmpty_ThrowsArgumentException()
        {
            // Arrange
            var user = new User();
            string token = null;

            // Act
            Func<Task> act = async () => await _authService.VerifyOTPAsync(user, token);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task VerifyOTPAsync_WhenVerificationNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var user = new User
            {
                Verifications = new List<Verification>()
            };
            var token = "validtoken";

            // Act
            Func<Task> act = async () => await _authService.VerifyOTPAsync(user, token);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task VerifyOTPAsync_WhenOTPIsInvalid_ThrowsInvalidOperationException()
        {
            // Arrange
            var user = new User
            {
                Verifications = new List<Verification>
            {
                new Verification
                {
                    TokenHash = "validtoken",
                    ExpireAt = DateTime.UtcNow.AddMinutes(5),
                    CreatedAt = DateTime.UtcNow
                }
            }
            };
            var token = "invalidtoken";

            // Act
            Func<Task> act = async () => await _authService.VerifyOTPAsync(user, token);

            // Assert
            await Assert.ThrowsAsync<ConflictException>(act);
        }

        [Fact]
        public async Task VerifyOTPAsync_WhenOTPIsValid_ReturnsTrue()
        {
            // Arrange
            var user = new User
            {
                Verifications = new List<Verification>
            {
                new Verification
                {
                    TokenHash = "validtoken",
                    ExpireAt = DateTime.UtcNow.AddMinutes(5),
                    CreatedAt = DateTime.UtcNow
                }
            }
            };
            var token = "validtoken";

            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _authService.VerifyOTPAsync(user, token);

            // Assert
            Assert.True(result);
        }

        //public async Task<CurrentUserResponse> GetCurrentUser(int userId)
        //{
        //    var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId, u => u.Role);
        //    if (user == null)
        //    {
        //        throw new NotFoundException("User not found");
        //    }
        //    return _mapper.Map<CurrentUserResponse>(user);
        //}

        [Fact]
        public async Task GetCurrentUser_WhenUserNotFound_ThrowsNotFoundException()
        {
            // Arrange
            int userId = 1;
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _authService.GetCurrentUser(userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task GetCurrentUser_WhenUserFound_ReturnsCurrentUserResponse()
        {
            // Arrange
            int userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "test@gmail.com",
                UserName = "test",
                Role = new Role { Id = 1, Name = "Player" }
            };
            var currentUserResponse = new CurrentUserResponse
            {
                Id = user.Id,
                RoleName = user.Role.Name,
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>[]>()))
                .ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map<CurrentUserResponse>(user)).Returns(currentUserResponse);

            // Act
            var result = await _authService.GetCurrentUser(userId);

            // Assert
            Assert.Equal(currentUserResponse, result);
        }
    }
}