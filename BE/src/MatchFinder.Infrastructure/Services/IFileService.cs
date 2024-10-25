using Microsoft.AspNetCore.Http;

namespace MatchFinder.Infrastructure.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);

        bool IsImageFile(IFormFile file);
    }
}