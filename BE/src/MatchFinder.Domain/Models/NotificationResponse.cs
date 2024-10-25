namespace MatchFinder.Domain.Models
{
    public class NotificationResponse
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool IsRead { get; set; } = false;
    }
}