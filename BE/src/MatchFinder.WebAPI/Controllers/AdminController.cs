using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        private readonly IFieldService _fieldService;
        private readonly ITransactionService _transactionService;

        public AdminController(IFieldService fieldService, ITransactionService transactionService)
        {
            _fieldService = fieldService;
            _transactionService = transactionService;
        }

        [HttpGet("fields")]
        public async Task<IActionResult> AllFields([FromQuery] GetFieldsRequest request)
        {
            var fields = await _fieldService.GetAllFieldsAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = fields.Total
                }
            });
        }

        [HttpPost("debt-payment")]
        public async Task<IActionResult> DebtPayment([FromBody] DebtPaymentRequest request)
        {
            var result = await _transactionService.DebtPaymentAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Debt payment successfully",
                Data = result
            });
        }
    }
}