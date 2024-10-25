using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Services
{
    public class BookingAuthorizer : IBookingAuthorizer
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingAuthorizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsAuthorizedAsync(int userId, int requestId)
        {
            var userBookings = await _unitOfWork.BookingRepository.GetAllIncludingDeletedAsync(b => b.UserId == userId);
            return userBookings.Any(b => b.Id == requestId);
        }
    }
}