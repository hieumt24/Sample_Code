using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class RechargeRequest
    {
        [Range(10000, 50000000, ErrorMessage = "Minimum deposit amount 10.000vnd, maximum 50.000.000vnd")]
        public int Amount { get; set; }

        public string? Description { get; set; }
    }

    public class DebtPaymentRequest
    {
        [Range(10000, int.MaxValue, ErrorMessage = "Minimum deposit amount 10.000vnd")]
        public int Amount { get; set; }

        public string? Description { get; set; }
        public int ReceiverId { get; set; }
    }

    public class GetTransactionsRequest : Pagination
    {
        public int? FieldId { get; set; }
        public DateTime? Date { get; set; }
    }
}