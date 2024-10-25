namespace MatchFinder.Application.Models.Responses
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class CurrentUserResponse
    {
        public int Id { get; set; }
        public string RoleName { get; set; }
        public string Status { get; set; }
    }

    public class UserViewFromOther
    {
        public string UserName { get; set; }
        public string Status { get; set; }
    }
}