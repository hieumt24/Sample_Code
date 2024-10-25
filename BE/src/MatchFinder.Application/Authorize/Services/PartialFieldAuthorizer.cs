using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Authorize.Services
{
    public class PartialFieldAuthorizer : IPartialFieldAuthorizer
    {
        private readonly IUnitOfWork _unitOfWork;

        public PartialFieldAuthorizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsAuthorizedAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.PartialFieldRepository.GetAllIncludingDeletedAsync(b => b.Field.OwnerId == userId);
            return ownerField.Any(b => b.Id == requestId);
        }

        public async Task<bool> IsAuthorizedStaffAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.PartialFieldRepository.GetAllIncludingDeletedAsync(b => b.Field.Staffs.Any(s => s.UserId == userId && s.IsActive == true));
            return ownerField.Any(b => b.Id == requestId);
        }
    }
}