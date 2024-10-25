using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MatchFinder.Application.Authorize.Interfaces
{
    public interface IUserAuthenticator
    {
        bool IsAuthenticated(ClaimsPrincipal user, out int userId);
    }

    public interface IRequestIdExtractor
    {
        Task<string> ExtractRequestIdAsync(HttpRequest request);
    }
}