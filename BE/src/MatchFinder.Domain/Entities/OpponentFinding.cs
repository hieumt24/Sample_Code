using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class OpponentFinding : BaseEntity
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int UserFindingId { get; set; }
        public User UserFinding { get; set; }
        public int? FieldId { get; set; }
        public Field? Field { get; set; }
        public int? BookingId { get; set; }
        public Booking? Booking { get; set; }
        public string Status { get; set; }
        public string? FieldName { get; set; }
        public string? FieldAddress { get; set; }
        public string? FieldProvince { get; set; }
        public string? FieldDistrict { get; set; }
        public string? FieldCommune { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public DateOnly? Date { get; set; }
        public bool IsOverdue { get; set; } = false;
        public ICollection<OpponentFindingRequest> OpponentFindingRequests { get; set; }
    }
}