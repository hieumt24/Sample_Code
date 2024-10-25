using MatchFinder.Application.Authorize.Filters.InactiveTime;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.Application.Authorize.Attributes.InactiveTime
{
    public class InactiveTimeAuthorizeAttribute : TypeFilterAttribute
    {
        public InactiveTimeAuthorizeAttribute() : base(typeof(InactiveTimeAuthorizeFilter))
        {
        }
    }
}