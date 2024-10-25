using MatchFinder.Application.Authorize.Attributes.Report;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/reports")]
    [ApiController]
    public class ReportController : BaseApiController
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] GetListReportRequest request)
        {
            RepositoryPaginationResponse<ReportResponse> result = new RepositoryPaginationResponse<ReportResponse>();
            if (HttpContext.User.IsInRole("Admin"))
            {
                result = await _reportService.GetAllReport(request);
            }
            else
            {
                result = await _reportService.GetMyReport(UserID, request);
            }

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get reports successfully",
                Data = result.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = result.Total
                }
            });
        }

        [ReportAuthorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _reportService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get report successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateReportAsync([FromBody] CreateReportRequest request)
        {
            var reportsResponse = await _reportService.CreateReportAsync(request, UserID);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "report created successfully",
                Data = reportsResponse
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatereportsAsync(int id, [FromBody] UpdateReportRequest request)
        {
            var reportsResponse = await _reportService.UpdateReportAsync(id, request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "report updated successfully",
                Data = reportsResponse
            });
        }
    }
}