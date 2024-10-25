using MatchFinder.Application.Authorize.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.Application.Authorize.Attributes
{
    public class HandleStatusAuthorizeAttribute : TypeFilterAttribute
    {
        public HandleStatusAuthorizeAttribute() : base(typeof(HandleStatusAuthorizeFilter))
        {
        }
    }
}