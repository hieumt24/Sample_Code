namespace MatchFinder.Domain.Models
{
    public class GeneralGetResponse
    {
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; } = null;
    }

    public class PaginationResponse : GeneralGetResponse
    {
        public Meta? Meta { get; set; }
    }

    public class Meta
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }
}