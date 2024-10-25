using Net.payOS.Types;

namespace MatchFinder.Infrastructure.Services
{
    public interface IPayOSPaymentService
    {
        Task<string> createPaymentLink(MatchFinder.Domain.Entities.Transaction transaction);

        Task<bool> cancelPaymentLink(int transactionId);

        Task<string> confirmWebhook(string webhookUrl);

        WebhookData verifyPaymentWebhookData(WebhookType body);
    }
}