namespace MatchFinder.Domain.Models
{
    public class BaseEntity
    {
        public DateTime? CreatedAt { get; set; } = DateTime.Now.AddHours(7);
        public string? CreatedBy { get; set; }
        public DateTime? LastUpdatedAt { get; set; } = null;
        public string? LastUpdatedBy { get; set; } = null;
        public bool? IsDeleted { get; set; } = false;
    }
}