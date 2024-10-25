using MatchFinder.Application.Authorize.Interfaces;
using System.Security.Claims;

namespace MatchFinder.Application.Services
{
    public class UserAuthenticator : IUserAuthenticator
    {
        public bool IsAuthenticated(ClaimsPrincipal user, out int userId)
        {
            userId = 0;
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out userId))
            {
                return false;
            }
            return true;
        }
    }
}