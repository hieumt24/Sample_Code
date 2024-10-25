using MatchFinder.Infrastructure.Services.Core;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;

namespace MatchFinder.Infrastructure.Services.Impl
{
    public class PayOSPaymentService : IPayOSPaymentService
    {
        private readonly PayOS _payOS;
        private readonly PayOSSettings _payOSSettings;

        public PayOSPaymentService(PayOS payOS, IOptions<PayOSSettings> payOSSettings)
        {
            _payOS = payOS;
            _payOSSettings = payOSSettings.Value;
        }

        public async Task<bool> cancelPaymentLink(int transactionId)
        {
            PaymentLinkInformation paymentLinkInformation = await _payOS.cancelPaymentLink(transactionId);
            if (paymentLinkInformation.status == "CANCELLED")
            {
                return true;
            }
            return false;
        }

        public async Task<string> confirmWebhook(string webhook_url)
        {
            return await _payOS.confirmWebhook(webhook_url);
        }

        public async Task<string> createPaymentLink(MatchFinder.Domain.Entities.Transaction transaction)
        {
            PaymentData paymentData = new PaymentData(transaction.Id, (int)transaction.Amount, transaction.Description, null, _payOSSettings.CancelUrl, _payOSSettings.ReturnUrl);
            CreatePaymentResult createPayment = await _payOS.createPaymentLink(paymentData);
            return createPayment.checkoutUrl;
        }

        public WebhookData verifyPaymentWebhookData(WebhookType hook)
        {
            return _payOS.verifyPaymentWebhookData(hook);
        }
    }
}