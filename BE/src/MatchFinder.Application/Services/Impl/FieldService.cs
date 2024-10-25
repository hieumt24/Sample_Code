using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.DataAccess;
using MatchFinder.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;

namespace MatchFinder.Application.Services.Impl
{
    public class FieldService : IFieldService
    {
        private IMapper _mapper;
        private IFileService _fileService;
        private IBookingService _bookingService;
        private IEmailService _emailService;
        private IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private MatchFinderContext _context;

        public FieldService(IMapper mapper, IFileService fileService, IBookingService bookingService, IEmailService emailService, IUnitOfWork unitOfWork, MatchFinderContext context, INotificationService notificationService)
        {
            _mapper = mapper;
            _fileService = fileService;
            _bookingService = bookingService;
            _emailService = emailService;
            _unitOfWork = unitOfWork;
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<FieldResponse> GetByIdAsync(int id)
        {
            var field = await _unitOfWork.FieldRepository.GetLoadingAsync(
                x => x.Id == id,
                f => f.Include(pf => pf.PartialFields)
                        .ThenInclude(b => b.Bookings
                            .Where(b =>
                                    EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                f => f.Include(r => r.Rates),
                f => f.Include(o => o.Owner)
            );

            if (field == null)
            {
                throw new NotFoundException("Field not found!");
            }
            return _mapper.Map<FieldResponse>(field);
        }

        public async Task<object> CountAllRateAsync(int fid)
        {
            var rates = await _unitOfWork.RateRepository.GetAllAsync(x => x.FieldId == fid);
            var result = new
            {
                average = rates.Any() ? rates.Average(r => r.Star) : 0,
                totalRate = rates.Count(),
                star1 = rates.Count(r => r.Star == 1),
                star2 = rates.Count(r => r.Star == 2),
                star3 = rates.Count(r => r.Star == 3),
                star4 = rates.Count(r => r.Star == 4),
                star5 = rates.Count(r => r.Star == 5)
            };
            return result;
        }

        public async Task<FieldResponse> CreateFieldAsync(FieldCreateRequest request, int id)
        {
            var cover = string.Empty;
            if (request.Cover != null && _fileService.IsImageFile(request.Cover))
                cover = await _fileService.SaveFileAsync(request.Cover);
            var avt = string.Empty;
            if (request.Avatar != null && _fileService.IsImageFile(request.Avatar))
                avt = await _fileService.SaveFileAsync(request.Avatar);

            var owner = await _unitOfWork.UserRepository.GetAsync(x => x.Id == id);
            if (owner == null)
            {
                throw new ConflictException("Account invalid!");
            }

            var newField = new Field
            {
                Name = request.Name,
                Address = request.Address,
                Commune = request.Commune,
                District = request.District,
                Province = request.District,
                PhoneNumber = request.PhoneNumber,
                Latitude = Math.Round(request.Latitude, 9),
                Longitude = Math.Round(request.Longitude, 9),
                Description = request.Description,
                Avatar = avt,
                Cover = cover,
                OpenTime = request.OpenTime,
                CloseTime = request.CloseTime,
                IsFixedSlot = request.IsFixedSlot,
                Status = FieldStatus.WAITING,
                Owner = owner,
                Price = request.Price,
                Deposit = request.Deposit
            };

            await _unitOfWork.FieldRepository.AddAsync(newField);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                await IndexFootballFieldAsync(newField);
                var admins = await _unitOfWork.UserRepository.GetAllAsync(u => u.Role.Name == "Admin" && u.Status == Domain.Enums.UserStatus.ACTIVE);
                var receiverIds = admins.Select(a => a.Id).ToList();

                var notification = new Notification()
                {
                    Title = "Sân mới đã được tạo",
                    Content = $"{owner.UserName} đã tạo sân mới: \"{newField.Name}\"!"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return _mapper.Map<FieldResponse>(newField);
            }
            throw new ConflictException("Create field fail");
        }

        public async Task IndexFootballFieldAsync(Field field)
        {
            var tokens = new List<(string Token, int FieldId)>();

            tokens.AddRange(Tokenize(field.Name).Select(t => (t, 1)));
            tokens.AddRange(Tokenize(field.Address).Select(t => (t, 2)));
            tokens.AddRange(Tokenize(field.PhoneNumber).Select(t => (t, 3)));

            foreach (var (token, fieldId) in tokens)
            {
                _context.InvertedIndexes.Add(new InvertedIndex
                {
                    Token = token,
                    FieldId = fieldId,
                    RecordId = field.Id
                });
            }

            await _context.SaveChangesAsync();
        }

        private IEnumerable<string> Tokenize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return Enumerable.Empty<string>();

            string normalized = input.Normalize(NormalizationForm.FormD);
            normalized = new string(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray());

            return normalized.ToLower().Split(new[] { ' ', ',', '.', '-' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public async Task RemoveIndexForFieldAsync(int fieldId)
        {
            var indexEntries = await _context.InvertedIndexes
                .Where(i => i.RecordId == fieldId)
                .ToListAsync();

            _context.InvertedIndexes.RemoveRange(indexEntries);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<FieldResponse>> SearchAsync(string keyword)
        {
            var tokens = Tokenize(keyword);

            var fieldsQuery = from idx in _context.InvertedIndexes
                              join field in _context.Fields.Include(f => f.Rates) on idx.RecordId equals field.Id
                              join owner in _context.Users on field.OwnerId equals owner.Id
                              where tokens.Any(t => idx.Token.Contains(t))
                              && field.Status.Equals(FieldStatus.ACCEPTED)
                              && field.IsDeleted == false
                              select new
                              {
                                  Field = field,
                                  OwnerName = owner.UserName
                              };

            var fields = await fieldsQuery.Distinct().ToListAsync();

            var fieldResponses = fields.Select(f => new FieldResponse
            {
                Id = f.Field.Id,
                Name = f.Field.Name,
                Address = f.Field.Address,
                Province = f.Field.Province,
                District = f.Field.District,
                Commune = f.Field.Commune,
                PhoneNumber = f.Field.PhoneNumber,
                Latitude = f.Field.Latitude,
                Longitude = f.Field.Longitude,
                Status = f.Field.Status,
                OpenTime = ConvertSecondsToHourMinString(f.Field.OpenTime),
                CloseTime = ConvertSecondsToHourMinString(f.Field.CloseTime),
                Description = f.Field.Description,
                Avatar = f.Field.Avatar,
                Cover = f.Field.Cover,
                Rating = f.Field.Rates != null && f.Field.Rates.Any() ? f.Field.Rates.Average(r => r.Star).ToString("0.0") : "N/A",
                Price = f.Field.Price,
                Deposit = f.Field.Deposit,
                NumberOfBookings = f.Field.PartialFields != null ? f.Field.PartialFields.Sum(pf => pf.Bookings?.Count ?? 0) : 0,
                IsFixedSlot = f.Field.IsFixedSlot,
                OwnerId = f.Field.OwnerId,
                OwnerName = f.OwnerName
            }).ToList();

            return fieldResponses;
        }

        private string ConvertSecondsToTimeString(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return time.ToString(@"hh\:mm\:ss");
        }

        private string ConvertSecondsToHourMinString(int totalSeconds)
        {
            int hours = totalSeconds / 3600;
            int minutes = (totalSeconds % 3600) / 60;
            return $"{hours:D2}:{minutes:D2}";
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetMyFieldsAsync(int ownerId, GetFieldsRequest request)
        {
            var fields = await _unitOfWork.FieldRepository
                                            .GetLoadingListAsync(x =>
                                                    (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)) &&
                                                    (string.IsNullOrEmpty(request.Address) || x.Address.Contains(request.Address)) &&
                                                    (string.IsNullOrEmpty(request.Province) || x.Province.Contains(request.Province)) &&
                                                    (string.IsNullOrEmpty(request.Commune) || x.Commune.Contains(request.Commune)) &&
                                                    (string.IsNullOrEmpty(request.Status) || x.Status.Equals(request.Status)) &&
                                                    (string.IsNullOrEmpty(request.District) || x.District.Contains(request.District)) &&
                                                    (request.FromPrice == null || x.Price >= request.FromPrice) &&
                                                    (request.ToPrice == null || x.Price <= request.ToPrice) &&
                                                    (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                                    (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)) &&
                                                    x.OwnerId.Equals(ownerId),
                                                    request.Limit, request.Offset,
                                                    f => f.Include(pf => pf.PartialFields)
                                                            .ThenInclude(b => b.Bookings
                                                                .Where(b =>
                                                                        EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                    f => f.Include(r => r.Rates),
                                                    f => f.Include(o => o.Owner));

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(fields.Data),
                Total = fields.Total
            };
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetStaffFieldsAsync(int sid, GetFieldsRequest request)
        {
            var fields = await _unitOfWork.FieldRepository
                                            .GetLoadingListAsync(x =>
                                                    x.Staffs.Any(s => s.UserId == sid && s.IsActive == true) &&
                                                    (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)) &&
                                                    (string.IsNullOrEmpty(request.Address) || x.Address.Contains(request.Address)) &&
                                                    (string.IsNullOrEmpty(request.Province) || x.Province.Contains(request.Province)) &&
                                                    (string.IsNullOrEmpty(request.Commune) || x.Commune.Contains(request.Commune)) &&
                                                    (string.IsNullOrEmpty(request.Status) || x.Status.Equals(request.Status)) &&
                                                    (string.IsNullOrEmpty(request.District) || x.District.Contains(request.District)) &&
                                                    (request.FromPrice == null || x.Price >= request.FromPrice) &&
                                                    (request.ToPrice == null || x.Price <= request.ToPrice) &&
                                                    (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                                    (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)),
                                                    request.Limit, request.Offset,
                                                    f => f.Include(pf => pf.PartialFields)
                                                            .ThenInclude(b => b.Bookings
                                                                .Where(b =>
                                                                        EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                    f => f.Include(r => r.Rates),
                                                    f => f.Include(o => o.Owner));

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(fields.Data),
                Total = fields.Total
            };
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetAllFieldsAsync(GetFieldsRequest request)
        {
            var fields = await _unitOfWork.FieldRepository
                                            .GetLoadingListAsync(x =>
                                                    (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)) &&
                                                    (string.IsNullOrEmpty(request.Address) || x.Address.Contains(request.Address)) &&
                                                    (string.IsNullOrEmpty(request.Province) || x.Province.Contains(request.Province)) &&
                                                    (string.IsNullOrEmpty(request.Commune) || x.Commune.Contains(request.Commune)) &&
                                                    (string.IsNullOrEmpty(request.District) || x.District.Contains(request.District)) &&
                                                    (request.FromPrice == null || x.Price >= request.FromPrice) &&
                                                    (request.ToPrice == null || x.Price <= request.ToPrice) &&
                                                    (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                                    (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)) &&
                                                    (request.Status == null || x.Status.Equals(request.Status)),
                                                    request.Limit, request.Offset,
                                                    f => f.Include(pf => pf.PartialFields)
                                                            .ThenInclude(b => b.Bookings
                                                                .Where(b =>
                                                                        EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                    f => f.Include(r => r.Rates),
                                                    f => f.Include(o => o.Owner));

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(fields.Data),
                Total = fields.Total
            };
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsAsync(GetFieldsRequest request)
        {
            var fields = await _unitOfWork.FieldRepository
                                            .GetLoadingListAsync(x =>
                                                    (string.IsNullOrEmpty(request.Name) || x.Name.Contains(request.Name)) &&
                                                    (string.IsNullOrEmpty(request.Address) || x.Address.Contains(request.Address)) &&
                                                    (string.IsNullOrEmpty(request.Province) || x.Province.Contains(request.Province)) &&
                                                    (string.IsNullOrEmpty(request.Commune) || x.Commune.Contains(request.Commune)) &&
                                                    (string.IsNullOrEmpty(request.District) || x.District.Contains(request.District)) &&
                                                    (request.FromPrice == null || x.Price >= request.FromPrice) &&
                                                    (request.ToPrice == null || x.Price <= request.ToPrice) &&
                                                    (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                                    (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)) &&
                                                    x.Status.ToLower().Equals(FieldStatus.ACCEPTED),
                                                    request.Limit, request.Offset,
                                                    f => f.Include(pf => pf.PartialFields)
                                                            .ThenInclude(b => b.Bookings
                                                                .Where(b =>
                                                                        EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                    f => f.Include(r => r.Rates),
                                                    f => f.Include(o => o.Owner));

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(fields.Data),
                Total = fields.Total
            };
        }

        public async Task<object> SearchOptionAsync(SearchOptionRequest request)
        {
            int startTime = (int)request.StartDateTime.TimeOfDay.TotalSeconds;
            int endTime = startTime + request.Duration;

            Expression<Func<Field, bool>> baseExpression = f => ((!f.IsFixedSlot)
                                                                    || (f.IsFixedSlot && f.Slots.Any(s => s.StartTime == startTime && s.EndTime == endTime)))
                                                                && (f.OpenTime <= startTime && f.CloseTime >= endTime)
                                                                && (!f.InactiveTimes.Any(it =>
                                                                    it.StartTime <= request.StartDateTime && it.EndTime >= request.StartDateTime.AddSeconds(request.Duration)))
                                                                && (f.PartialFields.Any(pf =>
                                                                    !pf.Bookings.Any(b =>
                                                                        b.Status != BookingStatus.REJECTED &&
                                                                        b.Status != BookingStatus.CANCELED &&
                                                                        (b.IsDeleted == null || b.IsDeleted == false) &&
                                                                        b.Date == DateOnly.FromDateTime(request.StartDateTime)
                                                                        &&
                                                                        ((b.StartTime <= startTime && b.EndTime > startTime) ||
                                                                         (b.StartTime < endTime && b.EndTime >= endTime) ||
                                                                         (b.StartTime >= startTime && b.EndTime <= endTime))))
                                                                );

            if (!string.IsNullOrEmpty(request.Province))
            {
                baseExpression = baseExpression.And(f => f.Province == request.Province);
            }
            if (!string.IsNullOrEmpty(request.District))
            {
                baseExpression = baseExpression.And(f => f.District == request.District);
            }
            if (!string.IsNullOrEmpty(request.Commune))
            {
                baseExpression = baseExpression.And(f => f.Commune == request.Commune);
            }
            if (request.FromPrice != null)
            {
                baseExpression = baseExpression.And(x => x.Price >= request.FromPrice);
            }
            if (request.ToPrice != null)
            {
                baseExpression = baseExpression.And(x => x.Price <= request.ToPrice);
            }
            if (request.FromStar != null || request.ToStar != null)
            {
                baseExpression = baseExpression.And(x =>
                                                    (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                                    (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)));
            }

            IEnumerable<Field> allFields = await _unitOfWork.FieldRepository.GetQueryable(baseExpression)
                                                        .Include(pf => pf.PartialFields)
                                                        .Include(r => r.Rates)
                                                        .Include(o => o.Owner)
                                                        .Include(it => it.InactiveTimes)
                                                        .Include(s => s.Slots)
                                                        .ToListAsync();
            var result = new
            {
                All =
                new RepositoryPaginationResponse<FieldResponse>
                {
                    Data = _mapper.Map<IEnumerable<FieldResponse>>(allFields),
                    Total = allFields.Count()
                }
            };

            if (request.Latitude.HasValue && request.Longitude.HasValue && request.Radius.HasValue)
            {
                var nearFields = allFields.Where(f =>
                    CalculateHaversineDistance(f.Latitude, f.Longitude, request.Latitude.Value, request.Longitude.Value) <= request.Radius.Value
                );

                return new
                {
                    All = result.All,
                    NearMe = new RepositoryPaginationResponse<FieldResponse>
                    {
                        Data = _mapper.Map<IEnumerable<FieldResponse>>(nearFields),
                        Total = nearFields.Count()
                    }
                };
            }

            return result;
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsEarlyAsync(int uid, GetFieldsEarlyRequest request)
        {
            var fields = await _unitOfWork.FieldRepository
                .GetLoadingListAsync(
                    x => EF.Functions.Like(x.Status, FieldStatus.ACCEPTED) &&
                    (request.FromPrice == null || x.Price >= request.FromPrice) &&
                    (request.ToPrice == null || x.Price <= request.ToPrice) &&
                    (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                    (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)) &&
                    x.PartialFields.SelectMany(pf => pf.Bookings).Any(b => b.UserId == uid),
                    request.Limit,
                    request.Offset,
                    x => x.Id,
                    true,
                    f => f.Include(pf => pf.PartialFields)
                            .ThenInclude(b => b.Bookings
                                .Where(b =>
                                        EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                    f => f.Include(r => r.Rates),
                    f => f.Include(o => o.Owner));

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(fields.Data),
                Total = fields.Total
            };
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsAsync(FieldsLocationRequest request)
        {
            var fields = await _unitOfWork.FieldRepository
                                            .GetLoadingListAsync(x =>
                                                x.Latitude >= request.FromLatitude &&
                                                x.Latitude <= request.ToLatitude &&
                                                x.Longitude >= request.FromLongitude &&
                                                x.Longitude <= request.ToLongitude &&
                                                (request.FromPrice == null || x.Price >= request.FromPrice) &&
                                                (request.ToPrice == null || x.Price <= request.ToPrice) &&
                                                (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                                (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)) &&
                                                x.Status.ToLower().Equals(FieldStatus.ACCEPTED),
                                                request.Limit, request.Offset,
                                                f => f.Include(pf => pf.PartialFields)
                                                        .ThenInclude(b => b.Bookings
                                                            .Where(b =>
                                                                    EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                f => f.Include(r => r.Rates),
                                                f => f.Include(o => o.Owner));

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(fields.Data),
                Total = fields.Total
            };
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetFieldsByScanRadius(FieldsScanLocationRequest request)
        {
            // Fetch fields from the database
            var fields = await _unitOfWork.FieldRepository
                                          .GetLoadingListAsync(x =>
                                            (request.FromPrice == null || x.Price >= request.FromPrice) &&
                                            (request.ToPrice == null || x.Price <= request.ToPrice) &&
                                            (request.FromStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) >= request.FromStar)) &&
                                            (request.ToStar == null || (x.Rates.Any() && x.Rates.Average(r => r.Star) <= request.ToStar)) &&
                                              x.Status.ToLower().Equals(FieldStatus.ACCEPTED),
                                              request.Limit, request.Offset,
                                              f => f.Include(pf => pf.PartialFields)
                                                    .ThenInclude(b => b.Bookings
                                                        .Where(b =>
                                                            EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                              f => f.Include(r => r.Rates),
                                              f => f.Include(o => o.Owner));

            var filteredFields = fields.Data
                .AsEnumerable()
                .Where(x => CalculateHaversineDistance(x.Latitude, x.Longitude, request.Latitude, request.Longitude) <= request.Radius)
                .ToList();

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(filteredFields),
                Total = filteredFields.Count
            };
        }

        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;

            double lat1Rad = ConvertToRadians(lat1);
            double lon1Rad = ConvertToRadians(lon1);
            double lat2Rad = ConvertToRadians(lat2);
            double lon2Rad = ConvertToRadians(lon2);

            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private double ConvertToRadians(double degree)
        {
            return degree * Math.PI / 180.0;
        }

        public async Task<FieldResponse> RejectFieldAsync(int id)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(b => b.Id == id
                                                                        && b.Status.ToLower() == FieldStatus.WAITING.ToLower(),
                                                                        p => p.PartialFields,
                                                                        t => t.Owner);
            if (field == null)
                throw new NotFoundException("Field not exist!");

            field.Status = FieldStatus.REJECTED;

            _unitOfWork.FieldRepository.Update(field);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<FieldResponse>(field);
            }
            throw new ConflictException("The requested Field failed!");
        }

        public async Task<FieldResponse> HandleStatusAsync(int id, string status)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(b => b.Id == id,
                                                                        p => p.PartialFields,
                                                                        t => t.Owner);
            if (field == null)
                throw new NotFoundException("Field not exist!");

            string statusBefore = field.Status.ToLower();

            status = status.ToLower();
            field.Status = status.Equals(FieldStatus.REJECTED.ToLower()) ? FieldStatus.REJECTED :
                             (status.Equals(FieldStatus.WAITING.ToLower()) ? FieldStatus.WAITING :
                             (status.Equals(FieldStatus.ACCEPTED.ToLower()) ? FieldStatus.ACCEPTED :
                             (status.Equals(FieldStatus.BANNED.ToLower()) ? FieldStatus.BANNED : "INVALID")));

            if (field.Status == "INVALID")
                throw new ConflictException("Request action invalid!");

            _unitOfWork.FieldRepository.Update(field);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                bool isSuccess = false;
                if (statusBefore == FieldStatus.ACCEPTED.ToLower() && field.Status.ToLower() == FieldStatus.REJECTED.ToLower())
                {
                    isSuccess = await _emailService.SendEmailAsync(field.Owner.Email, EmailConstants.SUBJECT_BANNED_FIELD, EmailConstants.BodyBanFieldEmail(field));

                    DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
                    int timeNow = (int)TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan().TotalSeconds;

                    var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.PartialField.FieldId == id
                                                                                    && !EF.Functions.Like(b.Status, BookingStatus.CANCELED)
                                                                                    && !EF.Functions.Like(b.Status, BookingStatus.REJECTED)
                                                                                    && (b.Date > dateNow
                                                                                    || (b.Date == dateNow
                                                                                    && b.StartTime >= timeNow)),
                                                                                    b => b.PartialField.Field);
                    foreach (var booking in bookings)
                    {
                        await HandleStatusAsync(booking);
                    }
                    var receiverIds = new List<int> { field.OwnerId };

                    var notification = new Notification()
                    {
                        Title = $"Admin tắt hoạt động ${field.Name}",
                        Content = $"Admin đã tắt hoạt động sân: \"{field.Name}\"!"
                    };
                    await _notificationService.SendNotificationToListUser(receiverIds, notification);

                }
                else if (statusBefore == FieldStatus.REJECTED.ToLower() && field.Status.ToLower() == FieldStatus.ACCEPTED.ToLower())
                {
                    isSuccess = await _emailService.SendEmailAsync(field.Owner.Email, EmailConstants.SUBJECT_REINSTATE_FIELD, EmailConstants.BodyReinstateFieldEmail(field));

                    var receiverIds = new List<int> { field.OwnerId };
                    var notification = new Notification()
                    {
                        Title = $"Admin kích hoạt ${field.Name}",
                        Content = $"Admin đã kích hoạt sân: \"{field.Name}\"!"
                    };
                    await _notificationService.SendNotificationToListUser(receiverIds, notification);
                }
                else if (field.Status.ToLower() == FieldStatus.REJECTED.ToLower())
                {
                    isSuccess = await _emailService.SendEmailAsync(field.Owner.Email, EmailConstants.SUBJECT_INACTIVATE_FIELD, EmailConstants.BodyInActiveFieldEmail(field));

                    var receiverIds = new List<int> { field.OwnerId };
                    var notification = new Notification()
                    {
                        Title = $"Admin từ chối ${field.Name}",
                        Content = $"Admin đã từ chối kích hoạt sân: \"{field.Name}\"!"
                    };
                    await _notificationService.SendNotificationToListUser(receiverIds, notification);
                }
                else if (field.Status.ToLower() == FieldStatus.ACCEPTED.ToLower())
                {
                    isSuccess = await _emailService.SendEmailAsync(field.Owner.Email, EmailConstants.SUBJECT_ACTIVATE_FIELD, EmailConstants.BodyActiveFieldEmail(field));

                    var receiverIds = new List<int> { field.OwnerId };
                    var notification = new Notification()
                    {
                        Title = $"Admin kích hoạt ${field.Name}",
                        Content = $"Admin đã kích hoạt sân: \"{field.Name}\"!"
                    };
                    await _notificationService.SendNotificationToListUser(receiverIds, notification);
                }
                if (!isSuccess)
                {
                    throw new InvalidOperationException("Failed to send email");
                }
                return _mapper.Map<FieldResponse>(field);
            }
            throw new ConflictException("The requested Field failed!");
        }

        public async Task SoftDeleteFieldAsync(int id)
        {
            DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
            int timeNow = (int)TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan().TotalSeconds;

            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.PartialField.FieldId == id
                                                                            && !EF.Functions.Like(b.Status, BookingStatus.CANCELED)
                                                                            && !EF.Functions.Like(b.Status, BookingStatus.REJECTED)
                                                                            && (b.Date > dateNow
                                                                            || (b.Date == dateNow
                                                                            && b.StartTime >= timeNow)),
                                                                            b => b.PartialField.Field);

            var affectedRows = await _unitOfWork.FieldRepository.SoftDeleteFieldAndPartialFieldsAsync(id);

            if (affectedRows == 0)
                throw new NotFoundException("Field not exist!");
            foreach (var booking in bookings)
            {
                await HandleStatusAsync(booking);
            }
        }

        private async Task HandleStatusAsync(Booking booking)
        {
            booking.Status = BookingStatus.REJECTED;
            _unitOfWork.BookingRepository.Update(booking);

            if (await _unitOfWork.CommitAsync() <= 0)
                return;

            await CreateAndSaveTransactionAsync(booking);
        }

        private async Task CreateAndSaveTransactionAsync(Booking booking)
        {
            var trans = new MatchFinder.Domain.Entities.Transaction
            {
                Status = TransactionStatus.SUCCESSFUL,
                Amount = booking.DepositAmount,
                Booking = booking,
                UserId = 1
            };

            if (booking.Status == BookingStatus.REJECTED)
            {
                trans.Type = TransactionType.REFUND;
                trans.Description = $"Hoàn tiền đặt sân tại {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào {booking.Date.ToString("dd/MM/YYYY")} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                trans.ReciverId = booking.UserId;
            }

            await _unitOfWork.TransactionRepository.AddAsync(trans);
            await _unitOfWork.CommitAsync();
        }

        private string GetTimeString(int minutes)
        {
            return TimeOnly.MinValue.AddMinutes(minutes / 60).ToString("HH:mm");
        }

        public async Task<FieldResponse> UpdateFieldAsync(FieldUpdateRequest request)
        {
            var cover = string.Empty;
            if (request.Cover != null && _fileService.IsImageFile(request.Cover))
                cover = await _fileService.SaveFileAsync(request.Cover);
            var avt = string.Empty;
            if (request.Avatar != null && _fileService.IsImageFile(request.Avatar))
                avt = await _fileService.SaveFileAsync(request.Avatar);

            var field = await _unitOfWork.FieldRepository.GetLoadingAsync(b => b.Id == request.Id
                                                                        && !EF.Functions.Like(b.Status, FieldStatus.REJECTED),
                                                                        f => f.Include(pf => pf.PartialFields)
                                                                                .ThenInclude(b => b.Bookings
                                                                                    .Where(b =>
                                                                                            EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                                        f => f.Include(r => r.Rates),
                                                                        f => f.Include(o => o.Owner));
            if (field == null)
                throw new NotFoundException("Field not exist!");

            field.Name = request.Name ?? field.Name;
            field.Address = request.Address ?? field.Address;
            field.Province = request.Province ?? field.Province;
            field.District = request.District ?? field.District;
            field.Commune = request.Commune ?? field.Commune;
            field.PhoneNumber = request.PhoneNumber ?? field.PhoneNumber;
            field.Latitude = request.Latitude != null ? Math.Round((double)request.Latitude, 9) : field.Latitude;
            field.Longitude = request.Longitude != null ? Math.Round((double)request.Longitude, 9) : field.Longitude;
            int newOpenTime = request.OpenTime ?? field.OpenTime;
            int newCloseTime = request.CloseTime ?? field.CloseTime;

            if (newOpenTime > newCloseTime)
                throw new InvalidOperationException("OpenTime cannot be greater than CloseTime.");

            field.OpenTime = newOpenTime;
            field.CloseTime = newCloseTime;
            field.Description = request.Description ?? field.Description;
            field.Avatar = string.IsNullOrEmpty(avt) ? field.Avatar : avt;
            field.Cover = string.IsNullOrEmpty(cover) ? field.Cover : cover;
            field.IsFixedSlot = request.IsFixedSlot ?? field.IsFixedSlot;
            field.Price = request.Price ?? field.Price;
            field.Deposit = request.Deposit ?? field.Deposit;

            _unitOfWork.FieldRepository.Update(field);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                await RemoveIndexForFieldAsync(field.Id);
                await IndexFootballFieldAsync(field);
                return _mapper.Map<FieldResponse>(field);
            }
            throw new ConflictException("The requested Field failed!");
        }

        public async Task<FieldResponse> FixedSlotFieldAsync(FixedSlotRequest request)
        {
            var field = await _unitOfWork.FieldRepository.GetLoadingAsync(b => b.Id == request.Id
                                                                        && !EF.Functions.Like(b.Status, FieldStatus.REJECTED),
                                                                        f => f.Include(pf => pf.PartialFields)
                                                                                .ThenInclude(b => b.Bookings
                                                                                    .Where(b =>
                                                                                            EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                                        f => f.Include(r => r.Rates),
                                                                        f => f.Include(o => o.Owner));
            if (field == null)
                throw new NotFoundException("Field not exist!");

            field.IsFixedSlot = request.IsFixedSlot;

            _unitOfWork.FieldRepository.Update(field);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<FieldResponse>(field);
            }
            throw new ConflictException("The requested Field failed!");
        }

        public async Task<object> GetFreeSlot(GetFreeSlotRequest request)
        {
            var field = await _unitOfWork.FieldRepository.GetLoadingAsync(f => f.Id == request.Id
                                                                            && f.IsFixedSlot
                                                                            && EF.Functions.Like(f.Status, FieldStatus.ACCEPTED),
                                                                            f => f.Include(pf => pf.PartialFields)
                                                                                    .ThenInclude(b => b.Bookings
                                                                                        .Where(b =>
                                                                                                EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                                                                            f => f.Include(r => r.Slots),
                                                                            f => f.Include(r => r.Rates),
                                                                            f => f.Include(o => o.Owner));
            if (field == null)
            {
                throw new NotFoundException("Field not found or not a fixed slot field");
            }

            var result = new List<FreeSlotsResponse>();

            for (var date = request.FromDate; date <= request.ToDate; date = date.AddDays(1))
            {
                var availableSlots = new HashSet<Slot>();

                foreach (var partialField in field.PartialFields)
                {
                    var bookedSlots = partialField.Bookings
                        .Where(b => b.Date == date)
                        .Select(b => new { b.StartTime, b.EndTime })
                        .ToList();

                    foreach (var slot in field.Slots)
                    {
                        if (!bookedSlots.Any(b => b.StartTime <= slot.StartTime && b.EndTime > slot.StartTime))
                        {
                            availableSlots.Add(slot);
                        }
                    }
                }

                if (availableSlots.Any())
                {
                    result.Add(new FreeSlotsResponse
                    {
                        Date = date,
                        Slots = availableSlots.Select(s => new SlotResponse
                        {
                            Id = s.Id,
                            StartTime = TimeSpan.FromSeconds(s.StartTime).ToString(@"hh\:mm"),
                            EndTime = TimeSpan.FromSeconds(s.EndTime).ToString(@"hh\:mm"),
                            FieldId = field.Id,
                            FieldName = field.Name
                        }).OrderBy(s => s.StartTime).ToList()
                    });
                }
            }
            return result;
        }

        public async Task<IEnumerable<FieldResponse>> GetRatingField(FieldRatingFilterRequest request)
        {
            var fields = await _unitOfWork.FieldRepository.GetFieldByDistance(request.Lat, request.Long, 20);
            fields = fields.OrderByDescending(f => f.Rates.Any() ? f.Rates.Average(r => r.Star) : 0)
                .ThenByDescending(f => f.PartialFields.Count(pf => pf.Bookings.Any(b => b.Status == "ACCEPTED")))
                .Take(6)
                .ToList();

            return _mapper.Map<IEnumerable<FieldResponse>>(fields);
        }

        public async Task<IEnumerable<FieldResponse>> GetRecommendField(int fieldId)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(f => f.Id == fieldId && f.Status == FieldStatus.ACCEPTED,
                                                                    f => f.PartialFields);
            if (field == null)
            {
                throw new NotFoundException("Field not found!");
            }
            var minAmount = field.Price - 50000;
            var maxAmount = field.Price + 50000;

            var fields = await _unitOfWork.FieldRepository.GetFieldByDistance(field.Latitude, field.Longitude, 10);
            fields = fields.Where(f => f.Price >= minAmount && f.Price <= maxAmount && f.PartialFields.Any(pf => pf.Status == PartialFieldStatus.ACTIVE &&
                                                                pf.Bookings.Any(b => b.Status != BookingStatus.ACCEPTED)));
            return _mapper.Map<IEnumerable<FieldResponse>>(fields);
        }
    }
}