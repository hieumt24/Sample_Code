namespace MatchFinder.Application.Authorize.Interfaces
{
    public interface IFieldAuthorizer
    {
        Task<bool> IsAuthorizedAsync(int userId, int requestId);

        Task<bool> IsAuthorizedStaffAsync(int userId, int requestId);
    }
}