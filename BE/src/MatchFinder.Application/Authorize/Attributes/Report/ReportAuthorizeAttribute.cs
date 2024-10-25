using MatchFinder.Application.Authorize.Filters.Report;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.Application.Authorize.Attributes.Report
{
    public class ReportAuthorizeAttribute : TypeFilterAttribute
    {
        public ReportAuthorizeAttribute() : base(typeof(ReportAuthorizeFilter))
        {
        }
    }
}