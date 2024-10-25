using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class FieldRevenue : BaseEntity
    {
        public int Id { get; set; }
        public decimal AmountPerHour { get; set; }
        public DateTime Period { get; set; }
        public int PartialFieldId { get; set; }
        public PartialField PartialField { get; set; }
    }
}