namespace MatchFinder.Domain.Models
{
    public class RepositoryPaginationResponse<T>
    {
        public IEnumerable<T>? Data { get; set; }
        public int Total { get; set; }
    }
}