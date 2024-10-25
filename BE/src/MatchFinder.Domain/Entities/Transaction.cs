using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Transaction : BaseEntity
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string? PaymentLink { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int? ReciverId { get; set; }
        public User? Reciver { get; set; }
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
    }
}