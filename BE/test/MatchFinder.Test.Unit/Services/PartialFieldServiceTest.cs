using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services.Impl;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace MatchFinder.Application.Services.Tests
{
    public class PartialFieldServiceTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IFileService> _fileServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly PartialFieldService _partialFieldService;

        public PartialFieldServiceTests()
        {
            _mapperMock = new Mock<IMapper>();
            _fileServiceMock = new Mock<IFileService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _partialFieldService = new PartialFieldService(_mapperMock.Object, _fileServiceMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ReturnsPartialFieldResponse()
        {
            // Arrange
            int id = 1;
            var mockPartialField = new PartialField { Id = id, Name = "Field 1", Description = "Description 1" };

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(), It.IsAny<Expression<Func<PartialField, object>>>()))
                           .ReturnsAsync(mockPartialField);

            _mapperMock.Setup(x => x.Map<PartialFieldResponse>(mockPartialField))
                       .Returns(new PartialFieldResponse { Id = id, Name = "Field 1", Description = "Description 1" });

            // Act
            var result = await _partialFieldService.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialFieldResponse>(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithInvalidId_ThrowsNotFoundException()
        {
            // Arrange
            int id = 1;

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(), It.IsAny<Expression<Func<PartialField, object>>>()))
                           .ReturnsAsync((PartialField)null);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _partialFieldService.GetByIdAsync(id));

            // Assert
            Assert.Equal("Partial field not found", exception.Message);
        }

        [Fact]
        public async Task GetListAsync_WithFilterRequest_ReturnsRepositoryPaginationResponse()
        {
            // Arrange
            var filterRequest = new PartialFieldFilterRequest
            {
                FieldName = "Field 1",
                PartialFieldName = "Partial Field 1",
                Status = "ACTIVE",
                FromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)), // Ensure FromDate is before ToDate
                ToDate = DateOnly.FromDateTime(DateTime.Now),
                Limit = 10,
                Offset = 0
            };

            var mockPartialFields = new RepositoryPaginationResponse<PartialField>
            {
                Data = new List<PartialField> { new PartialField { Id = 1, Name = "Partial Field 1", Status = "ACTIVE" } },
                Total = 1
            };

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetListAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(), filterRequest.Limit, filterRequest.Offset, It.IsAny<Expression<Func<PartialField, object>>>()))
                           .ReturnsAsync(mockPartialFields);

            _mapperMock.Setup(x => x.Map<IEnumerable<PartialFieldResponse>>(mockPartialFields.Data))
                       .Returns(new List<PartialFieldResponse> { new PartialFieldResponse { Id = 1, Name = "Partial Field 1", Status = "ACTIVE" } });

            // Act
            var result = await _partialFieldService.GetListAsync(filterRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<RepositoryPaginationResponse<PartialFieldResponse>>(result);
            Assert.Equal(1, result.Total);
        }

        [Fact]
        public async Task GetListByFieldAsync_WithFieldId_ReturnsPartialFieldWithNumberBookingsResponses()
        {
            // Arrange
            int fieldId = 1;
            var mockPartialFields = new List<PartialField>
            {
                new PartialField
                {
                    Id = 1,
                    Name = "Partial Field 1",
                    Status = "ACTIVE",
                    FieldId = fieldId,
                    Field = new Field { Name = "Field 1" },
                    Bookings = new List<Booking>
                    {
                        new Booking { Status = "WAITING" },
                        new Booking { Status = "ACCEPTED" },
                        new Booking { Status = "REJECTED" },
                        new Booking { Status = "CANCELED" }
                    }
                }
            };

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAllAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(),
                It.IsAny<Expression<Func<PartialField, object>>>(),
                It.IsAny<Expression<Func<PartialField, object>>>()))
                .ReturnsAsync(mockPartialFields);

            var expectedResponse = new List<PartialFieldWithNumberBookingsResponse>
            {
                new PartialFieldWithNumberBookingsResponse
                {
                    Id = 1,
                    Name = "Partial Field 1",
                    Status = "ACTIVE",
                    FieldId = fieldId,
                    FieldName = "Field 1",
                    NumberWaiting = 1,
                    NumberAccepted = 1,
                    NumberRejected = 1,
                    NumberCanceled = 1
                }
            };

            _mapperMock.Setup(x => x.Map<IEnumerable<PartialFieldWithNumberBookingsResponse>>(mockPartialFields))
                .Returns(expectedResponse);

            // Act
            var result = await _partialFieldService.GetListByFieldAsync(fieldId);

            // Assert
            Assert.NotNull(result);
            Assert.IsAssignableFrom<IEnumerable<PartialFieldWithNumberBookingsResponse>>(result);
            Assert.Single(result);

            var firstResult = result.First();
            Assert.Equal(1, firstResult.Id);
            Assert.Equal("Partial Field 1", firstResult.Name);
            Assert.Equal("ACTIVE", firstResult.Status);
            Assert.Equal(fieldId, firstResult.FieldId);
            Assert.Equal("Field 1", firstResult.FieldName);
            Assert.Equal(1, firstResult.NumberWaiting);
            Assert.Equal(1, firstResult.NumberAccepted);
            Assert.Equal(1, firstResult.NumberRejected);
            Assert.Equal(1, firstResult.NumberCanceled);

            _unitOfWorkMock.Verify(x => x.PartialFieldRepository.GetAllAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(),
                It.IsAny<Expression<Func<PartialField, object>>>(),
                It.IsAny<Expression<Func<PartialField, object>>>()), Times.Once);

            _mapperMock.Verify(x => x.Map<IEnumerable<PartialFieldWithNumberBookingsResponse>>(mockPartialFields), Times.Once);
        }

        //[Fact]
        //public async Task CreatePartialFieldAsync_WithValidRequest_ReturnsPartialFieldResponse()
        //{
        //    // Arrange
        //    var request = new PartialFieldCreateRequest
        //    {
        //        FieldId = 1,
        //        Name = "New Partial Field",
        //        Description = "New Description",
        //        Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg"),
        //        Image_2 = new FormFile(new MemoryStream(), 0, 0, "Image_2", "Image_2.jpg"),
        //        Amount = 100,
        //        Deposit = 50
        //    };

        //    var mockField = new Field { Id = 1, Name = "Field 1" };
        //    var newPartialField = new PartialField
        //    {
        //        Id = 1,
        //        Name = request.Name,
        //        Description = request.Description,
        //        Image1 = "urlImage1.jpg",
        //        Image2 = "urlImage2.jpg",
        //        FieldId = request.FieldId,
        //        Status = PartialFieldStatus.ACTIVE,
        //        Amount = request.Amount,
        //        Deposit = request.Deposit,
        //    };

        //    _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
        //                   .ReturnsAsync(mockField);

        //    _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
        //                    .Returns(true);

        //    _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
        //                    .ReturnsAsync((IFormFile file) => file.FileName);

        //    _unitOfWorkMock.Setup(x => x.PartialFieldRepository.AddAsync(It.IsAny<PartialField>()))
        //                   .Callback<PartialField>(pf => pf.Id = newPartialField.Id); // Simulate setting ID on add

        //    _unitOfWorkMock.Setup(x => x.CommitAsync())
        //                   .ReturnsAsync(1); // Simulate successful commit

        //    _mapperMock.Setup(x => x.Map<PartialFieldResponse>(It.IsAny<PartialField>()))
        //               .Returns(new PartialFieldResponse
        //               {
        //                   Id = newPartialField.Id,
        //                   Name = newPartialField.Name,
        //                   Description = newPartialField.Description,
        //                   Image1 = newPartialField.Image1,
        //                   Image2 = newPartialField.Image2,
        //                   FieldId = newPartialField.FieldId,
        //                   Status = newPartialField.Status.ToString(),
        //                   Amount = newPartialField.Amount,
        //               });

        //    // Act
        //    var result = await _partialFieldService.CreatePartialFieldAsync(request);

        //    // Assert
        //    Assert.NotNull(result);
        //    Assert.IsType<PartialFieldResponse>(result);
        //}

        //[Fact]
        //public async Task CreatePartialFieldAsync_WithInvalidFieldId_ThrowsNotFoundException()
        //{
        //    // Arrange
        //    var request = new PartialFieldCreateRequest { FieldId = 1 };

        //    _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
        //                   .ReturnsAsync((Field)null);

        //    // Act
        //    var exception = await Assert.ThrowsAsync<NotFoundException>(() => _partialFieldService.CreatePartialFieldAsync(request));

        //    // Assert
        //    Assert.Equal("Field not found", exception.Message);
        //}

        //[Fact]
        //public async Task CreatePartialFieldAsync_WithInvalidImageFile_ThrowsInvalidOperationException()
        //{
        //    // Arrange
        //    var request = new PartialFieldCreateRequest
        //    {
        //        FieldId = 1,
        //        Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.txt")
        //    };

        //    var mockField = new Field { Id = 1, Name = "Field 1" };

        //    _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
        //                   .ReturnsAsync(mockField);

        //    _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
        //                    .Throws(new DataInvalidException("Image_1.txt is not an image"));

        //    // Act
        //    var exception = await Assert.ThrowsAsync<DataInvalidException>(() => _partialFieldService.CreatePartialFieldAsync(request));

        //    // Assert
        //    Assert.Equal("Image_1.txt is not an image", exception.Message);
        //}

        //[Fact]
        //public async Task CreatePartialFieldAsync_WithImageSaveFailure_ThrowsException()
        //{
        //    // Arrange
        //    var request = new PartialFieldCreateRequest
        //    {
        //        FieldId = 1,
        //        Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg")
        //    };

        //    var mockField = new Field { Id = 1, Name = "Field 1" };

        //    _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
        //                   .ReturnsAsync(mockField);

        //    _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
        //                    .Returns(true);

        //    _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
        //                    .ThrowsAsync(new Exception("File save error"));

        //    // Act
        //    var exception = await Assert.ThrowsAsync<Exception>(() => _partialFieldService.CreatePartialFieldAsync(request));

        //    // Assert
        //    Assert.Equal("File save error", exception.Message);
        //}

        //[Fact]
        //public async Task CreatePartialFieldAsync_WithUnsuccessfulCommit_ThrowsConflictException()
        //{
        //    // Arrange
        //    var request = new PartialFieldCreateRequest
        //    {
        //        FieldId = 1,
        //        Name = "New Partial Field",
        //        Description = "New Description",
        //        Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg"),
        //        Image_2 = new FormFile(new MemoryStream(), 0, 0, "Image_2", "Image_2.jpg"),
        //        Amount = 100,
        //        Deposit = 50
        //    };

        //    var mockField = new Field { Id = 1, Name = "Field 1" };
        //    var newPartialField = new PartialField
        //    {
        //        Id = 1,
        //        Name = request.Name,
        //        Description = request.Description,
        //        Image1 = "https://blobstorage.com/Image_1.jpg",
        //        Image2 = "https://blobstorage.com/Image_2.jpg",
        //        FieldId = request.FieldId,
        //        Status = PartialFieldStatus.ACTIVE,
        //        Amount = request.Amount,
        //        Deposit = request.Deposit,
        //    };

        //    _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
        //                   .ReturnsAsync(mockField);

        //    _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
        //                    .Returns(true);

        //    _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
        //                    .ReturnsAsync((IFormFile file) => "https://blobstorage.com/" + file.FileName);

        //    _unitOfWorkMock.Setup(x => x.PartialFieldRepository.AddAsync(It.IsAny<PartialField>()))
        //                   .Callback<PartialField>(pf => pf.Id = newPartialField.Id); // Simulate setting ID on add

        //    _unitOfWorkMock.Setup(x => x.CommitAsync())
        //                   .ReturnsAsync(0); // Simulate unsuccessful commit

        //    // Act
        //    var exception = await Assert.ThrowsAsync<ConflictException>(() => _partialFieldService.CreatePartialFieldAsync(request));

        //    // Assert
        //    Assert.Equal("Create partial field fail", exception.Message);
        //}

        [Fact]
        public async Task UpdatePartialFieldAsync_WithValidRequest_ReturnsPartialFieldResponse()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest
            {
                FieldId = 1,
                Name = "Updated Partial Field",
                Description = "Updated Description",
                Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg"),
                Image_2 = new FormFile(new MemoryStream(), 0, 0, "Image_2", "Image_2.jpg"),
                Status = PartialFieldStatus.ACTIVE
            };

            var mockField = new Field { Id = 1, Name = "Field 1" };
            var mockPartialField = new PartialField
            {
                Id = id,
                Name = "Partial Field 1",
                Description = "Description 1",
                Image1 = "urlImage1.jpg",
                Image2 = "urlImage2.jpg",
                FieldId = 1,
                Status = PartialFieldStatus.ACTIVE
            };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(mockField);

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync(mockPartialField);

            _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
                            .Returns(true);

            _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                            .ReturnsAsync((IFormFile file) => "https://blobstorage.com/" + file.FileName);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(1); // Simulate successful commit

            _mapperMock.Setup(x => x.Map<PartialFieldResponse>(It.IsAny<PartialField>()))
                       .Returns(new PartialFieldResponse
                       {
                           Id = id,
                           Name = request.Name,
                           Description = request.Description,
                           Image1 = "https://blobstorage.com/Image_1.jpg",
                           Image2 = "https://blobstorage.com/Image_2.jpg",
                           FieldId = 1,
                           Status = PartialFieldStatus.ACTIVE.ToString()
                       });

            // Act
            var result = await _partialFieldService.UpdatePartialFieldAsync(id, request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialFieldResponse>(result);
            Assert.Equal(id, result.Id);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithInvalidFieldId_ThrowsNotFoundException()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest { FieldId = 1 };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync((Field)null);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _partialFieldService.UpdatePartialFieldAsync(id, request));

            // Assert
            Assert.Equal("Field not found", exception.Message);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithInvalidPartialFieldId_ThrowsNotFoundException()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest { FieldId = 1 };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(new Field { Id = 1, Name = "Field 1" });

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync((PartialField)null);

            // Act
            var exception = await Assert.ThrowsAsync<NotFoundException>(() => _partialFieldService.UpdatePartialFieldAsync(id, request));

            // Assert
            Assert.Equal("Partial field not found", exception.Message);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithInvalidImageFile_ThrowsDataInvalidException()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest
            {
                FieldId = 1,
                Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.txt")
            };

            var mockField = new Field { Id = 1, Name = "Field 1" };
            var mockPartialField = new PartialField { Id = id, FieldId = 1 };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(mockField);

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync(mockPartialField);

            _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
                            .Throws(new DataInvalidException("Image_1.txt is not an image"));

            // Act
            var exception = await Assert.ThrowsAsync<DataInvalidException>(() => _partialFieldService.UpdatePartialFieldAsync(id, request));

            // Assert
            Assert.Equal("Image_1.txt is not an image", exception.Message);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithImageSaveFailure_ThrowsException()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest
            {
                FieldId = 1,
                Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg")
            };

            var mockField = new Field { Id = 1, Name = "Field 1" };
            var mockPartialField = new PartialField { Id = id, FieldId = 1 };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(mockField);

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync(mockPartialField);

            _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
                            .Returns(true);

            _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                            .ThrowsAsync(new Exception("File save error"));

            // Act
            var exception = await Assert.ThrowsAsync<Exception>(() => _partialFieldService.UpdatePartialFieldAsync(id, request));

            // Assert
            Assert.Equal("File save error", exception.Message);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithUnsuccessfulCommit_ThrowsConflictException()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest
            {
                FieldId = 1,
                Name = "Updated Partial Field",
                Description = "Updated Description",
                Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg"),
                Image_2 = new FormFile(new MemoryStream(), 0, 0, "Image_2", "Image_2.jpg"),
                Status = PartialFieldStatus.ACTIVE
            };

            var mockField = new Field { Id = 1, Name = "Field 1" };
            var mockPartialField = new PartialField
            {
                Id = id,
                Name = "Partial Field 1",
                Description = "Description 1",
                Image1 = "urlImage1.jpg",
                Image2 = "urlImage2.jpg",
                FieldId = 1,
                Status = PartialFieldStatus.ACTIVE
            };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(mockField);

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync(mockPartialField);

            _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
                            .Returns(true);

            _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                            .ReturnsAsync((IFormFile file) => "https://blobstorage.com/" + file.FileName);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(0); // Simulate unsuccessful commit

            // Act
            var exception = await Assert.ThrowsAsync<ConflictException>(() => _partialFieldService.UpdatePartialFieldAsync(id, request));

            // Assert
            Assert.Equal("Update partial field fail", exception.Message);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithSomePropertiesOmitted_OnlyUpdatesProvidedFields()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest
            {
                Name = "Updated Name",
                Image_1 = new FormFile(new MemoryStream(), 0, 0, "Image_1", "Image_1.jpg")
            };

            var mockField = new Field { Id = 1, Name = "Field 1" };
            var mockPartialField = new PartialField
            {
                Id = id,
                Name = "Old Name",
                Description = "Old Description",
                Image1 = "oldImage1.jpg",
                Image2 = "oldImage2.jpg",
                FieldId = 1,
                Status = PartialFieldStatus.ACTIVE
            };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(mockField);

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync(mockPartialField);

            _fileServiceMock.Setup(x => x.IsImageFile(It.IsAny<IFormFile>()))
                            .Returns(true);

            _fileServiceMock.Setup(x => x.SaveFileAsync(It.IsAny<IFormFile>()))
                            .ReturnsAsync((IFormFile file) => "https://blobstorage.com/" + file.FileName);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(1); // Simulate successful commit

            _mapperMock.Setup(x => x.Map<PartialFieldResponse>(It.IsAny<PartialField>()))
                       .Returns(new PartialFieldResponse
                       {
                           Id = id,
                           Name = request.Name,
                           Description = mockPartialField.Description,
                           Image1 = "https://blobstorage.com/Image_1.jpg",
                           Image2 = mockPartialField.Image2,
                           FieldId = mockPartialField.FieldId,
                           Status = mockPartialField.Status.ToString()
                       });

            // Act
            var result = await _partialFieldService.UpdatePartialFieldAsync(id, request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialFieldResponse>(result);
            Assert.Equal(request.Name, result.Name);
            Assert.Equal(mockPartialField.Description, result.Description);
            Assert.Equal("https://blobstorage.com/Image_1.jpg", result.Image1);
            Assert.Equal(mockPartialField.Image2, result.Image2);
            Assert.Equal(mockPartialField.FieldId, result.FieldId);
            Assert.Equal(mockPartialField.Status.ToString(), result.Status);
        }

        [Fact]
        public async Task UpdatePartialFieldAsync_WithNoPropertiesProvided_DoesNotChangePartialField()
        {
            // Arrange
            int id = 1;
            var request = new PartialFieldUpdateRequest();

            var mockField = new Field { Id = 1, Name = "Field 1" };
            var mockPartialField = new PartialField
            {
                Id = id,
                Name = "Original Name",
                Description = "Original Description",
                Image1 = "originalImage1.jpg",
                Image2 = "originalImage2.jpg",
                FieldId = 1,
                Status = PartialFieldStatus.ACTIVE
            };

            _unitOfWorkMock.Setup(x => x.FieldRepository.GetAsync(It.IsAny<Expression<Func<Field, bool>>>()))
                           .ReturnsAsync(mockField);

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(It.IsAny<Expression<Func<PartialField, bool>>>()))
                           .ReturnsAsync(mockPartialField);

            _unitOfWorkMock.Setup(x => x.CommitAsync())
                           .ReturnsAsync(1); // Simulate successful commit

            _mapperMock.Setup(x => x.Map<PartialFieldResponse>(It.IsAny<PartialField>()))
                       .Returns(new PartialFieldResponse
                       {
                           Id = id,
                           Name = mockPartialField.Name,
                           Description = mockPartialField.Description,
                           Image1 = mockPartialField.Image1,
                           Image2 = mockPartialField.Image2,
                           FieldId = mockPartialField.FieldId,
                           Status = mockPartialField.Status.ToString()
                       });

            // Act
            var result = await _partialFieldService.UpdatePartialFieldAsync(id, request);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<PartialFieldResponse>(result);
            Assert.Equal(mockPartialField.Name, result.Name);
            Assert.Equal(mockPartialField.Description, result.Description);
            Assert.Equal(mockPartialField.Image1, result.Image1);
            Assert.Equal(mockPartialField.Image2, result.Image2);
            Assert.Equal(mockPartialField.FieldId, result.FieldId);
            Assert.Equal(mockPartialField.Status.ToString(), result.Status);
        }

        [Fact]
        public async Task GetOwnerIdAsync_WithValidPartialFieldId_ReturnsOwnerId()
        {
            // Arrange
            int partialFieldId = 1;
            var mockPartialField = new PartialField { Id = partialFieldId, Field = new Field { OwnerId = 1 } };

            _unitOfWorkMock.Setup(x => x.PartialFieldRepository.GetAsync(
                It.IsAny<Expression<Func<PartialField, bool>>>(), It.IsAny<Expression<Func<PartialField, object>>>()))
                           .ReturnsAsync(mockPartialField);

            // Act
            var result = await _partialFieldService.GetOwnerIdAsync(partialFieldId);

            // Assert
            Assert.Equal(1, result);
        }
    }
}