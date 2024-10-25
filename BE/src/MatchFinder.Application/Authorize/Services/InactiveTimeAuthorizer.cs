using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Authorize.Services
{
    public class InactiveTimeAuthorizer : IInactiveTimeAuthorizer
    {
        private readonly IUnitOfWork _unitOfWork;

        public InactiveTimeAuthorizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsAuthorizedAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.FieldRepository.GetAllIncludingDeletedAsync(f => f.OwnerId == userId
                                                                                            , i => i.InactiveTimes);
            return ownerField.Any(b => b.InactiveTimes.Any(i => i.Id == requestId));
        }

        public async Task<bool> IsAuthorizedByFieldAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.FieldRepository.GetAllIncludingDeletedAsync(f => f.OwnerId == userId,
                                                                                            i => i.InactiveTimes);
            return ownerField.Any(b => b.Id == requestId);
        }

        public async Task<bool> IsAuthorizedStaffAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.FieldRepository.GetAllIncludingDeletedAsync(f => f.Staffs.Any(i => i.UserId == userId && i.IsActive == true)
                                                                                            , i => i.InactiveTimes);
            return ownerField.Any(b => b.InactiveTimes.Any(i => i.Id == requestId));
        }

        public async Task<bool> IsAuthorizedStaffByFieldAsync(int userId, int requestId)
        {
            var ownerField = await _unitOfWork.FieldRepository.GetAllIncludingDeletedAsync(f => f.Staffs.Any(i => i.UserId == userId && i.IsActive == true)
                                                                                            , i => i.InactiveTimes);
            return ownerField.Any(b => b.Id == requestId);
        }
    }
}