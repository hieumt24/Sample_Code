using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class User : BaseEntity
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string PasswordSalt { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public decimal? Amount { get; set; } = 0;
        public UserStatus Status { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public ICollection<Staff> Staffs { get; set; }
        public ICollection<OpponentFinding> OpponentFindings { get; set; }
        public ICollection<OpponentFindingRequest> OpponentFindingRequests { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Verification> Verifications { get; set; }
        public ICollection<TeamMember> TeamMembers { get; set; }
        public ICollection<NotificationUser> NotificationUsers { get; set; }
        public ICollection<Field>? Fields { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<FavoriteField> FavoriteFields { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
    }
}