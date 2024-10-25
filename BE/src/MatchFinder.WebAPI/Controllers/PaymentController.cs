using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPayOSPaymentService _payOSPaymentService;
        private readonly ITransactionService _transactionService;

        public PaymentController(IPayOSPaymentService payOSPaymentService, ITransactionService transactionService)
        {
            _payOSPaymentService = payOSPaymentService;
            _transactionService = transactionService;
        }

        //[HttpPost("create")]
        //public async Task<IActionResult> CreatePaymentLink()
        //{
        //    var booking = new Booking()
        //    {
        //        Id = 1
        //    };
        //    var paymentLink = await _payOSPaymentService.createPaymentLink(booking);

        //    return Ok(new GeneralGetResponse
        //    {
        //        Success = true,
        //        Message = "Create payment link successfully",
        //        Data = new
        //        {
        //            paymentUrl = paymentLink
        //        }
        //    });
        //}

        [HttpPost("confirm-webhook")]
        public async Task<IActionResult> ConfirmWebhook([FromBody] WebhookConfirmRequest request)
        {
            await _payOSPaymentService.confirmWebhook(request.WebhookUrl);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Confirm Webhook successfully"
            });
        }

        [HttpPost("payos-transfer-handler")]
        public async Task<IActionResult> PayOSTransferHandler(WebhookType body)
        {
            await _transactionService.VerifyPaymentWebhookData(body);

            return Ok(new GeneralGetResponse
            {
                Success = true
            });
        }
    }
}