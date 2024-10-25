using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace MatchFinder.Application.Services.Impl
{
    public class PartialFieldService : IPartialFieldService
    {
        private IMapper _mapper;
        private IFileService _fileService;
        private IUnitOfWork _unitOfWork;

        public PartialFieldService(IMapper mapper, IFileService fileService, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _fileService = fileService;
            _unitOfWork = unitOfWork;
        }

        public async Task<RepositoryPaginationResponse<PartialFieldResponse>> GetListAsync(PartialFieldFilterRequest filterRequest)
        {
            var partialFields = await _unitOfWork.PartialFieldRepository
                .GetListAsync(x =>
                                (string.IsNullOrEmpty(filterRequest.FieldName) || x.Field.Name.Contains(filterRequest.FieldName)) &&
                                (string.IsNullOrEmpty(filterRequest.PartialFieldName) || x.Name.Contains(filterRequest.PartialFieldName)) &&
                                (string.IsNullOrEmpty(filterRequest.Status) || x.Status == filterRequest.Status) &&
                                (x.Field.CreatedAt >= filterRequest.FromDate.Value.ToDateTime(TimeOnly.MinValue)) &&
                                (x.Field.CreatedAt <= filterRequest.ToDate.Value.ToDateTime(TimeOnly.MaxValue))
                ,
                filterRequest.Limit, filterRequest.Offset,
                 x => x.Field);
            return new RepositoryPaginationResponse<PartialFieldResponse>
            {
                Data = _mapper.Map<IEnumerable<PartialFieldResponse>>(partialFields.Data),
                Total = partialFields.Total
            };
        }

        public async Task<RepositoryPaginationResponse<PartialFieldResponse>> GetListAvailable(int fieldId, PartialFieldAvailableRequest filterRequest)
        {
            var requestDate = filterRequest.Date.ToDateTime(TimeOnly.MinValue);
            var field = await _unitOfWork.FieldRepository.GetQueryable(f => f.Id == fieldId)
                                                        .Include(pf => pf.PartialFields)
                                                            .ThenInclude(b => b.Bookings
                                                            .Where(b =>
                                                                    !EF.Functions.Like(b.Status, BookingStatus.REJECTED)
                                                                    && !EF.Functions.Like(b.Status, BookingStatus.CANCELED)
                                                                    && (b.IsDeleted == null || b.IsDeleted == false)))
                                                        .Include(it => it.InactiveTimes.Where(item => item.IsDeleted != true))
                                                        .FirstOrDefaultAsync();
            if (field == null)
                throw new NotFoundException("Field not found");

            var query = _unitOfWork.PartialFieldRepository.GetQueryable()
                .Where(pf =>
                    pf.FieldId == fieldId &&
                    EF.Functions.Like(pf.Field.Status, FieldStatus.ACCEPTED) &&
                    EF.Functions.Like(pf.Status, PartialFieldStatus.ACTIVE)
                );

            if (filterRequest.StartTime.HasValue)
            {
                var startTime = filterRequest.StartTime.Value;
                var endTime = filterRequest.StartTime.Value + filterRequest.Duration;
                query = query.Where(pf =>
                    (pf.Field.IsFixedSlot == false &&
                        !pf.Bookings.Any(b =>
                            b.Date == filterRequest.Date &&
                            ((b.StartTime <= startTime && b.EndTime > startTime) ||
                             (b.StartTime < endTime &&
                              b.EndTime >= endTime) ||
                             (b.StartTime >= startTime && b.EndTime <= endTime))
                        ) &&
                        !pf.Field.InactiveTimes.Any(it =>
                            it.StartTime <= requestDate.AddSeconds(endTime) &&
                            it.EndTime > requestDate.AddSeconds(startTime))
                    ) ||
                    (pf.Field.IsFixedSlot == true &&
                        pf.Field.Slots.Any(s =>
                            s.StartTime == startTime &&
                            !pf.Bookings.Any(b =>
                                b.Date == filterRequest.Date
                                && b.StartTime == s.StartTime
                            ) && 
                            !pf.Field.InactiveTimes.Any(it =>
                            it.StartTime <= requestDate.AddSeconds(endTime) &&
                            it.EndTime > requestDate.AddSeconds(startTime))
                        )
                    )
                );
            }
            else
            {
                if (field.IsFixedSlot == false)
                {
                    IEnumerable<PartialField> partialFieldsFixSlot = await query.Include(b => b.Bookings
                                                                                .Where(b =>
                                                                                        !EF.Functions.Like(b.Status, BookingStatus.REJECTED)
                                                                                        && !EF.Functions.Like(b.Status, BookingStatus.CANCELED)
                                                                                        && (b.IsDeleted == null || b.IsDeleted == false)))
                                                                                .ToListAsync();
                    partialFieldsFixSlot = partialFieldsFixSlot
                                            .Where(pf => IsPartialFieldAvailable(pf, field, filterRequest.Date, filterRequest.Duration))
                                            .ToList();

                    int fieldTotal = partialFieldsFixSlot.Count();
                    partialFieldsFixSlot = partialFieldsFixSlot.Skip(filterRequest.Offset).Take(filterRequest.Limit);

                    return new RepositoryPaginationResponse<PartialFieldResponse>
                    {
                        Data = _mapper.Map<IEnumerable<PartialFieldResponse>>(partialFieldsFixSlot),
                        Total = fieldTotal
                    };
                }

                query = query.Where(pf =>
                                        pf.Field.Slots.Any(s =>
                                            !pf.Bookings.Any(b =>
                                                b.Date == filterRequest.Date
                                                &&
                                                ((b.StartTime <= s.StartTime && b.EndTime >= s.StartTime) ||
                                                (b.StartTime <= s.EndTime && b.EndTime >= s.EndTime) ||
                                                (b.StartTime <= s.StartTime && b.EndTime >= s.EndTime))
                                            )
                                            && !pf.Field.InactiveTimes.Any(it =>
                                                it.StartTime <= requestDate &&
                                                it.EndTime > requestDate)
                                        )
                );
            }

            int total = await query.CountAsync();
            query = query.Skip(filterRequest.Offset).Take(filterRequest.Limit);
            var partialFields = await query.ToListAsync();

            return new RepositoryPaginationResponse<PartialFieldResponse>
            {
                Data = _mapper.Map<IEnumerable<PartialFieldResponse>>(partialFields),
                Total = total
            };
        }

        private bool IsPartialFieldAvailable(PartialField partialField, Field field, DateOnly date, int duration)
        {
            var bookings = partialField.Bookings
                .Where(b => b.Date == date && (b.Status.ToLower() == BookingStatus.ACCEPTED.ToLower()
                                                ||  b.Status.ToLower() == BookingStatus.WAITING.ToLower()))
                .OrderBy(b => b.StartTime)
                .ToList();

            var inactiveTimes = field.InactiveTimes
                .Where(it => it.StartTime <= date.ToDateTime(TimeOnly.MaxValue) &&
                             it.EndTime >= date.ToDateTime(TimeOnly.MinValue))
                .ToList();

            int currentTime = field.OpenTime;

            foreach (var b in bookings)
            {
                int endTime = currentTime + duration;
                bool isBooked = bookings.Any(b => b.StartTime < endTime && b.EndTime > currentTime);

                bool isInactive = inactiveTimes.Any(it =>
                    it.StartTime.TimeOfDay.TotalSeconds < endTime &&
                    it.EndTime.TimeOfDay.TotalSeconds > currentTime);

                if (!isBooked && !isInactive && currentTime < field.CloseTime)
                    return true;

                if (b == bookings.Last())
                    return false;

                currentTime = b.EndTime;
            }

            var currentTimeInactive = date.ToDateTime(TimeOnly.MinValue).AddSeconds(field.OpenTime);
            foreach (var i in inactiveTimes)
            {
                var endTime = currentTimeInactive.AddSeconds(duration);

                bool isInactive = inactiveTimes.Any(it =>
                    it.StartTime < endTime &&
                    it.EndTime > currentTimeInactive);

                if (!isInactive && currentTimeInactive < date.ToDateTime(TimeOnly.MinValue).AddSeconds(field.CloseTime))
                    return true;

                if (i == inactiveTimes.Last())
                    return false;

                currentTimeInactive = i.EndTime;
            }

            return true;
        }

        public async Task<IEnumerable<PartialFieldWithNumberBookingsResponse>> GetListByFieldAsync(int fieldId)
        {
            var partialFields = await _unitOfWork.PartialFieldRepository.GetAllAsync(x => x.FieldId == fieldId,
                x => x.Field,
                x => x.Bookings);
            return _mapper.Map<IEnumerable<PartialFieldWithNumberBookingsResponse>>(partialFields);
        }

        public async Task<PartialFieldResponse> GetByIdAsync(int id)
        {
            var partialField = await _unitOfWork.PartialFieldRepository.GetAsync(x => x.Id == id, x => x.Field);
            if (partialField == null)
            {
                throw new NotFoundException("Partial field not found");
            }
            return _mapper.Map<PartialFieldResponse>(partialField);
        }

        public async Task<PartialFieldResponse> CreatePartialFieldAsync(int uid, PartialFieldCreateRequest request)
        {
            var fieldExist = await _unitOfWork.FieldRepository.GetAsync(x => x.Id == request.FieldId && x.OwnerId == uid && x.Status == FieldStatus.ACCEPTED);
            if (fieldExist == null)
            {
                throw new NotFoundException("Field not found");
            }
            var urlImage1 = string.Empty;
            if (request.Image_1 != null && _fileService.IsImageFile(request.Image_1))
                urlImage1 = await _fileService.SaveFileAsync(request.Image_1);
            var urlImage2 = string.Empty;
            if (request.Image_2 != null && _fileService.IsImageFile(request.Image_2))
                urlImage2 = await _fileService.SaveFileAsync(request.Image_2);

            var mewPartialField = new PartialField
            {
                Name = request.Name,
                Description = request.Description,
                Image1 = urlImage1,
                Image2 = urlImage2,
                FieldId = request.FieldId,
                Status = PartialFieldStatus.ACTIVE
            };

            await _unitOfWork.PartialFieldRepository.AddAsync(mewPartialField);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<PartialFieldResponse>(mewPartialField);
            }
            throw new ConflictException("Create partial field fail");
        }

        public async Task<PartialFieldResponse> UpdatePartialFieldAsync(int id, PartialFieldUpdateRequest request)
        {
            if (request.FieldId.HasValue)
            {
                var fieldExist = await _unitOfWork.FieldRepository.GetAsync(x => x.Id == request.FieldId && x.Status == FieldStatus.ACCEPTED);
                if (fieldExist == null)
                {
                    throw new NotFoundException("Field not found");
                }
            }

            var partialField = await _unitOfWork.PartialFieldRepository.GetAsync(x => x.Id == id);
            if (partialField == null)
            {
                throw new NotFoundException("Partial field not found");
            }

            var urlImage1 = string.Empty;
            if (request.Image_1 != null && _fileService.IsImageFile(request.Image_1))
                urlImage1 = await _fileService.SaveFileAsync(request.Image_1);
            var urlImage2 = string.Empty;
            if (request.Image_2 != null && _fileService.IsImageFile(request.Image_2))
                urlImage2 = await _fileService.SaveFileAsync(request.Image_2);

            partialField.Name = request.Name ?? partialField.Name;
            partialField.Description = request.Description ?? partialField.Description;
            partialField.Image1 = string.IsNullOrEmpty(urlImage1) ? partialField.Image1 : urlImage1;
            partialField.Image2 = string.IsNullOrEmpty(urlImage2) ? partialField.Image2 : urlImage2;
            partialField.FieldId = request.FieldId ?? partialField.FieldId;
            partialField.Status = request.Status ?? partialField.Status;

            _unitOfWork.PartialFieldRepository.Update(partialField);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                if (!string.IsNullOrEmpty(request.Status) && request.Status == PartialFieldStatus.INACTIVE && await IsHavingBooking(partialField.Id))
                {
                    DateOnly dateNow = DateOnly.FromDateTime(DateTime.Now);
                    int timeNow = (int)TimeOnly.FromDateTime(DateTime.Now).ToTimeSpan().TotalSeconds;

                    var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.PartialFieldId == id
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
                }
                return _mapper.Map<PartialFieldResponse>(partialField);
            }
            throw new ConflictException("Update partial field fail");
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
                trans.Description = $"Hoàn tiền đặt sân tại {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào {booking.Date.ToString("dd/MM/yyyy")} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                trans.ReciverId = booking.UserId;
            }

            await _unitOfWork.TransactionRepository.AddAsync(trans);
            await _unitOfWork.CommitAsync();
        }

        private string GetTimeString(int minutes)
        {
            return TimeOnly.MinValue.AddMinutes(minutes / 60).ToString("HH:mm");
        }

        public async Task<int> GetOwnerIdAsync(int partialFieldId)
        {
            var partialField = await _unitOfWork.PartialFieldRepository.GetAsync(x => x.Id == partialFieldId, x => x.Field);
            return partialField.Field.OwnerId;
        }

        public async Task<bool> IsHavingBooking(int partialFieldId)
        {
            // Get the current date and time
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentTimeInSeconds = (int)DateTime.Now.TimeOfDay.TotalSeconds;

            var booking = await _unitOfWork.BookingRepository

                .GetAllAsync(b => b.PartialFieldId == partialFieldId &&
                                  (b.Date > currentDate ||
                                  (b.Date == currentDate && b.StartTime > currentTimeInSeconds)));
            var hasBooking = booking.Any();

            return hasBooking;
        }
    }
}