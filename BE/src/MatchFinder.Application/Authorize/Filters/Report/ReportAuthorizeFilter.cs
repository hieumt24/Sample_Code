using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchFinder.Application.Authorize.Filters.Report
{
    public class ReportAuthorizeFilter : BaseAuthorizeFilter
    {
        private readonly IReportAuthorizer _authorizer;

        public ReportAuthorizeFilter(IUserAuthenticator userAuthenticator, IRequestIdExtractor requestIdExtractor, IReportAuthorizer reportAuthorizer)
            : base(userAuthenticator, requestIdExtractor)
        {
            _authorizer = reportAuthorizer;
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

            if (!await _authorizer.IsAuthorizedAsync(userId, requestId))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}