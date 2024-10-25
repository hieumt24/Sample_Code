namespace MatchFinder.Application.Models.Requests
{
    public class RefreshTokenRequest
    {
        public int userId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}