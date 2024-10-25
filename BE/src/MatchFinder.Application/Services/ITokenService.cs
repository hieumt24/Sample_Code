using MatchFinder.Domain.Entities;

namespace MatchFinder.Application.Services
{
    public interface ITokenService
    {
        string GenerateToken(User user);

        RefreshToken GenerateRefreshToken();
    }
}