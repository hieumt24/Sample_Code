using MatchFinder.Application.Authorize.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.Application.Authorize.Attributes
{
    public class BookingAuthorizeAttribute : TypeFilterAttribute
    {
        public BookingAuthorizeAttribute() : base(typeof(BookingAuthorizeFilter))
        {
        }
    }
}