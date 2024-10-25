using AutoMapper;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Moq;
using System.Linq.Expressions;
using Xunit;
using static MatchFinder.Application.Models.Requests.UserRequest;

namespace MatchFinder.Application.Services.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _fileServiceMock = new Mock<IFileService>();
            _userService = new UserService(_unitOfWorkMock.Object, _mapperMock.Object, _fileServiceMock.Object);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, Role = new Role { Name = "Admin" } };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByEmailAsync(email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(email, result.Email);
        }

        [Fact]
        public async Task GetUserByEmailAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange
            var email = "test@example.com";

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync((User)null);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _userService.GetUserByEmailAsync(email));
        }

        [Fact]
        public async Task GetUserVerificationAsync_WhenUserExists_ReturnsUser()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserVerificationAsync(u => u.Email == user.Email);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUserVerificationAsync_WhenUserDoesNotExist_ReturnsNull()
        {
            // Arrange

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _userService.GetUserVerificationAsync(u => u.Email == "test@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ActiveAccount_WhenUserExists_ActivatesAccountAndReturnsOne()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, Status = UserStatus.IN_ACTIVE };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(1);

            // Act
            var result = await _userService.ActiveAccount(email);

            // Assert
            Assert.Equal(1, result);
            Assert.Equal(UserStatus.ACTIVE, user.Status);
            _unitOfWorkMock.Verify(x => x.UserRepository.Update(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task ActiveAccount_WhenUserDoesNotExist_ThrowsKeyNotFoundException()
        {
            // Arrange
            var email = "nonexistent@example.com";
            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                               .ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _userService.ActiveAccount(email));
        }

        [Fact]
        public async Task SearchAsync_WhenUsersExist_ReturnsUsers()
        {
            // Arrange
            var request = new UserFilterRequest
            {
                Email = "test@gmail.com",
                UserName = "test",
                PhoneNumber = "1234567890",
                Status = "ACTIVE",
                FromCreateDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), // Ensure FromDate is before ToDate
                ToCreateDate = DateOnly.FromDateTime(DateTime.Now),
            };

            var users = new RepositoryPaginationResponse<User>
            {
                Data = new[]
                {
                    new User { Email = "test@gmail.com", UserName = "test", PhoneNumber = "1234567890", Status = UserStatus.ACTIVE, CreatedAt = DateTime.Now, Role = new Role { Name = "Admin" } }
                    },
                Total = 1
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetListAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync(users);

            // Act
            var result = await _userService.SearchAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Total);
        }

        [Fact]
        public async Task GetDetailUserFullField_WhenUserExists_ReturnsUserResponse()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                Email = "test@example.com",
                UserName = "testuser",
                PhoneNumber = "1234567890",
                Status = UserStatus.ACTIVE,
                CreatedAt = DateTime.Now,
                Role = new Role { Name = "Admin" }
            };

            var userResponse = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Status = user.Status.ToString(),
                CreatedAt = user.CreatedAt,
                RoleName = user.Role.Name
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync(user);

            _mapperMock.Setup(x => x.Map<UserResponse>(user))
                       .Returns(userResponse);

            // Act
            var result = await _userService.GetDetailUserFullField(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userResponse.Id, result.Id);
            Assert.Equal(userResponse.Email, result.Email);
            Assert.Equal(userResponse.UserName, result.UserName);
            Assert.Equal(userResponse.PhoneNumber, result.PhoneNumber);
            Assert.Equal(userResponse.Status, result.Status);
            Assert.Equal(userResponse.CreatedAt, result.CreatedAt);
            Assert.Equal(userResponse.RoleName, result.RoleName);
        }

        [Fact]
        public async Task GetDetailUserFullField_WhenUserDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1;

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<Expression<Func<User, object>>>()))
                           .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _userService.GetDetailUserFullField(userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsUserViewFromOther()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Status = UserStatus.ACTIVE
            };

            var expectedUserView = new UserViewFromOther
            {
                UserName = user.UserName,
                Status = user.Status.ToString()
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUserView.UserName, result.UserName);
            Assert.Equal(expectedUserView.Status, result.Status);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var userId = 1;

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _userService.GetUserByIdAsync(userId);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }

        [Fact]
        public async Task UpdateStatusUser_WhenUserExists_UpdatesStatusAndReturnsUserViewFromOther()
        {
            // Arrange
            var userId = 1;
            var newStatus = "ACTIVE";
            var request = new UserChangeStatusRequest
            {
                Id = userId,
                Status = newStatus
            };

            var user = new User
            {
                Id = userId,
                UserName = "testuser",
                Status = UserStatus.IN_ACTIVE
            };

            var expectedUserView = new UserViewFromOther
            {
                UserName = user.UserName,
                Status = UserStatus.ACTIVE.ToString()
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);
            _unitOfWorkMock.Setup(x => x.UserRepository.Update(user));
            _unitOfWorkMock.Setup(x => x.CommitAsync()).ReturnsAsync(1);

            // Act
            var result = await _userService.UpdateStatusUser(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUserView.UserName, result.UserName);
            Assert.Equal(expectedUserView.Status, result.Status);
            Assert.Equal(UserStatus.ACTIVE, user.Status);
            _unitOfWorkMock.Verify(x => x.UserRepository.Update(user), Times.Once);
            _unitOfWorkMock.Verify(x => x.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateStatusUser_WhenUserDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var request = new UserChangeStatusRequest
            {
                Id = 1,
                Status = "ACTIVE"
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync((User)null);

            // Act
            Func<Task> act = async () => await _userService.UpdateStatusUser(request);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(act);
        }
    }
}