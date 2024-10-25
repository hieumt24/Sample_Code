using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;

namespace MatchFinder.Application.Services
{
    public interface IStatisticService
    {
        Task<IEnumerable<StatisticBookingMonthlyResponse>> GetBookingMonthlyAsync(int requestorId, StatisticRequestFilter request);

        Task<Dictionary<string, int>> GetBookingSlotAsync(int requestorId, StatisticBookingSlotRequest request);

        Task<StatisticBookingWeekDayResponse> GetBookingWeekDayAsync(int requestorId, StatisticRequestFilter request);

        Task<StatisticBookingStatusResponse> GetBookingStatusAsync(int requestorId, StatisticRequestFilter request);

        Task<IEnumerable<StatisticRegisterMonthlyResponse>> GetRegisterMonthlyAsync(int requestorId, StatisticRequestFilter request);
    }
}