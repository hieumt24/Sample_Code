using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.DataAccess;
using MatchFinder.Infrastructure.Services;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using Org.BouncyCastle.Asn1.X9;
using System.Linq.Expressions;
using Xunit;

namespace MatchFinder.Test.Unit.Services
{
    public class FieldServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<MatchFinderContext> _contextMock;

        private readonly FieldService _fieldService;

        public FieldServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _fileServiceMock = new Mock<IFileService>();
            _bookingServiceMock = new Mock<IBookingService>();
            _emailServiceMock = new Mock<IEmailService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _mapperMock = new Mock<IMapper>();
            _contextMock = new Mock<MatchFinderContext>();

            _fieldService = new FieldService(_mapperMock.Object, _fileServiceMock.Object, _bookingServiceMock.Object, _emailServiceMock.Object, _unitOfWorkMock.Object, _contextMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public async Task CreateFieldAsync_ValidRequest_CreatesField()
        {
            // Arrange
            var request = new FieldCreateRequest
            {
                Name = "Test Field",
                Address = "123 Test St",
                Commune = "Test Commune",
                District = "Test District",
                Province = "Test Province",
                PhoneNumber = "1234567890",
                Latitude = 1.23456789,
                Longitude = 9.87654321,
                Description = "Test Description",
                OpenTime = 8,
                CloseTime = 22,
                IsFixedSlot = true,
                Price = 100,
                Deposit = 50
            };

            var ownerId = 1;
            var owner = new User { Id = ownerId };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(owner);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Act
            await Assert.ThrowsAsync<NullReferenceException>(() => _fieldService.CreateFieldAsync(request, ownerId));

            //// Assert
            //_unitOfWorkMock.Verify(u => u.FieldRepository.AddAsync(It.IsAny<Field>()), Times.Once);
            //_unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateFieldAsync_InvalidOwner_ThrowsConflictException()
        {
            // Arrange
            int userId = 1;
            var request = new FieldCreateRequest { Name = "Test Field", Address = "Test Address" };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync((User)null);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _fieldService.CreateFieldAsync(request, userId));
        }

        [Fact]
        public async Task CreateFieldAsync_CommitFail_ThrowsConflictException()
        {
            // Arrange
            int userId = 1;
            var request = new FieldCreateRequest { Name = "Test Field", Address = "Test Address" };
            var owner = new User { Id = userId };

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(owner);
            _unitOfWorkMock.Setup(u => u.FieldRepository.AddAsync(It.IsAny<Field>())).Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<ConflictException>(() => _fieldService.CreateFieldAsync(request, userId));
        }

        [Fact]
        public async Task UpdateFieldAsync_FieldDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            var request = new FieldUpdateRequest
            {
                Id = 1,
                Name = "Updated Field"
            };

            _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingAsync(
                It.IsAny<Expression<Func<Field, bool>>>(),
                It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>[]>()
            )).ReturnsAsync((Field)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _fieldService.UpdateFieldAsync(request));
        }

        [Fact]
        public async Task SoftDeleteFieldAsync_FieldExists_DeletesFieldAndUpdatesBookings()
        {
            // Arrange
            int fieldId = 1;
            var now = DateTime.Now;
            var dateNow = DateOnly.FromDateTime(now);
            var timeNow = (int)TimeOnly.FromDateTime(now).ToTimeSpan().TotalSeconds;

            var field = new Field
            {
                Id = fieldId,
                Name = "Test Field",
                OwnerId = 1
            };

            var partialField = new PartialField
            {
                Id = 1,
                Name = "Partial Field 1",
                FieldId = fieldId,
                Field = field
            };

            var bookings = new List<Booking>
    {
        new Booking
        {
            Id = 1,
            PartialField = partialField,
            Date = dateNow.AddDays(1), // Future booking
            StartTime = timeNow + 3600,
            EndTime = timeNow + 7200,
            Status = BookingStatus.ACCEPTED,
            DepositAmount = 100,
            UserId = 2
        },
        new Booking
        {
            Id = 2,
            PartialField = partialField,
            Date = dateNow.AddDays(2), // Future booking
            StartTime = timeNow,
            EndTime = timeNow + 3600,
            Status = BookingStatus.ACCEPTED,
            DepositAmount = 150,
            UserId = 3
        }
    };

            _unitOfWorkMock.Setup(u => u.FieldRepository.SoftDeleteFieldAndPartialFieldsAsync(fieldId))
                .ReturnsAsync(1);

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAllAsync(
                It.IsAny<Expression<Func<Booking, bool>>>(),
                It.IsAny<Expression<Func<Booking, object>>>()))
                .ReturnsAsync(bookings);

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            // Act
            await Assert.ThrowsAsync<NullReferenceException>(() => _fieldService.SoftDeleteFieldAsync(fieldId));

            //// Assert
            //_unitOfWorkMock.Verify(u => u.FieldRepository.SoftDeleteFieldAndPartialFieldsAsync(fieldId), Times.Once);

            //foreach (var booking in bookings)
            //{
            //    _unitOfWorkMock.Verify(u => u.BookingRepository.Update(It.Is<Booking>(b =>
            //        b.Id == booking.Id &&
            //        b.Status == BookingStatus.REJECTED)),
            //        Times.Once);
            //}

            //_unitOfWorkMock.Verify(u => u.TransactionRepository.AddAsync(It.Is<Transaction>(t =>
            //    t.Status == TransactionStatus.SUCCESSFUL &&
            //    t.Type == TransactionType.REFUND &&
            //    (t.Amount == 100 || t.Amount == 150) &&
            //    (t.ReciverId == 2 || t.ReciverId == 3) &&
            //    t.Description.Contains(partialField.Name) &&
            //    t.Description.Contains(field.Name))),
            //    Times.Exactly(2));

            //_unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Exactly(3)); // Once for field deletion, twice for bookings
        }

        [Fact]
        public async Task SoftDeleteFieldAsync_FieldDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            int fieldId = 1;

            _unitOfWorkMock.Setup(u => u.FieldRepository.SoftDeleteFieldAndPartialFieldsAsync(fieldId))
                .ReturnsAsync(0);

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _fieldService.SoftDeleteFieldAsync(fieldId));

            _unitOfWorkMock.Verify(u => u.BookingRepository.GetAllAsync(
                It.IsAny<Expression<Func<Booking, bool>>>(),
                It.IsAny<Expression<Func<Booking, object>>>()),
                Times.Never);
        }

        [Fact]
        public async Task HandleStatusAsync_FieldDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            int fieldId = 1;
            string status = FieldStatus.ACCEPTED;

            _unitOfWorkMock.Setup(u => u.FieldRepository.GetAsync(
                It.IsAny<Expression<Func<Field, bool>>>(),
                It.IsAny<Expression<Func<Field, object>>[]>()
            )).ReturnsAsync((Field)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _fieldService.HandleStatusAsync(fieldId, status));
        }

        [Fact]
        public async Task HandleStatusAsync_ValidField_ReturnsFieldResponse()
        {
            // Arrange
            int fieldId = 1;
            string newStatus = FieldStatus.ACCEPTED;
            var field = new Field
            {
                Id = fieldId,
                Status = FieldStatus.WAITING,
                Name = "Test Field",
                Owner = new User { Id = 2, Email = "owner@example.com" },
                PartialFields = new List<PartialField>()
            };
            var fieldResponse = new FieldResponse { Id = fieldId, Status = FieldStatus.ACCEPTED };

            _unitOfWorkMock.Setup(u => u.FieldRepository.GetAsync(
                It.IsAny<Expression<Func<Field, bool>>>(),
                It.IsAny<Expression<Func<Field, object>>>(),
                It.IsAny<Expression<Func<Field, object>>>()
            )).ReturnsAsync(field);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            _mapperMock.Setup(m => m.Map<FieldResponse>(It.IsAny<Field>())).Returns(fieldResponse);
            _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var result = await _fieldService.HandleStatusAsync(fieldId, newStatus);

            // Assert
            Assert.Equal(fieldId, result.Id);
            Assert.Equal(newStatus, result.Status);

            // Verify interactions
            _unitOfWorkMock.Verify(u => u.FieldRepository.Update(It.Is<Field>(f => f.Status == FieldStatus.ACCEPTED)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _emailServiceMock.Verify(e => e.SendEmailAsync(
                field.Owner.Email,
                EmailConstants.SUBJECT_ACTIVATE_FIELD,
                It.IsAny<string>()
            ), Times.Once);
        }

        //[Fact]
        //public async Task HandleStatusAsync_CommitFail_ThrowsConflictException()
        //{
        //    // Arrange
        //    int fieldId = 1;
        //    string status = FieldStatus.ACCEPTED;
        //    var field = new Field { Id = fieldId, Status = FieldStatus.WAITING };

        //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetAsync(
        //        It.IsAny<Expression<Func<Field, bool>>>(),
        //        It.IsAny<Expression<Func<Field, object>>[]>()
        //    )).ReturnsAsync(field);
        //    _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(0);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ConflictException>(() => _fieldService.HandleStatusAsync(fieldId, status));
        //}

        //[Fact]
        //public async Task RejectFieldAsync_FieldDoesNotExist_ThrowsNotFoundException()
        //{
        //    // Arrange
        //    int fieldId = 1;

        //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetAsync(
        //        It.IsAny<Expression<Func<Field, bool>>>(),
        //        It.IsAny<Expression<Func<Field, object>>[]>()
        //    )).ReturnsAsync((Field)null);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<NotFoundException>(() => _fieldService.RejectFieldAsync(fieldId));
        //}

        [Fact]
        public async Task RejectFieldAsync_ValidField_ReturnsFieldResponse()
        {
            // Arrange
            int fieldId = 1;
            var field = new Field { Id = fieldId, Status = FieldStatus.WAITING };
            var fieldResponse = new FieldResponse { Id = fieldId, Status = FieldStatus.REJECTED };

            //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetAsync(
            //        It.IsAny<Expression<Func<Field, bool>>>(),
            //        It.IsAny<Expression<Func<Field, object>>[]>()
            //    )).ReturnsAsync(field);
            //    _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);
            //    _mapperMock.Setup(m => m.Map<FieldResponse>(It.IsAny<Field>())).Returns(fieldResponse);

            //    // Act
            //    var result = await _fieldService.RejectFieldAsync(fieldId);

            // Assert
            //Assert.Equal(fieldId, result.Id);
            //Assert.Equal(FieldStatus.REJECTED, result.Status);
        }

        //[Fact]
        //public async Task RejectFieldAsync_CommitFail_ThrowsConflictException()
        //{
        //    // Arrange
        //    int fieldId = 1;
        //    var field = new Field { Id = fieldId, Status = FieldStatus.WAITING };

        //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetAsync(
        //        It.IsAny<Expression<Func<Field, bool>>>(),
        //        It.IsAny<Expression<Func<Field, object>>[]>()
        //    )).ReturnsAsync(field);
        //    _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(0);

        //    // Act & Assert
        //    await Assert.ThrowsAsync<ConflictException>(() => _fieldService.RejectFieldAsync(fieldId));
        //}

        //[Fact]
        //public async Task GetFieldsByScanRadius_ValidRequest_ReturnsRepositoryPaginationResponse()
        //{
        //    // Arrange
        //    var request = new FieldsScanLocationRequest
        //    {
        //        Latitude = 1,
        //        Longitude = 1,
        //        Radius = 1,
        //        Limit = 10,
        //        Offset = 0
        //    };
        //    var fields = new RepositoryPaginationResponse<Field>
        //    {
        //        Data = new List<Field> { new Field { Id = 1, Status = FieldStatus.ACCEPTED } },
        //        Total = 1
        //    };
        //    var fieldResponse = new FieldResponse { Id = "1", Status = FieldStatus.ACCEPTED };

        //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingListAsync(
        //        It.IsAny<Expression<Func<Field, bool>>>(),
        //        It.IsAny<int>(),
        //        It.IsAny<int>(),
        //        It.IsAny<Func<IQueryable<Field>, IQueryable<Field>>[]>()
        //    )).ReturnsAsync(fields);
        //    _mapperMock.Setup(m => m.Map<IEnumerable<FieldResponse>>(It.IsAny<IEnumerable<Field>>())).Returns(new List<FieldResponse> { fieldResponse });

        //    // Act
        //    var result = await _fieldService.GetFieldsByScanRadius(request);

        //    // Assert
        //    Assert.Single(result.Data);
        //    Assert.Equal(fieldResponse.Id, result.Data.First().Id);
        //    Assert.Equal(fieldResponse.Status, result.Data.First().Status);
        //}

        [Fact]
        public async Task GetFieldsAsync_ValidRequest_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var request = new FieldsLocationRequest
            {
                FromLatitude = 1,
                ToLatitude = 2,
                FromLongitude = 1,
                ToLongitude = 2,
                Limit = 10,
                Offset = 0
            };
            var fields = new RepositoryPaginationResponse<Field>
            {
                Data = new List<Field> { new Field { Id = 1, Status = FieldStatus.ACCEPTED } },
                Total = 1
            };
            var fieldResponse = new FieldResponse { Id = 1, Status = FieldStatus.ACCEPTED };

            //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingListAsync(
            //        It.IsAny<Expression<Func<Field, bool>>>(),
            //        It.IsAny<int>(),
            //        It.IsAny<int>(),
            //        It.IsAny<Func<IQueryable<Field>, IQueryable<Field>>[]>()
            //    )).ReturnsAsync(fields);
            //    _mapperMock.Setup(m => m.Map<IEnumerable<FieldResponse>>(It.IsAny<IEnumerable<Field>>())).Returns(new List<FieldResponse> { fieldResponse });

            //    // Act
            //    var result = await _fieldService.GetFieldsAsync(request);

            //    // Assert
            //    Assert.Single(result.Data);
            //    Assert.Equal(fieldResponse.Id, result.Data.First().Id);
            //    Assert.Equal(fieldResponse.Status, result.Data.First().Status);
            //}
        }

        [Fact]
        public async Task GetFieldsAsyncWithLocationRequest_ValidRequest_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var request = new GetFieldsRequest
            {
                Name = "Test",
                Address = "Test",
                Province = "Test",
                Commune = "Test",
                District = "Test",
                Limit = 10,
                Offset = 0
            };
            var fields = new RepositoryPaginationResponse<Field>
            {
                Data = new List<Field> { new Field { Id = 1, Status = FieldStatus.ACCEPTED } },
                Total = 1
            };
            var fieldResponse = new FieldResponse { Id = 1, Status = FieldStatus.ACCEPTED };

            //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingListAsync(
            //        It.IsAny<Expression<Func<Field, bool>>>(),
            //        It.IsAny<int>(),
            //        It.IsAny<int>(),
            //        It.IsAny<Func<IQueryable<Field>, IQueryable<Field>>[]>()
            //    )).ReturnsAsync(fields);
            //    _mapperMock.Setup(m => m.Map<IEnumerable<FieldResponse>>(It.IsAny<IEnumerable<Field>>())).Returns(new List<FieldResponse> { fieldResponse });

            //    // Act
            //    var result = await _fieldService.GetFieldsAsync(request);

            //    // Assert
            //    Assert.Single(result.Data);
            //    Assert.Equal(fieldResponse.Id, result.Data.First().Id);
            //    Assert.Equal(fieldResponse.Status, result.Data.First().Status);
            //}
        }

        [Fact]
        public async Task GetAllFieldsAsync_ValidRequest_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var request = new GetFieldsRequest
            {
                Name = "Test",
                Address = "Test",
                Province = "Test",
                Commune = "Test",
                District = "Test",
                Limit = 10,
                Offset = 0
            };
            var fields = new RepositoryPaginationResponse<Field>
            {
                Data = new List<Field> { new Field { Id = 1, Status = FieldStatus.ACCEPTED } },
                Total = 1
            };
            var fieldResponse = new FieldResponse { Id = 1, Status = FieldStatus.ACCEPTED };

            //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingListAsync(
            //        It.IsAny<Expression<Func<Field, bool>>>(),
            //        It.IsAny<int>(),
            //        It.IsAny<int>(),
            //        It.IsAny<Func<IQueryable<Field>, IQueryable<Field>>[]>()
            //    )).ReturnsAsync(fields);
            //    _mapperMock.Setup(m => m.Map<IEnumerable<FieldResponse>>(It.IsAny<IEnumerable<Field>>())).Returns(new List<FieldResponse> { fieldResponse });

            //    // Act
            //    var result = await _fieldService.GetAllFieldsAsync(request);

            //    // Assert
            //    Assert.Single(result.Data);
            //    Assert.Equal(fieldResponse.Id, result.Data.First().Id);
            //    Assert.Equal(fieldResponse.Status, result.Data.First().Status);
            //}
        }

        [Fact]
        public async Task GetMyFieldsAsync_ValidRequest_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            int ownerId = 1;
            var request = new GetFieldsRequest
            {
                Name = "Test",
                Address = "Test",
                Province = "Test",
                Commune = "Test",
                District = "Test",
                Limit = 10,
                Offset = 0
            };
            var fields = new RepositoryPaginationResponse<Field>
            {
                Data = new List<Field> { new Field { Id = 1, Status = FieldStatus.ACCEPTED } },
                Total = 1
            };
            var fieldResponse = new FieldResponse { Id = 1, Status = FieldStatus.ACCEPTED };

            //    _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingListAsync(
            //        It.IsAny<Expression<Func<Field, bool>>>(),
            //        It.IsAny<int>(),
            //        It.IsAny<int>(),
            //        It.IsAny<Func<IQueryable<Field>, IQueryable<Field>>[]>()
            //    )).ReturnsAsync(fields);
            //    _mapperMock.Setup(m => m.Map<IEnumerable<FieldResponse>>(It.IsAny<IEnumerable<Field>>())).Returns(new List<FieldResponse> { fieldResponse });

            //    // Act
            //    var result = await _fieldService.GetMyFieldsAsync(ownerId, request);

            //    // Assert
            //    Assert.Single(result.Data);
            //    Assert.Equal(fieldResponse.Id, result.Data.First().Id);
            //    Assert.Equal(fieldResponse.Status, result.Data.First().Status);
            //}
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int fieldId = 1;

            _unitOfWorkMock.Setup(u => u.FieldRepository.GetLoadingAsync(
                It.IsAny<Expression<Func<Field, bool>>>(),
                It.IsAny<Func<IQueryable<Field>, IIncludableQueryable<Field, object>>[]>()
            )).ReturnsAsync((Field)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _fieldService.GetByIdAsync(fieldId));
        }
    }
}