using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MatchFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        protected int UserID
        {
            get
            {
                var claimValue = FindClaim(ClaimTypes.NameIdentifier);
                if (int.TryParse(claimValue, out int userId))
                {
                    return userId;
                }
                return 0;
            }
        }

        private string FindClaim(string claimName)
        {
            var claimsIdentity = HttpContext.User.Identity as ClaimsIdentity;
            var claim = claimsIdentity?.FindFirst(claimName);
            return claim?.Value;
        }
    }
}