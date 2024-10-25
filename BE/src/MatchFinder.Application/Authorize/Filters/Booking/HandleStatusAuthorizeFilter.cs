using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchFinder.Application.Authorize.Filters
{
    public class HandleStatusAuthorizeFilter : BaseAuthorizeFilter
    {
        private readonly IFieldOwnershipAuthorizer _fieldOwnershipAuthorizer;

        public HandleStatusAuthorizeFilter(IUserAuthenticator userAuthenticator, IRequestIdExtractor requestIdExtractor, IFieldOwnershipAuthorizer fieldOwnershipAuthorizer)
            : base(userAuthenticator, requestIdExtractor)
        {
            _fieldOwnershipAuthorizer = fieldOwnershipAuthorizer;
        }

        public override async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var (isValid, userId, requestId) = await ValidateRequestAsync(context);
            if (!isValid) return;

            var user = context.HttpContext.User;
            if (user.IsInRole("Admin"))
            {
                return;
            }
            else if (user.IsInRole("Staff"))
            {
                if (!await _fieldOwnershipAuthorizer.IsAuthorizedStaffAsync(userId, requestId))
                {
                    context.Result = new ForbidResult();
                }
                return;
            }

            if (!await _fieldOwnershipAuthorizer.IsAuthorizedAsync(userId, requestId))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}