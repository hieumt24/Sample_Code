using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public ICollection<NotificationUser> NotificationUsers { get; set; }
    }
}