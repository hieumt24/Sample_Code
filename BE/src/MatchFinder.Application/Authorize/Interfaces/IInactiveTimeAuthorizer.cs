namespace MatchFinder.Application.Authorize.Interfaces
{
    public interface IInactiveTimeAuthorizer
    {
        Task<bool> IsAuthorizedAsync(int userId, int requestId);

        Task<bool> IsAuthorizedStaffAsync(int userId, int requestId);

        Task<bool> IsAuthorizedByFieldAsync(int userId, int requestId);

        Task<bool> IsAuthorizedStaffByFieldAsync(int userId, int requestId);
    }
}