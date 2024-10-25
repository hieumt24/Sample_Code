using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Authorize.Services
{
    public class FieldAuthorizer : IFieldAuthorizer
    {
        private readonly IUnitOfWork _unitOfWork;

        public FieldAuthorizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsAuthorizedAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.FieldRepository.GetAllIncludingDeletedAsync(b => b.OwnerId == userId);
            return ownerField.Any(b => b.Id == requestId);
        }

        public async Task<bool> IsAuthorizedStaffAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.FieldRepository.GetAllIncludingDeletedAsync(b => b.Staffs.Any(s => s.UserId == userId && s.IsActive == true));
            return ownerField.Any(b => b.Id == requestId);
        }
    }
}