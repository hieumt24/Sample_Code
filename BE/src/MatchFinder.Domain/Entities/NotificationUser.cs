using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class NotificationUser : BaseEntity
    {
        public int NotificationId { get; set; }
        public Notification Notification { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public bool IsRead { get; set; } = false;
    }
}