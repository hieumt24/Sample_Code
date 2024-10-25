using MatchFinder.Application.Authorize.Filters.PartialField;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.Application.Authorize.Attributes.PartialField
{
    public class PartialFieldAuthorizeAttribute : TypeFilterAttribute
    {
        public PartialFieldAuthorizeAttribute() : base(typeof(PartialFieldAuthorizeFilter))
        {
        }
    }
}