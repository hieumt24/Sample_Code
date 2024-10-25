using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Services
{
    public class FieldOwnershipAuthorizer : IFieldOwnershipAuthorizer
    {
        private readonly IUnitOfWork _unitOfWork;

        public FieldOwnershipAuthorizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsAuthorizedAsync(int userId, int requestId)
        {
            var booking = await _unitOfWork.BookingRepository.GetAsync(b => b.Id == requestId, b => b.PartialField.Field);

            if (booking == null)
            {
                return false;
            }

            return booking.PartialField.Field.OwnerId == userId;
        }

        public async Task<bool> IsAuthorizedStaffAsync(int userId, int requestId)
        {
            var booking = await _unitOfWork.BookingRepository.GetAsync(b => b.Id == requestId, b => b.PartialField.Field.Staffs);

            if (booking == null)
            {
                return false;
            }

            return booking.PartialField.Field.Staffs.Any(s => s.UserId == userId && s.IsActive == true);
        }
    }
}