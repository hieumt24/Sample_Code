namespace MatchFinder.Application.Models.Responses
{
    public class TransactionResponse
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int? ReciverId { get; set; }
        public string? ReciverName { get; set; }
        public int? BookingId { get; set; }
        public string? PaymentLink { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
    }
}