namespace MatchFinder.Application.Authorize.Interfaces
{
    public interface IReportAuthorizer
    {
        Task<bool> IsAuthorizedAsync(int userId, int requestId);
    }
}