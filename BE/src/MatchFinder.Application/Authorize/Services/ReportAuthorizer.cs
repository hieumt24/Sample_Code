using MatchFinder.Application.Authorize.Interfaces;
using MatchFinder.Domain.Interfaces;

namespace MatchFinder.Application.Authorize.Services
{
    public class ReportAuthorizer : IReportAuthorizer
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReportAuthorizer(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> IsAuthorizedAsync(int userId, int requestId)
        {
            var owner = await _unitOfWork.ReportRepository.GetAllIncludingDeletedAsync(r => r.UserId == userId);
            return owner.Any(b => b.Id == requestId);
        }
    }
}