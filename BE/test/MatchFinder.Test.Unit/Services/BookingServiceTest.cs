using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace MatchFinder.Test.Unit.Services
{
    public class BookingServiceTest
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IPayOSPaymentService> _payOSPaymentServiceMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ITransactionService> _transactionServiceMock;

        private BookingService _bookingService;

        public BookingServiceTest()
        {
            _mapperMock = new Mock<IMapper>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _payOSPaymentServiceMock = new Mock<IPayOSPaymentService>();
            _notificationServiceMock = new Mock<INotificationService>();
            _transactionServiceMock = new Mock<ITransactionService>();
            _bookingService = new BookingService(_mapperMock.Object, _unitOfWorkMock.Object, _transactionServiceMock.Object, _payOSPaymentServiceMock.Object, _notificationServiceMock.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_UserNotFound_ThrowsConflictException()
        {
            // Arrange
            var request = new BookingCreateRequest { TeamId = 1, PartialFieldId = 1, Date = DateOnly.FromDateTime(DateTime.Today).AddDays(1), StartTime = 3600, EndTime = 7200 };

            _unitOfWorkMock.Setup(u => u.TeamRepository.GetAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                           .ReturnsAsync(new Team());

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _bookingService.CreateBookingAsync(request, 1));
            Assert.Equal("User request not exist!", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_FieldNotFound_ThrowsConflictException()
        {
            // Arrange
            var request = new BookingCreateRequest { TeamId = 1, PartialFieldId = 1, Date = DateOnly.FromDateTime(DateTime.Today).AddDays(1), StartTime = 3600, EndTime = 7200 };
            var user = new User { Id = 1, Amount = 1000 };

            _unitOfWorkMock.Setup(u => u.TeamRepository.GetAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                           .ReturnsAsync(new Team());

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>(), It.IsAny<Expression<Func<PartialField, object>>>()))
                           .ReturnsAsync((PartialField)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _bookingService.CreateBookingAsync(request, 1));
            Assert.Equal("Field not exist!", exception.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_OverlappingBooking_ThrowsConflictException()
        {
            // Arrange
            var request = new BookingCreateRequest { TeamId = 1, PartialFieldId = 1, Date = DateOnly.FromDateTime(DateTime.Today).AddDays(1), StartTime = 3600, EndTime = 7200 };
            var pfield = new PartialField { Id = 1, Field = new Field { IsDeleted = false, Status = "ACCEPTED",
                InactiveTimes = new List<InactiveTime>()
            } };
            var existingBooking = new Booking { PartialFieldId = 1, Date = request.Date, StartTime = 3500, EndTime = 4000 };

            _unitOfWorkMock.Setup(u => u.TeamRepository.GetAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                           .ReturnsAsync(new Team());
            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(new User());
            _unitOfWorkMock.Setup(u => u.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>(), It.IsAny<Expression<Func<PartialField, object>>[]>()))
                           .ReturnsAsync(pfield);
            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAllAsync(It.IsAny<Expression<Func<Booking, bool>>>()))
                           .ReturnsAsync(new List<Booking> { existingBooking });

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _bookingService.CreateBookingAsync(request, 1));
            Assert.Equal("Đã có người đặt sân trong khoảng thời gian này!", exception.Message);
        }
        [Fact]
        public async Task CreateBookingAsync_InsufficientFunds_ThrowsConflictException()
        {
            // Arrange
            var userId = 1;
            var request = new BookingCreateRequest
            {
                TeamId = 1,
                PartialFieldId = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                StartTime = 3600, // 1 hour from midnight
                EndTime = 7200 // 2 hours from midnight
            };

            var user = new User { Id = userId };
            var team = new Team { Id = request.TeamId };
            var partialField = new PartialField
            {
                Id = request.PartialFieldId,
                Field = new Field
                {
                    Id = 1,
                    IsDeleted = false,
                    Status = "ACCEPTED",
                    OwnerId = 2,
                    Deposit = 100,
                    InactiveTimes = new List<InactiveTime>(),
                    Staffs = new List<Staff>()
                }
            };

            _unitOfWorkMock.Setup(u => u.TeamRepository.GetAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                .ReturnsAsync(team);

            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                .ReturnsAsync(user);

            _unitOfWorkMock.Setup(u => u.PartialFieldRepository.GetAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(),
                It.IsAny<Expression<Func<PartialField, object>>>(),
                It.IsAny<Expression<Func<PartialField, object>>>()))
                .ReturnsAsync(partialField);

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAllAsync(It.IsAny<Expression<Func<Booking, bool>>>()))
                .ReturnsAsync(new List<Booking>());

            // Setup TransactionService to return insufficient funds
            _transactionServiceMock.Setup(t => t.GetAmountAsync(userId))
                .ReturnsAsync(50m); // User has 50, but deposit required is 100

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(
                () => _bookingService.CreateBookingAsync(request, userId));

            Assert.Equal("Số dư không đủ!", exception.Message);

            // Verify that no booking was added or committed
            _unitOfWorkMock.Verify(u => u.BookingRepository.AddAsync(It.IsAny<Booking>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateBookingAsync_ValidRequest_ReturnsBookingResponse()
        {
            // Arrange
            var request = new BookingCreateRequest
            {
                TeamId = 1,
                PartialFieldId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                StartTime = 3600,
                EndTime = 7200
            };
            var pfield = new PartialField
            {
                Id = 1,
                Field = new Field
                {
                    IsDeleted = false,
                    Status = "ACCEPTED",
                    OwnerId = 2,
                    Name = "Field1",
                    Deposit = 100,
                    Price = 2,
                    InactiveTimes = new List<InactiveTime>()
                }
            };
            var user = new User
            {
                Id = 1,
                Transactions = new List<Transaction>
        {
            new Transaction
            {
                Amount = 1000,
                Type = TransactionType.RECHARGE,
                Status = TransactionStatus.SUCCESSFUL
            }
        }
            };
            var team = new Team { Id = 1 };
            var newBooking = new Booking
            {
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PartialField = pfield,
                User = user,
                Team = team,
                Status = BookingStatus.WAITING
            };

            _unitOfWorkMock.Setup(u => u.TeamRepository.GetAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                           .ReturnsAsync(team);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.PartialFieldRepository.GetAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(),
                It.IsAny<Expression<Func<PartialField, object>>[]>()))
                           .ReturnsAsync(pfield);
            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAllAsync(It.IsAny<Expression<Func<Booking, bool>>>()))
                           .ReturnsAsync(new List<Booking>());
            _unitOfWorkMock.Setup(u => u.BookingRepository.AddAsync(It.IsAny<Booking>()))
                           .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(1);

            // Mock for TransactionRepository
            var transactionRepositoryMock = new Mock<ITransactionRepository>();
            transactionRepositoryMock.Setup(t => t.AddAsync(It.IsAny<Transaction>()))
                                     .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.TransactionRepository)
                           .Returns(transactionRepositoryMock.Object);

            _mapperMock.Setup(m => m.Map<BookingResponse>(It.IsAny<Booking>()))
                       .Returns(new BookingResponse());

            // Mock for TransactionService
            _transactionServiceMock.Setup(t => t.GetAmountAsync(It.IsAny<int>()))
                                   .ReturnsAsync(100000m);

            // Act
            var result = await _bookingService.CreateBookingAsync(request, 1);

            // Assert
            Assert.NotNull(result);
            _notificationServiceMock.Verify(n => n.SendNotificationToListUser(It.IsAny<List<int>>(), It.IsAny<Notification>()), Times.Once);
            _transactionServiceMock.Verify(t => t.GetAmountAsync(It.IsAny<int>()), Times.Once);
            transactionRepositoryMock.Verify(t => t.AddAsync(It.IsAny<Transaction>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_BookingFailed_ThrowsConflictException()
        {
            // Arrange
            var request = new BookingCreateRequest
            {
                TeamId = 1,
                PartialFieldId = 1,
                Date = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), 
                StartTime = 3600,
                EndTime = 7200
            };
            var pfield = new PartialField
            {
                Id = 1,
                Field = new Field
                {
                    IsDeleted = false,
                    Status = "ACCEPTED",
                    OwnerId = 2,
                    Name = "Field1",
                    Deposit = 100,
                    Price = 2,
                    InactiveTimes = new List<InactiveTime>() 
                }
            };
            var user = new User { Id = 1, Amount = 1000 };
            var team = new Team { Id = 1 };
            var newBooking = new Booking
            {
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PartialField = pfield,
                User = user,
                Team = team,
                Status = BookingStatus.WAITING
            };

            _unitOfWorkMock.Setup(u => u.TeamRepository.GetAsync(It.IsAny<Expression<Func<Team, bool>>>()))
                           .ReturnsAsync(team);
            _unitOfWorkMock.Setup(u => u.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);
            _unitOfWorkMock.Setup(u => u.PartialFieldRepository.GetAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(),
                It.IsAny<Expression<Func<PartialField, object>>[]>()))
                           .ReturnsAsync(pfield);
            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAllAsync(It.IsAny<Expression<Func<Booking, bool>>>()))
                           .ReturnsAsync(new List<Booking>());
            _unitOfWorkMock.Setup(u => u.BookingRepository.AddAsync(It.IsAny<Booking>()))
                           .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(0);
            _mapperMock.Setup(m => m.Map<BookingResponse>(It.IsAny<Booking>()))
                       .Returns(new BookingResponse());

            _transactionServiceMock.Setup(t => t.GetAmountAsync(It.IsAny<int>()))
                                   .ReturnsAsync(1000m);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _bookingService.CreateBookingAsync(request, 1));
            Assert.Equal("Đặt sân không thành công!", exception.Message);
        }

        [Fact]
        public async Task GetListAsync_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var pagination = new SearchBookingRequest { Date = DateOnly.FromDateTime(DateTime.Today), StartTime = 3600, EndTime = 7200, UserId = 1, PartialFieldId = 1, Status = "ACCEPTED" };
            var bookings = new RepositoryPaginationResponse<Booking>
            {
                Data = new List<Booking> { new Booking { Id = 1 } },
                Total = 1
            };

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetListAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(bookings);

            _mapperMock.Setup(m => m.Map<IEnumerable<BookingResponse>>(It.IsAny<IEnumerable<Booking>>()))
                       .Returns(new List<BookingResponse> { new BookingResponse() });

            // Act
            var result = await _bookingService.GetListAsync(pagination);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Data);
            Assert.Equal(1, result.Total);
        }

        [Fact]
        public async Task GetListAsync_FilterList_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var pagination = new ListBookingBusyRequest { Date = DateOnly.FromDateTime(DateTime.Today), StartTime = 3600, EndTime = 7200, PartialFieldId = 1 };
            var bookings = new List<Booking>
            {
                new Booking { Id = 1, Date = pagination.Date.Value, StartTime = pagination.StartTime.Value, EndTime = pagination.EndTime.Value, PartialFieldId = pagination.PartialFieldId.Value }
            };

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAllAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(bookings);

            _mapperMock.Setup(m => m.Map<IEnumerable<BookingResponse>>(It.IsAny<IEnumerable<Booking>>()))
                       .Returns(new List<BookingResponse> { new BookingResponse() });

            // Act
            var result = await _bookingService.GetListAsync(pagination, 1);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Data);
            Assert.Equal(1, result.Total);
        }

        [Fact]
        public async Task GetByIdAsync_BookingNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync((Booking)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _bookingService.GetByIdAsync(1));
            Assert.Equal("Booking not exist!", exception.Message);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBookingResponse()
        {
            // Arrange
            var booking = new Booking { Id = 1, PartialField = new PartialField { Field = new Field { IsDeleted = false, Status = "ACCEPTED" } } };

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(booking);

            _mapperMock.Setup(m => m.Map<BookingResponse>(It.IsAny<Booking>()))
                       .Returns(new BookingResponse());

            // Act
            var result = await _bookingService.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetMyBooking_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var request = new MyBookingRequest { Limit = 10, Offset = 0 };
            var bookings = new RepositoryPaginationResponse<Booking>
            {
                Data = new List<Booking> { new Booking { Id = 1 } },
                Total = 1
            };

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetListAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(bookings);

            _mapperMock.Setup(m => m.Map<IEnumerable<BookingResponse>>(It.IsAny<IEnumerable<Booking>>()))
                       .Returns(new List<BookingResponse> { new BookingResponse() });

            // Act
            var result = await _bookingService.GetMyBooking(1, request);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Data);
            Assert.Equal(1, result.Total);
        }

        [Fact]
        public async Task RejectBookingAsync_BookingNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync((Booking)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _bookingService.RejectBookingAsync(1));
            Assert.Equal("Booking not exist!", exception.Message);
        }

        [Fact]
        public async Task RejectBookingAsync_ValidBooking_ReturnsBookingResponse()
        {
            // Arrange
            int bookingId = 1;
            var booking = new Booking
            {
                Id = bookingId,
                Status = BookingStatus.WAITING,
                DepositAmount = 100,
                UserId = 2,
                PartialField = new PartialField
                {
                    Name = "Field 1",
                    Field = new Field
                    {
                        Name = "Main Field",
                        OwnerId = 3
                    }
                },
                Date = DateOnly.FromDateTime(DateTime.Today),
                StartTime = 3600,
                EndTime = 7200,
                User = new User(),
                Team = new Team(),
                Rates = new List<Rate>(),
                OpponentFindings = new List<OpponentFinding>()
            };

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAsync(
                It.IsAny<Expression<Func<Booking, bool>>>(),
                It.IsAny<Expression<Func<Booking, object>>[]>()))
                .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(u => u.BookingRepository.Update(It.IsAny<Booking>()))
                .Verifiable();

            _unitOfWorkMock.Setup(u => u.CommitAsync())
                .ReturnsAsync(1);

            var transactionRepositoryMock = new Mock<ITransactionRepository>();
            transactionRepositoryMock.Setup(t => t.AddAsync(It.IsAny<Transaction>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.TransactionRepository)
                .Returns(transactionRepositoryMock.Object);

            _mapperMock.Setup(m => m.Map<BookingResponse>(It.IsAny<Booking>()))
                .Returns(new BookingResponse { Id = bookingId, Status = BookingStatus.CANCELED });

            // Act
            var result = await _bookingService.RejectBookingAsync(bookingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(BookingStatus.CANCELED, result.Status);

            _unitOfWorkMock.Verify(u => u.BookingRepository.Update(It.Is<Booking>(b => b.Status == BookingStatus.CANCELED)), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Exactly(2));
            transactionRepositoryMock.Verify(t => t.AddAsync(It.Is<Transaction>(tr =>
                tr.Status == TransactionStatus.SUCCESSFUL &&
                tr.Amount == 100 &&
                tr.Type == TransactionType.BOOKING &&
                tr.ReciverId == 2)), Times.Once);
            _mapperMock.Verify(m => m.Map<BookingResponse>(It.IsAny<Booking>()), Times.Once);
        }

        [Fact]
        public async Task RejectBookingAsync_BookingFailed_ThrowsConflictException()
        {
            // Arrange
            var booking = new Booking { Id = 1, Status = BookingStatus.WAITING, DepositAmount = 100, User = new User { Amount = 1000 } };

            _unitOfWorkMock.Setup(u => u.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(), It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(u => u.CommitAsync()).ReturnsAsync(0);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _bookingService.RejectBookingAsync(1));
            Assert.Equal("The requested booking failed!", exception.Message);
        }
    }
}