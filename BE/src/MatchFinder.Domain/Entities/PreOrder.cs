using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class PreOrder : BaseEntity
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; }
        public int ItemId { get; set; }
        public Menu Item { get; set; }
    }
}