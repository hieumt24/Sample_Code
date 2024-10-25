using MatchFinder.Application.Authorize.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MatchFinder.Application.Authorize.Filters
{
    public class BookingAuthorizeFilter : BaseAuthorizeFilter
    {
        private readonly IBookingAuthorizer _bookingAuthorizer;

        public BookingAuthorizeFilter(IUserAuthenticator userAuthenticator, IRequestIdExtractor requestIdExtractor, IBookingAuthorizer bookingAuthorizer)
            : base(userAuthenticator, requestIdExtractor)
        {
            _bookingAuthorizer = bookingAuthorizer;
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

            if (!await _bookingAuthorizer.IsAuthorizedAsync(userId, requestId))
            {
                context.Result = new ForbidResult();
            }
        }
    }
}