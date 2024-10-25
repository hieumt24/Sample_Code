using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;

namespace MatchFinder.Application.Services
{
    public interface IImageService
    {
        Task<IEnumerable<ImageResponse>> GetListByFieldIdAsync(int fieldId);

        Task<ImageResponse> GetByIdAsync(int id);

        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<ImageResponse>> UploadAsync(ImageCreateRequest request);
    }
}