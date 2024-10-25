using MatchFinder.Domain.Entities;

namespace MatchFinder.Application.Services
{
    public interface IVerificationService
    {
        Task<Verification> GenerateTokenAsync(int id);
    }
}