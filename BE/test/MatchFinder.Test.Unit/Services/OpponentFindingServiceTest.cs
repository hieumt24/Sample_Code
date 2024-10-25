using AutoMapper;
using MatchFinder.Application.Attributes;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.UnitOfWork;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MatchFinder.Test.Unit.Services
{
    public class OpponentFindingServiceTest
    {
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<INotificationService> _notificationMock;

        private readonly OpponentFindingService _opponentFindingService;

        public OpponentFindingServiceTest()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mapperMock = new Mock<IMapper>();
            _notificationMock = new Mock<INotificationService>();

            _opponentFindingService = new OpponentFindingService(_unitOfWorkMock.Object, _mapperMock.Object, _notificationMock.Object);
        }

        [Fact]
        public async Task CreateNewOpponentFinding_WhenBookingIsInvalid_ThrowConflictException()
        {
            //Arrange
            var request = new OpponentFindingCreateRequest
            {
                BookingId = 1,
                Content = "test"
            };

            var userRequestId = 1;

            _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
                                                                It.IsAny<Expression<Func<Booking, object>>[]>()))
                                                                .ReturnsAsync((Booking)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Booking is invalid", exception.Message);
        }

        [Fact]
        public async Task CreateNewOpponentFinding_WhenOpponentFindingIsAlreadyExist_ThrowConflictException()
        {
            //Arrange
            var request = new OpponentFindingCreateRequest
            {
                BookingId = 1,
                Content = "test"
            };

            var userRequestId = 1;

            var booking = new Booking
            {
                Id = 1,
                UserId = 2
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                BookingId = 1
            };

            _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
                                                                It.IsAny<Expression<Func<Booking, object>>[]>()))
                                                                .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                                                                .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding already exist", exception.Message);
        }

        //[Fact]
        //public async Task CreateNewOpponentFinding_WhenBookingDateTimeIsLessThanCurrentDateTime_ThrowConflictException()
        //{
        //    //Arrange
        //    var request = new OpponentFindingCreateRequest
        //    {
        //        BookingId = 1,
        //        Content = "test"
        //    };

        //    var userRequestId = 1;

        //    var booking = new Booking
        //    {
        //        Id = 1,
        //        UserId = 2,
        //        Date = DateOnly.FromDateTime(DateTime.Now),
        //        StartTime = 60 * 60 * 13,
        //        EndTime = 60 * 60 * 15
        //    };

        //    _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
        //                                                        It.IsAny<Expression<Func<Booking, object>>[]>()))
        //                                                        .ReturnsAsync(booking);

        //    _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
        //                                                        .ReturnsAsync((OpponentFinding)null);

        //    //Act
        //    Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);

        //    //Assert
        //    var exception = await Assert.ThrowsAsync<ConflictException>(act);
        //    Assert.Equal("Booking end time is in the past, cannot create opponent finding", exception.Message);
        //}

        [Fact]
        public async Task CreateNewOpponentFinding_WhenRequestAcceptedOverlap_ThrowConflictException()
        {
            // Arrange
            var request = new OpponentFindingCreateRequest
            {
                BookingId = 1,
                Content = "test"
            };

            var userRequestId = 1;

            var booking = new Booking
            {
                Id = 1,
                UserId = userRequestId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                PartialField = new PartialField
                {
                    FieldId = 1
                },
                Status = BookingStatus.ACCEPTED
            };

            var opponentFindingRequest = new OpponentFindingRequest
            {
                Id = 1,
                OpponentFindingId = 2,
                IsAccepted = true
            };

            _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
                                                                It.IsAny<Expression<Func<Booking, object>>[]>()))
                                                                .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                           .ReturnsAsync((OpponentFinding)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>(),
                                                                                        It.IsAny<Expression<Func<OpponentFindingRequest, object>>[]>()))
                            .ReturnsAsync(new List<OpponentFindingRequest> { opponentFindingRequest });

            // Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);

            // Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overlapped with other accepted requests", exception.Message);
        }

        [Fact]
        public async Task CreateNewOpponentFinding_WhenOverlappedWithOtherOpponentFinding_ThrowConflictException()
        {
            // Arrange
            var request = new OpponentFindingCreateRequest
            {
                BookingId = 1,
                Content = "test"
            };

            var userRequestId = 1;

            var booking = new Booking
            {
                Id = 1,
                UserId = userRequestId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                PartialField = new PartialField
                {
                    FieldId = 1
                },
                Status = BookingStatus.ACCEPTED
            };

            var overlappingOpponentFinding = new OpponentFinding
            {
                Id = 2,
                UserFindingId = userRequestId,
                Date = booking.Date,
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                Status = OpponentFindingStatus.FINDING
            };

            _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
                                                                    It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                           .ReturnsAsync((OpponentFinding)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>(),
                                                                                        It.IsAny<Expression<Func<OpponentFindingRequest, object>>[]>()))
                .ReturnsAsync(new List<OpponentFindingRequest> { });

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                               It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(new List<OpponentFinding> { overlappingOpponentFinding });

            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);

            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overlapped with other opponent finding", exception.Message);
        }

        [Fact]
        public async Task CreateNewOpponentFinding_WhenSuccessfully_CreateANewPost()
        {
            // Arrange
            var request = new OpponentFindingCreateRequest
            {
                BookingId = 1,
                Content = "test"
            };

            var userRequestId = 1;

            var booking = new Booking
            {
                Id = 1,
                UserId = userRequestId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                PartialField = new PartialField
                {
                    FieldId = 1
                }
            };

            var opponentFinding = new OpponentFindingResponse
            {
                Id = 1,
                BookingId = 1
            };

            _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
                                                                    It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                           .ReturnsAsync((OpponentFinding)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync((IEnumerable<OpponentFindingRequest>)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.AddAsync(It.IsAny<OpponentFinding>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                .ReturnsAsync(1);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFinding);

            // Act
            var result = await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateNewOpponentFinding_WhenCreateFail_ThrowConflictException()
        {
            // Arrange
            var request = new OpponentFindingCreateRequest
            {
                BookingId = 1,
                Content = "test"
            };

            var userRequestId = 1;

            var booking = new Booking
            {
                Id = 1,
                UserId = userRequestId,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                PartialField = new PartialField
                {
                    FieldId = 1
                }
            };

            var opponentFinding = new OpponentFindingResponse
            {
                Id = 1,
                BookingId = 1
            };

            _unitOfWorkMock.Setup(x => x.BookingRepository.GetAsync(It.IsAny<Expression<Func<Booking, bool>>>(),
                                                                    It.IsAny<Expression<Func<Booking, object>>[]>()))
                           .ReturnsAsync(booking);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                           .ReturnsAsync((OpponentFinding)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync((IEnumerable<OpponentFindingRequest>)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.AddAsync(It.IsAny<OpponentFinding>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                .ReturnsAsync(0);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFinding);

            // Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);

            // Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Create new opponent finding failed", exception.Message);
        }

        [Fact]
        public async Task CreateNewOpponentFindingNotBooking_WhenUserNotFound_ThrowNotFoundException()
        {
            //Arrange
            var request = new OpponentFindingNotBookingCreateRequest
            {
                Content = "test",
                FieldProvince = "test",
                FieldDistrict = "test",
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var userRequestId = 1;

            var user = new User
            {
                Id = 1,
                Status = UserStatus.IN_ACTIVE
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync((User)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("User not found", exception.Message);
        }

        //[Fact]
        //public async Task CreateNewOpponentFindingNotBooking_WhenDateTimeIsLessThanCurrentDateTime_ThrowNotFoundException()
        //{
        //    //Arrange
        //    var request = new OpponentFindingNotBookingCreateRequest
        //    {
        //        Content = "test",
        //        FieldProvince = "test",
        //        FieldDistrict = "test",
        //        StartTime = 60 * 60 * 13,
        //        EndTime = 60 * 60 * 15,
        //        Date = DateOnly.FromDateTime(DateTime.Now)
        //    };

        //    var userRequestId = 1;

        //    var user = new User
        //    {
        //        Id = 1,
        //        Status = UserStatus.ACTIVE
        //    };

        //    _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
        //                   .ReturnsAsync(user);

        //    //Act
        //    Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);

        //    //Assert
        //    var exception = await Assert.ThrowsAsync<ConflictException>(act);
        //    Assert.Equal("Opponent finding end time is in the past, cannot create opponent finding", exception.Message);
        //}

        [Fact]
        public async Task CreateNewOpponentFindingNotBooking_WhenRequestAcceptedOverlap_ThrowConflictException()
        {
            // Arrange
            var request = new OpponentFindingNotBookingCreateRequest
            {
                Content = "test",
                FieldProvince = "test",
                FieldDistrict = "test",
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var userRequestId = 1;

            var user = new User
            {
                Id = 1,
                Status = UserStatus.ACTIVE
            };

            var opponentFindingRequest = new OpponentFindingRequest
            {
                Id = 1,
                OpponentFindingId = 2,
                IsAccepted = true
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>(),
                                                                                        It.IsAny<Expression<Func<OpponentFindingRequest, object>>[]>()))
                            .ReturnsAsync(new List<OpponentFindingRequest> { opponentFindingRequest });

            // Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);

            // Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overlapped with other accepted requests", exception.Message);
        }

        [Fact]
        public async Task CreateNewOpponentFindingNotBooking_WhenOverlappedWithOtherOpponentFinding_ThrowConflictException()
        {
            // Arrange
            var request = new OpponentFindingNotBookingCreateRequest
            {
                Content = "test",
                FieldProvince = "test",
                FieldDistrict = "test",
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var userRequestId = 1;

            var user = new User
            {
                Id = 1,
                Status = UserStatus.ACTIVE
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                UserFindingId = userRequestId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = OpponentFindingStatus.FINDING
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>(),
                                                                                        It.IsAny<Expression<Func<OpponentFindingRequest, object>>[]>()))
                            .ReturnsAsync(new List<OpponentFindingRequest> { });

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                               It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(new List<OpponentFinding> { opponentFinding });

            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);

            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overlapped with other opponent finding", exception.Message);
        }

        [Fact]
        public async Task CreateNewOpponentFindingNotBooking_WhenSuccessfully_CreateANewPost()
        {
            // Arrange
            var request = new OpponentFindingNotBookingCreateRequest
            {
                Content = "test",
                FieldProvince = "test",
                FieldDistrict = "test",
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var userRequestId = 1;

            var user = new User
            {
                Id = 1,
                Status = UserStatus.ACTIVE
            };

            var opponentFinding = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                           .ReturnsAsync((OpponentFinding)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync((IEnumerable<OpponentFindingRequest>)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.AddAsync(It.IsAny<OpponentFinding>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                .ReturnsAsync(1);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFinding);

            // Act
            var result = await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateNewOpponentFindingNotBooking_WhenCreateFail_ThrowConflictException()
        {
            // Arrange
            var request = new OpponentFindingNotBookingCreateRequest
            {
                Content = "test",
                FieldProvince = "test",
                FieldDistrict = "test",
                StartTime = 60 * 60 * 13,
                EndTime = 60 * 60 * 15,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            var userRequestId = 1;

            var user = new User
            {
                Id = 1,
                Status = UserStatus.ACTIVE
            };

            var opponentFinding = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(user);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>()))
                           .ReturnsAsync((OpponentFinding)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync((IEnumerable<OpponentFindingRequest>)null);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.AddAsync(It.IsAny<OpponentFinding>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                .ReturnsAsync(0);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFinding);

            // Act
            Func<Task> act = async () => await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);

            // Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Create new opponent finding failed", exception.Message);
        }

        [Fact]
        public async Task GetOpponentFinding_WhenOpponentFindingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var opponentFindingId = 1;

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((OpponentFinding)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.GetOpponentFinding(opponentFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Opponent finding not found", exception.Message);
        }

        [Fact]
        public async Task GetOpponentFinding_WhenOpponentFindingFound_ReturnOpponentFinding()
        {
            //Arrange
            var opponentFindingId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                       .Returns(opponentFindingResponse);

            //Act
            var result = await _opponentFindingService.GetOpponentFinding(opponentFindingId);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateOpponentFinding_WhenOpponentFindingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var opponentFindingId = 1;

            var request = new OpponentFindingUpdateRequest
            {
                Content = "test"
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((OpponentFinding)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.UpdateOpponentFinding(opponentFindingId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Opponent finding not found", exception.Message);
        }

        [Fact]
        public async Task UpdateOpponentFinding_WhenOpponentFindingIsOverdue_ThrowConflictException()
        {
            //Arrange
            var opponentFindingId = 1;

            var request = new OpponentFindingUpdateRequest
            {
                Content = "test"
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.FINDING,
                IsOverdue = true
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.UpdateOpponentFinding(opponentFindingId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overdue", exception.Message);
        }

        [Fact]
        public async Task UpdateOpponentFinding_WhenSuccessfully_UpdateOpponentFinding()
        {
            //Arrange
            var opponentFindingId = 1;

            var request = new OpponentFindingUpdateRequest
            {
                Content = "test"
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.FINDING,
                IsOverdue = false
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.Update(It.IsAny<OpponentFinding>()));

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(1);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                       .Returns(opponentFindingResponse);

            //Act
            var result = await _opponentFindingService.UpdateOpponentFinding(opponentFindingId, request);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateOpponentFinding_WhenUpdateFail_ThrowConflictException()
        {
            //Arrange
            var opponentFindingId = 1;

            var request = new OpponentFindingUpdateRequest
            {
                Content = "test"
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.FINDING,
                IsOverdue = false
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.Update(It.IsAny<OpponentFinding>()));

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(0);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                       .Returns(opponentFindingResponse);

            //Act
            Func<Task> act = async () => await _opponentFindingService.UpdateOpponentFinding(opponentFindingId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Update opponent finding failed", exception.Message);
        }

        [Fact]
        public async Task CancelPost_WhenOpponentFindingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((OpponentFinding)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CancelPost(opponentFindingId, userFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Opponent finding not found", exception.Message);
        }

        [Fact]
        public async Task CancelPost_WhenOpponentFindingIsOverdue_ThrowConflictException()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.FINDING,
                IsOverdue = true
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CancelPost(opponentFindingId, userFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overdue", exception.Message);
        }

        [Fact]
        public async Task CancelPost_WhenOpponentFindingIsAccepted_UpdateOpponentFindingRequest()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.ACCEPTED,
                IsOverdue = false
            };

            var opponentFindingRequest = new OpponentFindingRequest
            {
                Id = 1,
                OpponentFindingId = 1,
                IsAccepted = true,
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync(opponentFindingRequest);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.Update(It.IsAny<OpponentFindingRequest>()));

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.Update(It.IsAny<OpponentFinding>()));

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(1);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFindingResponse);

            //Act
            var result = await _opponentFindingService.CancelPost(opponentFindingId, userFindingId);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CancelPost_WhenCancelPostFail_ThrowConflictException()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.FINDING,
                IsOverdue = false
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.Update(It.IsAny<OpponentFinding>()));

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(0);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFindingResponse);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CancelPost(opponentFindingId, userFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Cancel opponent finding failed", exception.Message);
        }

        [Fact]
        public async Task CheckOpponentFindingExisted_WhenCalled_ReturnOpponentFindingOverlap()
        {
            //Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var startTime = 60 * 60 * 13;
            var endTime = 60 * 60 * 15;

            var opponentFinding = new OpponentFinding
            {
                Id = 1
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                             It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(new List<OpponentFinding> { opponentFinding });

            _mapperMock.Setup(x => x.Map<IEnumerable<OpponentFindingResponse>>(It.IsAny<IEnumerable<OpponentFinding>>()))
                .Returns(new List<OpponentFindingResponse> { opponentFindingResponse });

            //Act
            var result = await _opponentFindingService.CheckOpponentFindingExisted(userId, date, startTime, endTime);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CheckOpponentFindingExisted_WhenCalled_ReturnEmptyList()
        {
            //Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var startTime = 60 * 60 * 13;
            var endTime = 60 * 60 * 15;

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                             It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((IEnumerable<OpponentFinding>)null);

            //Act
            var result = await _opponentFindingService.CheckOpponentFindingExisted(userId, date, startTime, endTime);

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task CheckOpponentFindingAccepted_WhenCalled_ReturnOpponentFindingRequest()
        {
            //Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var startTime = 60 * 60 * 13;
            var endTime = 60 * 60 * 15;

            var opponentFindingRequest = new OpponentFinding
            {
                Id = 1
            };

            var opponentFindingRequestResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                                      It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(new List<OpponentFinding> { opponentFindingRequest });

            _mapperMock.Setup(x => x.Map<IEnumerable<OpponentFindingResponse>>(It.IsAny<IEnumerable<OpponentFindingRequest>>()))
                .Returns(new List<OpponentFindingResponse> { opponentFindingRequestResponse });

            //Act
            var result = await _opponentFindingService.CheckOpponentFindingAccepted(userId, date, startTime, endTime);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CheckOpponentFindingAccepted_WhenCalled_ReturnEmptyList()
        {
            //Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var startTime = 60 * 60 * 13;
            var endTime = 60 * 60 * 15;

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                                      It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((IEnumerable<OpponentFinding>)null);

            //Act
            var result = await _opponentFindingService.CheckOpponentFindingAccepted(userId, date, startTime, endTime);

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task CheckRequestWasAccepted_WhenCalled_ReturnOpponentFindingResponseWithUserRequesting()
        {
            //Arrange
            var userId = 1;
            var date = DateOnly.FromDateTime(DateTime.Now.AddDays(1));
            var startTime = 60 * 60 * 13;
            var endTime = 60 * 60 * 15;

            var opponentFindingRequest = new OpponentFindingRequest
            {
                Id = 1,
                OpponentFinding = new OpponentFinding
                {
                    Id = 1,
                    Status = OpponentFindingStatus.ACCEPTED
                }
            };

            var opponentFindingRequestResponse = new OpponentFindingResponseWithUserRequesting
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAllAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>(),
                                                                                      It.IsAny<Expression<Func<OpponentFindingRequest, object>>[]>()))
                           .ReturnsAsync(new List<OpponentFindingRequest> { opponentFindingRequest });

            _mapperMock.Setup(x => x.Map<OpponentFindingResponseWithUserRequesting>(It.IsAny<OpponentFindingRequest>()))
                .Returns(opponentFindingRequestResponse);

            //Act
            var result = await _opponentFindingService.CheckRequestWasAccepted(userId, date, startTime, endTime);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CanceledMatching_WhenOpponentFindingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((OpponentFinding)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CanceledMatching(opponentFindingId, userFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Opponent finding not found", exception.Message);
        }

        [Fact]
        public async Task CanceledMatching_WhenOpponentFindingIsOverdue_ThrowConflictException()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.ACCEPTED,
                IsOverdue = true
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CanceledMatching(opponentFindingId, userFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overdue", exception.Message);
        }

        //[Fact]
        //public async Task CanceledMatching_WhenSuccessfully_CancelOpponentFinding()
        //{
        //    //Arrange
        //    var opponentFindingId = 1;
        //    var userFindingId = 1;

        //    var opponentFinding = new OpponentFinding
        //    {
        //        Id = 1,
        //        Status = OpponentFindingStatus.ACCEPTED,
        //        IsOverdue = false
        //    };

        //    var opponentFindingRequest = new OpponentFindingRequest
        //    {
        //        Id = 1,
        //        OpponentFindingId = 1,
        //        IsAccepted = true
        //    };

        //    var opponentFindingResponse = new OpponentFindingResponse
        //    {
        //        Id = 1
        //    };

        //    _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
        //                                                                It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
        //                   .ReturnsAsync(opponentFinding);

        //    _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
        //                   .ReturnsAsync(opponentFindingRequest);

        //    _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.Update(It.IsAny<OpponentFindingRequest>()));

        //    _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.Update(It.IsAny<OpponentFinding>()));

        //    _unitOfWorkMock.Setup(x => x.CommitAsync())
        //                   .ReturnsAsync(1);

        //    _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
        //        .Returns(opponentFindingResponse);

        //    //Act
        //    var result = await _opponentFindingService.CanceledMatching(opponentFindingId, userFindingId);

        //    //Assert
        //    Assert.NotNull(result);
        //}

        [Fact]
        public async Task CanceledMatching_WhenCancelPostFail_ThrowConflictException()
        {
            //Arrange
            var opponentFindingId = 1;
            var userFindingId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                Status = OpponentFindingStatus.ACCEPTED,
                IsOverdue = false
            };

            var opponentFindingRequest = new OpponentFindingRequest
            {
                Id = 1,
                OpponentFindingId = 1,
                IsAccepted = true
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync(opponentFindingRequest);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.Update(It.IsAny<OpponentFindingRequest>()));

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.Update(It.IsAny<OpponentFinding>()));

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(0);

            _mapperMock.Setup(x => x.Map<OpponentFindingResponse>(It.IsAny<OpponentFinding>()))
                .Returns(opponentFindingResponse);

            //Act
            Func<Task> act = async () => await _opponentFindingService.CanceledMatching(opponentFindingId, userFindingId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Cancel opponent finding failed", exception.Message);
        }

        //public async Task<OpponentFindingResponse> RestoreFinding(int oldOpponentFindingId, int userRequestId)
        //{
        //    var opponentFinding = await _unitOfWork
        //        .OpponentFindingRepository
        //        .GetAsync(x => x.Id == oldOpponentFindingId
        //        && x.UserFindingId == userRequestId,
        //        x => x.UserFinding,
        //        x => x.Booking,
        //        x => x.Field,
        //        x => x.OpponentFindingRequests);
        //    if (opponentFinding == null)
        //    {
        //        throw new NotFoundException("Opponent finding not found");
        //    }

        //    if (opponentFinding.IsOverdue)
        //    {
        //        throw new ConflictException("Opponent finding is overdue");
        //    }

        //    if (opponentFinding.Status != OpponentFindingStatus.OPPONENT_CANCELLED)
        //    {
        //        throw new ConflictException("Can't restore opponent finding post because it is not cancelled");
        //    }

        //    opponentFinding.Status = OpponentFindingStatus.FINDING;
        //    _unitOfWork.OpponentFindingRepository.Update(opponentFinding);

        //    var tempList = await GetListRequestAvailableRestore(userRequestId, oldOpponentFindingId);
        //    if (tempList.Any())
        //    {
        //        foreach (var item in tempList)
        //        {
        //            item.Status = null;
        //            _unitOfWork.OpponentFindingRequestRepository.Update(item);
        //        }
        //    }

        //    await _unitOfWork.CommitAsync();

        //    return _mapper.Map<OpponentFindingResponse>(opponentFinding);
        //}
        //private async Task<IEnumerable<OpponentFindingRequest>> GetListRequestAvailableRestore(int userRequestId, int oldOpponentFindingId)
        //{
        //    var oldOpponentFinding = await _unitOfWork
        //        .OpponentFindingRepository
        //        .GetAsync(x => x.Id == oldOpponentFindingId,
        //                x => x.UserFinding,
        //                x => x.Booking,
        //                x => x.Field,
        //                x => x.OpponentFindingRequests);

        //    if (oldOpponentFinding == null)
        //    {
        //        throw new NotFoundException("Opponent finding not found");
        //    }

        //    if (oldOpponentFinding.IsOverdue)
        //    {
        //        throw new ConflictException("Opponent finding is overdue");
        //    }

        //    var date = oldOpponentFinding.Booking == null ? oldOpponentFinding.Date : oldOpponentFinding.Booking.Date;
        //    var startTime = oldOpponentFinding.Booking == null ? oldOpponentFinding.StartTime : oldOpponentFinding.Booking.StartTime;
        //    var endTime = oldOpponentFinding.Booking == null ? oldOpponentFinding.EndTime : oldOpponentFinding.Booking.EndTime;

        //    var previousCanceledPost = await RequestCancelOverlap(userRequestId, date.Value, startTime.Value, endTime.Value);
        //    previousCanceledPost = previousCanceledPost.Where(x => x.Id != oldOpponentFindingId
        //                                                        && x.OpponentFinding.Status == OpponentFindingStatus.FINDING);

        //    var tempList = previousCanceledPost.ToList();

        //    if (previousCanceledPost.Any())
        //    {
        //        for (int i = 0; i < tempList.Count(); i++)
        //        {
        //            var dateCheck = tempList[i].OpponentFinding.Booking == null ? tempList[i].OpponentFinding.Date : tempList[i].OpponentFinding.Booking.Date;
        //            var startTimeCheck = tempList[i].OpponentFinding.Booking == null ? tempList[i].OpponentFinding.StartTime : tempList[i].OpponentFinding.Booking.StartTime;
        //            var endTimeCheck = tempList[i].OpponentFinding.Booking == null ? tempList[i].OpponentFinding.EndTime : tempList[i].OpponentFinding.Booking.EndTime;

        //            var isOverlapWithAcceptedRequests = await RequestAcceptedOverlap(userRequestId, dateCheck.Value, startTimeCheck.Value, endTimeCheck.Value);
        //            var isOverlapWithOtherFinding = await OpponentAcceptFindingOverlap(userRequestId, dateCheck.Value, startTimeCheck.Value, endTimeCheck.Value);
        //            if (isOverlapWithAcceptedRequests.Any() || isOverlapWithOtherFinding.Any())
        //            {
        //                tempList.Remove(tempList[i]);
        //            }
        //        }
        //    }

        //    return tempList;
        //}

        [Fact]
        public async Task RestoreFinding_WhenOpponentFindingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var oldOpponentFindingId = 1;
            var userRequestId = 1;

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((OpponentFinding)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RestoreFinding(oldOpponentFindingId, userRequestId);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Opponent finding not found", exception.Message);
        }

        [Fact]
        public async Task RestoreFinding_WhenOpponentFindingIsOverdue_ThrowConflictException()
        {
            //Arrange
            var oldOpponentFindingId = 1;
            var userRequestId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                IsOverdue = true
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RestoreFinding(oldOpponentFindingId, userRequestId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overdue", exception.Message);
        }

        [Fact]
        public async Task RestoreFinding_WhenOpponentFindingIsNotCancelled_ThrowConflictException()
        {
            //Arrange
            var oldOpponentFindingId = 1;
            var userRequestId = 1;

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                IsOverdue = false,
                Status = OpponentFindingStatus.FINDING
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RestoreFinding(oldOpponentFindingId, userRequestId);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Can't restore opponent finding post because it is not cancelled", exception.Message);
        }

        [Fact]
        public async Task GetRepositoryPagination_WhenCalled_ReturnOpponentFindingResponse()
        {
            //Arrange
            var filterRequest = new OpponentFindingFilterRequest
            {
                FieldName = "test",
                Province = "test",
                District = "test",
                Commune = "test",
                Address = "test",
                FromTime = 60 * 60 * 13,
                ToTime = 60 * 60 * 15,
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                ToDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                Status = "test",
                Limit = 10,
                Offset = 0
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1
            };

            var opponentFindingResponse = new OpponentFindingResponse
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetListAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                              filterRequest.Limit,
                                                                              filterRequest.Offset,
                                                                              It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(new RepositoryPaginationResponse<OpponentFinding>
                           {
                               Total = 1,
                               Data = new List<OpponentFinding> { opponentFinding }
                           });

            _mapperMock.Setup(x => x.Map<IEnumerable<OpponentFindingResponse>>(It.IsAny<IEnumerable<OpponentFinding>>()))
                .Returns(new List<OpponentFindingResponse> { opponentFindingResponse });

            //Act
            var result = await _opponentFindingService.GetRepositoryPagination(filterRequest);

            //Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RequestOpponentFinding_WhenOpponentFindingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var userRequestId = 1;
            var request = new RequestingOpponentFindingRequest
            {
                OpponentFindingId = 1,
                Message = "test"
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync((OpponentFinding)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RequestOpponentFinding(userRequestId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("Opponent finding not found", exception.Message);
        }

        [Fact]
        public async Task RequestOpponentFinding_WhenOpponentFindingIsOverdue_ThrowConflictException()
        {
            //Arrange
            var userRequestId = 1;
            var request = new RequestingOpponentFindingRequest
            {
                OpponentFindingId = 1,
                Message = "test"
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                IsOverdue = true
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RequestOpponentFinding(userRequestId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Opponent finding is overdue", exception.Message);
        }

        [Fact]
        public async Task RequestOpponentFinding_WhenUserRequestingNotFound_ThrowNotFoundException()
        {
            //Arrange
            var userRequestId = 1;
            var request = new RequestingOpponentFindingRequest
            {
                OpponentFindingId = 1,
                Message = "test"
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                IsOverdue = false
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync((User)null);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RequestOpponentFinding(userRequestId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<NotFoundException>(act);
            Assert.Equal("User requesting not found", exception.Message);
        }

        [Fact]
        public async Task RequestOpponentFinding_WhenOpponentRequestExist_ThrowConflictException()
        {
            //Arrange
            var userRequestId = 1;
            var request = new RequestingOpponentFindingRequest
            {
                OpponentFindingId = 1,
                Message = "test"
            };

            var opponentFinding = new OpponentFinding
            {
                Id = 1,
                IsOverdue = false
            };

            var userRequesting = new User
            {
                Id = 1
            };

            var opponentFindingRequest = new OpponentFindingRequest
            {
                Id = 1
            };

            _unitOfWorkMock.Setup(x => x.OpponentFindingRepository.GetAsync(It.IsAny<Expression<Func<OpponentFinding, bool>>>(),
                                                                        It.IsAny<Expression<Func<OpponentFinding, object>>[]>()))
                           .ReturnsAsync(opponentFinding);

            _unitOfWorkMock.Setup(x => x.UserRepository.GetAsync(It.IsAny<Expression<Func<User, bool>>>()))
                           .ReturnsAsync(userRequesting);

            _unitOfWorkMock.Setup(x => x.OpponentFindingRequestRepository.GetAsync(It.IsAny<Expression<Func<OpponentFindingRequest, bool>>>()))
                           .ReturnsAsync(opponentFindingRequest);

            //Act
            Func<Task> act = async () => await _opponentFindingService.RequestOpponentFinding(userRequestId, request);

            //Assert
            var exception = await Assert.ThrowsAsync<ConflictException>(act);
            Assert.Equal("Request opponent finding already exist", exception.Message);
        }
    }
}