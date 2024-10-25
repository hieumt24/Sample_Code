using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface ISlotService
    {
        Task<SlotResponse> CreateSlotAsync(CreateSlotRequest request);

        Task<SlotResponse> UpdateSlotAsync(UpdateSlotRequest request);

        Task SoftDeleteSlotAsync(int id);

        Task<SlotResponse> GetByIdAsync(int id);

        Task<RepositoryPaginationResponse<SlotResponse>> GetListAsync(int fid, GetListSlotRequest pagination);
    }
}