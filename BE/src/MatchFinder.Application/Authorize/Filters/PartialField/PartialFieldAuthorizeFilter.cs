using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchFinder.Application.Authorize.Filters.PartialField
{
    public class PartialFieldAuthorizeFilter : BaseAuthorizeFilter
    {
        private readonly IPartialFieldAuthorizer _authorizer;

        public PartialFieldAuthorizeFilter(IUserAuthenticator userAuthenticator, IRequestIdExtractor requestIdExtractor, IPartialFieldAuthorizer partialFieldAuthorizer)
            : base(userAuthenticator, requestIdExtractor)
        {
            _authorizer = partialFieldAuthorizer;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var (isValid, userId, requestId) = await ValidateRequestAsync(context);
            if (!isValid) return;

            var user = context.HttpContext.User;
            var request = context.HttpContext.Request;

            if (user.IsInRole("Admin"))
            {
                return;
            }
            else if (user.IsInRole("Staff") && (HttpMethods.IsPut(request.Method) || HttpMethods.IsGet(request.Method)))
            {
                if (!await _authorizer.IsAuthorizedStaffAsync(userId, requestId))
                {
                    context.Result = new ForbidResult();
                }
                return;
            }

            if (!await _authorizer.IsAuthorizedAsync(userId, requestId))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}