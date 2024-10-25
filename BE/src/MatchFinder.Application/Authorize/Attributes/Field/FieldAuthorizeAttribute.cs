using MatchFinder.Application.Authorize.Filters.Field;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.Application.Authorize.Attributes.Field
{
    public class FieldAuthorizeAttribute : TypeFilterAttribute
    {
        public FieldAuthorizeAttribute() : base(typeof(FieldAuthorizeFilter))
        {
        }
    }
}