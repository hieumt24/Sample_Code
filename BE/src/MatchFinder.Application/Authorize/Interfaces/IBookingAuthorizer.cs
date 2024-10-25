namespace MatchFinder.Application.Authorize.Interfaces
{
    public interface IBookingAuthorizer
    {
        Task<bool> IsAuthorizedAsync(int userId, int requestId);
    }

    public interface IFieldOwnershipAuthorizer
    {
        Task<bool> IsAuthorizedAsync(int userId, int requestId);
        Task<bool> IsAuthorizedStaffAsync(int userId, int requestId);
    }
}