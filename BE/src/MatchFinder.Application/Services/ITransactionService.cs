using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;
using Net.payOS.Types;

namespace MatchFinder.Application.Services
{
    public interface ITransactionService
    {
        Task<decimal> GetAmountAsync(int uid);

        Task<decimal> GetAvailableBalanceAsync(int uid);

        Task<TransactionResponse> GetByIdAsync(int id);

        Task<RepositoryPaginationResponse<TransactionResponse>> GetListAsync(int uid, GetTransactionsRequest request);

        Task<string> RechargeAsync(int uid, RechargeRequest request);

        Task<TransactionResponse> DebtPaymentAsync(int uid, DebtPaymentRequest request);

        Task VerifyPaymentWebhookData(WebhookType hook);

        Task<TransactionResponse> CancelPaymentLink(int userId, int transactionId);
    }
}