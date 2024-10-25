namespace MatchFinder.Domain.Models
{
    public class InvalidDataResponse
    {
        public string Message { get; set; } = string.Empty;
        public object? Errors { get; set; } = null;
    }
}