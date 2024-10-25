using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Rate : BaseEntity
    {
        public int Star { get; set; }
        public string Comment { get; set; }
        public int UserId { get; set; }
        public int BookingId { get; set; }
        public int FieldId { get; set; }

        public virtual User Rater { get; set; }
        public virtual Booking Booking { get; set; }
        public virtual Field Field { get; set; }
    }
}