using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Application.Services.Impl
{
    public class StatisticService : IStatisticService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StatisticService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<StatisticBookingMonthlyResponse>> GetBookingMonthlyAsync(int requestorId, StatisticRequestFilter request)
        {
            var bookings = await GetBookingsAsync(requestorId, request);

            var response = InitializeMonthlyResponses<StatisticBookingMonthlyResponse>(request.FromDate.Year, request.ToDate.Year);

            var result = bookings.GroupBy(x => new { x.Date.Month, x.Date.Year })
                .Select(x => new
                {
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    Total = x.Count()
                });

            foreach (var item in result)
            {
                var monthResponse = response.FirstOrDefault(x => x.Month == item.Month && x.Year == item.Year);
                if (monthResponse != null)
                {
                    monthResponse.Total = item.Total;
                }
            }

            return response;
        }

        public async Task<StatisticBookingWeekDayResponse> GetBookingWeekDayAsync(int requestorId, StatisticRequestFilter request)
        {
            var bookings = await GetBookingsAsync(requestorId, request);

            var result = bookings.GroupBy(x => x.Date.DayOfWeek)
                .Select(x => new
                {
                    Day = x.Key,
                    Total = x.Count()
                });

            var response = new StatisticBookingWeekDayResponse();
            WeekDayResponse(response, result);
            return response;
        }

        public async Task<StatisticBookingStatusResponse> GetBookingStatusAsync(int requestorId, StatisticRequestFilter request)
        {
            var bookings = await GetBookingsAsync(requestorId, request);

            var statusGrouped = bookings.GroupBy(x => x.Status)
                .Select(x => new
                {
                    Status = x.Key,
                    Total = x.Count()
                }).ToList();

            var monthlyStatusGrouped = bookings.GroupBy(x => new { x.Status, x.Date.Month, x.Date.Year })
                .Select(x => new
                {
                    Status = x.Key.Status,
                    Month = x.Key.Month,
                    Year = x.Key.Year,
                    Total = x.Count()
                }).ToList();

            var response = new StatisticBookingStatusResponse();

            response.Monthly = InitializeMonthlyResponses<StatusMonthly>(request.FromDate.Year, request.ToDate.Year);

            foreach (var item in statusGrouped)
            {
                switch (item.Status)
                {
                    case BookingStatus.CANCELED:
                        response.CanceledTotal = item.Total;
                        break;

                    case BookingStatus.ACCEPTED:
                        response.AcceptedTotal = item.Total;
                        break;

                    case BookingStatus.REJECTED:
                        response.RejectedTotal = item.Total;
                        break;
                }
            }

            foreach (var item in monthlyStatusGrouped)
            {
                var statusMonthly = response.Monthly.FirstOrDefault(x => x.Month == item.Month && x.Year == item.Year);

                if (statusMonthly != null)
                {
                    switch (item.Status)
                    {
                        case BookingStatus.CANCELED:
                            statusMonthly.Canceled = item.Total;
                            break;

                        case BookingStatus.ACCEPTED:
                            statusMonthly.Accepted = item.Total;
                            break;

                        case BookingStatus.REJECTED:
                            statusMonthly.Rejected = item.Total;
                            break;
                    }
                }
            }

            return response;
        }

        public async Task<IEnumerable<StatisticRegisterMonthlyResponse>> GetRegisterMonthlyAsync(int requestorId, StatisticRequestFilter request)
        {
            if (await IsAdmin(requestorId))
            {
                var fromDate = request.FromDate.ToDateTime(TimeOnly.MinValue);
                var toDate = request.ToDate.ToDateTime(TimeOnly.MaxValue);

                var result = await _unitOfWork.UserRepository
                    .GetAllAsync(x => x.Status == UserStatus.ACTIVE &&
                    x.CreatedAt >= fromDate && x.CreatedAt <= toDate);

                var response = InitializeMonthlyResponses<StatisticRegisterMonthlyResponse>(request.FromDate.Year, request.ToDate.Year);

                var groupedResult = result.GroupBy(x => new { x.CreatedAt.Value.Month, x.CreatedAt.Value.Year })
                    .Select(x => new
                    {
                        Month = x.Key.Month,
                        Year = x.Key.Year,
                        Total = x.Count()
                    });

                foreach (var item in groupedResult)
                {
                    var monthResponse = response.FirstOrDefault(x => x.Month == item.Month && x.Year == item.Year);
                    if (monthResponse != null)
                    {
                        monthResponse.Total = item.Total;
                    }
                }

                return response;
            }
            else
            {
                throw new ConflictException("You can only get statistic registration user if you are admin");
            }
        }

        public async Task<Dictionary<string, int>> GetBookingSlotAsync(int requestorId, StatisticBookingSlotRequest request)
        {
            var field = await _unitOfWork.FieldRepository.GetLoadingAsync(f => f.Id == request.FieldId,
                                                                          f => f.Include(s => s.Slots
                                                                                .Where(slot => !(slot.IsDeleted ?? false))
                                                                                ));
            if (field == null)
            {
                throw new NotFoundException("Field not found");
            }
            if (field.OwnerId != requestorId)
            {
                throw new UnauthorizedAccessException("You do not have permission to view this field's bookings");
            }
            if (!field.IsFixedSlot)
            {
                throw new ConflictException("This field does not use fixed slots");
            }

            var allSlots = field.Slots.Select(s => $"{ConvertSecondsToTimeString(s.StartTime)} - {ConvertSecondsToTimeString(s.EndTime)}").Distinct().ToList();

            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(
                b => b.PartialField.FieldId == request.FieldId &&
                     b.Date >= request.FromDate &&
                     b.Date <= request.ToDate &&
                     b.PartialField.Field.Slots.Any(s => s.StartTime == b.StartTime &&
                                                         s.EndTime == b.EndTime),
                b => b.PartialField
            );

            var bookingCounts = bookings
                .GroupBy(b => $"{ConvertSecondsToTimeString(b.StartTime)} - {ConvertSecondsToTimeString(b.EndTime)}")
                .ToDictionary(
                    sg => sg.Key,
                    sg => sg.Count()
                );

            var result = allSlots.ToDictionary(
                slot => slot,
                slot => bookingCounts.ContainsKey(slot) ? bookingCounts[slot] : 0
            );

            return result;
        }

        private string ConvertSecondsToTimeString(int totalSeconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(totalSeconds);
            return time.ToString(@"hh\:mm");
        }

        private async Task<IEnumerable<Booking>> GetBookingsAsync(int requestorId, StatisticRequestFilter request)
        {
            if (await IsAdmin(requestorId))
            {
                return await _unitOfWork.BookingRepository
                    .GetAllAsync(x =>
                        (!request.FieldId.HasValue || x.PartialField.FieldId == request.FieldId.Value) &&
                        x.Date >= request.FromDate && x.Date <= request.ToDate,
                        x => x.PartialField);
            }

            if (request.FieldId.HasValue && await IsOwner(requestorId, request.FieldId.Value))
            {
                return await _unitOfWork.BookingRepository
                    .GetAllAsync(x =>
                        x.PartialField.Field.OwnerId == requestorId &&
                        x.PartialField.FieldId == request.FieldId.Value &&
                        x.Date >= request.FromDate && x.Date <= request.ToDate,
                        x => x.PartialField);
            }

            throw new ConflictException("You can only get statistic your fields");
        }

        private void WeekDayResponse(StatisticBookingWeekDayResponse response, IEnumerable<dynamic> result)
        {
            foreach (var item in result)
            {
                switch (item.Day)
                {
                    case DayOfWeek.Monday:
                        response.Monday = item.Total;
                        break;

                    case DayOfWeek.Tuesday:
                        response.Tuesday = item.Total;
                        break;

                    case DayOfWeek.Wednesday:
                        response.Wednesday = item.Total;
                        break;

                    case DayOfWeek.Thursday:
                        response.Thursday = item.Total;
                        break;

                    case DayOfWeek.Friday:
                        response.Friday = item.Total;
                        break;

                    case DayOfWeek.Saturday:
                        response.Saturday = item.Total;
                        break;

                    case DayOfWeek.Sunday:
                        response.Sunday = item.Total;
                        break;
                }
            }
        }

        private async Task<bool> IsAdmin(int requestorId)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == requestorId);
            if (user == null)
                throw new NotFoundException("User not found");

            return user.RoleId == RoleConstant.ADMIN_ID;
        }

        private async Task<bool> IsOwner(int requestorId, int fieldId)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(x => x.Id == requestorId);
            if (user == null)
                throw new NotFoundException("User not found");

            var field = await _unitOfWork.FieldRepository.GetAsync(x => x.Id == fieldId);
            if (field == null)
                throw new NotFoundException("Field not found");

            return field.OwnerId == user.Id;
        }

        private List<T> InitializeMonthlyResponses<T>(int startYear, int endYear) where T : class
        {
            var response = new List<T>();
            for (var year = startYear; year <= endYear; year++)
            {
                for (var month = 1; month <= 12; month++)
                {
                    if (typeof(T) == typeof(StatisticBookingMonthlyResponse))
                    {
                        response.Add(new StatisticBookingMonthlyResponse(month, year) as T);
                    }
                    else if (typeof(T) == typeof(StatusMonthly))
                    {
                        response.Add(new StatusMonthly(month, year) as T);
                    }
                    else if (typeof(T) == typeof(StatisticRegisterMonthlyResponse))
                    {
                        response.Add(new StatisticRegisterMonthlyResponse(month, year) as T);
                    }
                }
            }
            return response;
        }
    }
}