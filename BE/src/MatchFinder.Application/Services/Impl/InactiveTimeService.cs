using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Application.Services.Impl
{
    public class InactiveTimeService : IInactiveTimeService
    {
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private IOpponentFindingService _opponentFindingService;
        private IBookingService _bookingService;

        public InactiveTimeService(IMapper mapper, IUnitOfWork unitOfWork, IOpponentFindingService opponentFindingService, IBookingService bookingService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _opponentFindingService = opponentFindingService;
            _bookingService = bookingService;
        }

        public async Task<InactiveTimeResponse> CreateAsync(InactiveTimeCreateRequest request)
        {
            await CheckDuplicateInactiveTime(request.FieldId, request.StartTime, request.EndTime);
            var mewInactiveTime = new InactiveTime
            {
                FieldId = request.FieldId,
                Reason = request.Reason,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
            };

            await _unitOfWork.InactiveTimeRepository.AddAsync(mewInactiveTime);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                await CancelBookingAndOpponentFinding(request.FieldId, request.StartTime, request.EndTime);
                return _mapper.Map<InactiveTimeResponse>(mewInactiveTime);
            }
            throw new ConflictException("Failed to create inactive time");
        }

        public async Task<InactiveTimeResponse> DeleteAsync(int id)
        {
            var inactiveTime = await _unitOfWork.InactiveTimeRepository.GetAsync(x => x.Id == id);
            if (inactiveTime == null)
            {
                throw new NotFoundException("Inactive time not found");
            }
            _unitOfWork.InactiveTimeRepository.SoftDelete(inactiveTime);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<InactiveTimeResponse>(inactiveTime);
            }
            throw new ConflictException("Failed to delete inactive time");
        }

        public async Task<InactiveTimeResponse> GetByIdAsync(int id)
        {
            var inactiveTime = await _unitOfWork.InactiveTimeRepository.GetAsync(x => x.Id == id);
            if (inactiveTime == null)
            {
                throw new NotFoundException("Inactive time not found");
            }
            return _mapper.Map<InactiveTimeResponse>(inactiveTime);
        }

        public async Task<RepositoryPaginationResponse<InactiveTimeResponse>> GetListByFieldAsync(InactiveTimeGetRequest request)
        {
            var startTime = (request.StartDate ?? DateOnly.MinValue).ToDateTime(TimeOnly.MinValue);
            var endTime = (request.EndDate ?? DateOnly.MaxValue).ToDateTime(TimeOnly.MaxValue);
            if (request.IsPaging == true)
            {
                var inactiveTimes = await _unitOfWork.InactiveTimeRepository
                    .GetListAsync(x => x.FieldId == request.FieldId
                                    && (
                                        (x.StartTime >= startTime
                                         && x.StartTime <= endTime)
                                        || (x.EndTime >= startTime
                                            && x.EndTime <= endTime)
                                        || (x.StartTime <= startTime
                                            && x.EndTime >= endTime)
                                        ),
                                    request.Limit, request.Offset);
                return new RepositoryPaginationResponse<InactiveTimeResponse>
                {
                    Data = _mapper.Map<IEnumerable<InactiveTimeResponse>>(inactiveTimes.Data),
                    Total = inactiveTimes.Total
                };
            }
            else
            {
                var inactiveTimes = await _unitOfWork.InactiveTimeRepository
               .GetAllAsync(x => x.FieldId == request.FieldId
                               && (
                                    (x.StartTime >= startTime
                                       && x.StartTime <= endTime)
                                     || (x.EndTime >= startTime
                                         && x.EndTime <= endTime)
                                     || (x.StartTime <= startTime
                                         && x.EndTime >= endTime)
                                   ));
                return new RepositoryPaginationResponse<InactiveTimeResponse>
                {
                    Data = _mapper.Map<IEnumerable<InactiveTimeResponse>>(inactiveTimes),
                    Total = inactiveTimes.Count()
                };
            }
        }

        public async Task<InactiveTimeResponse> UpdateAsync(int id, InactiveTimeUpdateRequest request)
        {
            var inactiveTime = await _unitOfWork.InactiveTimeRepository.GetAsync(x => x.Id == id);
            if (inactiveTime == null)
            {
                throw new NotFoundException("Inactive time not found");
            }

            if (request.EndTime.HasValue)
            {
                if ((request.StartTime.HasValue && request.StartTime >= request.EndTime)
                    || (inactiveTime.StartTime >= request.EndTime))
                {
                    throw new ConflictException("End time must be greater than Start date.");
                }
            }

            await CheckDuplicateInactiveTimeForUpdate(inactiveTime.FieldId, inactiveTime.Id, request.StartTime ?? inactiveTime.StartTime, request.EndTime ?? inactiveTime.EndTime);

            inactiveTime.StartTime = request.StartTime ?? inactiveTime.StartTime;
            inactiveTime.EndTime = request.EndTime ?? inactiveTime.EndTime;
            inactiveTime.Reason = request.Reason ?? inactiveTime.Reason;

            _unitOfWork.InactiveTimeRepository.Update(inactiveTime);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                await CancelBookingAndOpponentFinding(inactiveTime.FieldId, inactiveTime.StartTime, inactiveTime.EndTime);
                return _mapper.Map<InactiveTimeResponse>(inactiveTime);
            }
            throw new ConflictException("Failed to update inactive time");
        }

        public async Task CheckDuplicateInactiveTime(int fieldId, DateTime startTime, DateTime endTime)
        {
            var inactiveTime = await _unitOfWork.InactiveTimeRepository
                .GetAsync(x => x.FieldId == fieldId
                                && ((x.StartTime <= startTime && x.EndTime > startTime) ||
                                (x.StartTime < endTime && x.EndTime >= endTime) ||
                                    (x.StartTime >= startTime && x.EndTime <= endTime) ||
                                    (x.StartTime <= startTime && x.EndTime >= endTime)));
            if (inactiveTime != null)
            {
                throw new ConflictException("This field already has inactive time for this period");
            }
        }
        public async Task CheckDuplicateInactiveTimeForUpdate(int fieldId, int inactiveTimeId, DateTime startTime, DateTime endTime)
        {
            var inactiveTime = await _unitOfWork.InactiveTimeRepository
                .GetAsync(x => x.FieldId == fieldId &&
                                x.Id != inactiveTimeId &&
                                ((x.StartTime <= startTime && x.EndTime > startTime) ||
                                (x.StartTime < endTime && x.EndTime >= endTime) ||
                                    (x.StartTime >= startTime && x.EndTime <= endTime) ||
                                    (x.StartTime <= startTime && x.EndTime >= endTime)));
            if (inactiveTime != null)
            {
                throw new ConflictException("This field already has inactive time for this period");
            }
        }

        private async Task CancelBookingAndOpponentFinding(int fieldId, DateTime startTime, DateTime endTime)
        {
            var dateStart = DateOnly.FromDateTime(startTime);
            var dateEnd = DateOnly.FromDateTime(endTime);
            var startTimeInSeconds = startTime.Hour * 3600 + startTime.Minute * 60 + startTime.Second;
            var endTimeInSeconds = endTime.Hour * 3600 + endTime.Minute * 60 + endTime.Second;

            var bookings = await _unitOfWork.BookingRepository
                .GetAllAsync(x => x.PartialField.FieldId == fieldId
                               && !EF.Functions.Like(x.Status, BookingStatus.CANCELED)
                               && !EF.Functions.Like(x.Status, BookingStatus.REJECTED)
                               && ((x.Date < dateEnd || (x.Date == dateEnd && x.StartTime < endTimeInSeconds))
                                   && (x.Date > dateStart || (x.Date == dateStart && x.EndTime > startTimeInSeconds))),
                             x => x.PartialField);

            foreach (var booking in bookings)
            {
                bool isOverlapping = (booking.Date < dateEnd || (booking.Date == dateEnd && booking.StartTime < endTimeInSeconds))
                        && (booking.Date > dateStart || (booking.Date == dateStart && booking.EndTime > startTimeInSeconds));

                if (isOverlapping)
                {
                    HandleStatusRequest request = new HandleStatusRequest
                    {
                        Id = booking.Id,
                        Status = BookingStatus.REJECTED,
                    };
                    await _bookingService.HandleStatusAsync(request);

                    var opponentFinding = await _unitOfWork.OpponentFindingRepository
                        .GetAllAsync(x => x.BookingId == booking.Id
                                        && x.UserFindingId == booking.UserId);
                    foreach (var of in opponentFinding)
                    {
                        await _opponentFindingService.CancelPost(of.Id, of.UserFindingId);
                    }

                    await _unitOfWork.CommitAsync();
                }
            }
        }
    }
}