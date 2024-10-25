using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public int Id { get; set; }
        public string TokenHash { get; set; }
        public string TokenSalt { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ExpireAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}