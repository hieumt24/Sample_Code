using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchFinder.Application.Authorize.Filters
{
    public abstract class BaseAuthorizeFilter : IAsyncAuthorizationFilter
    {
        protected readonly IUserAuthenticator _userAuthenticator;
        protected readonly IRequestIdExtractor _requestIdExtractor;

        public BaseAuthorizeFilter(IUserAuthenticator userAuthenticator, IRequestIdExtractor requestIdExtractor)
        {
            _userAuthenticator = userAuthenticator;
            _requestIdExtractor = requestIdExtractor;
        }

        public abstract Task OnAuthorizationAsync(AuthorizationFilterContext context);

        protected async Task<(bool isValid, int userId, int requestId)> ValidateRequestAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!_userAuthenticator.IsAuthenticated(user, out int userId))
            {
                context.Result = new UnauthorizedResult();
                return (false, 0, 0);
            }

            var requestIdString = await _requestIdExtractor.ExtractRequestIdAsync(context.HttpContext.Request);
            if (!int.TryParse(requestIdString, out int requestId))
            {
                context.Result = new BadRequestObjectResult("Invalid requestId format");
                return (false, 0, 0);
            }

            return (true, userId, requestId);
        }
    }
}