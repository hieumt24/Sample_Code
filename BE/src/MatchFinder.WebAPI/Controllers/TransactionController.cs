using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Authorize]
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : BaseApiController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await _transactionService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get transaction successfully",
                Data = result
            });
        }

        [HttpGet("amount")]
        public async Task<IActionResult> GetAmountAsync()
        {
            var result = await _transactionService.GetAmountAsync(UserID);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get amount successfully",
                Data = result
            });
        }

        [HttpGet("available-balance")]
        public async Task<IActionResult> GetAvailableBalance()
        {
            var result = await _transactionService.GetAvailableBalanceAsync(UserID);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get amount successfully",
                Data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetListTransactionAsync([FromQuery] GetTransactionsRequest request)
        {
            var result = await _transactionService.GetListAsync(UserID, request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get transactions successfully",
                Data = result.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = result.Total
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> Recharge([FromBody] RechargeRequest request)
        {
            var result = await _transactionService.RechargeAsync(UserID, request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Create payment link successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpPut("{id}/cancel-paymentlink")]
        public async Task<IActionResult> CancelPaymentLink(int id)
        {
            var result = await _transactionService.CancelPaymentLink(UserID, id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Cancel payment link successfully",
                Data = result
            });
        }
    }
}