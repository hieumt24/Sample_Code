using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Models;
using System.Linq.Expressions;
using static MatchFinder.Application.Models.Requests.UserRequest;

namespace MatchFinder.Application.Services
{
    public interface IUserService
    {
        Task<User> GetUserByEmailAsync(string email);

        Task<User> GetUserVerificationAsync(Expression<Func<User, bool>> expression);

        Task<int> ActiveAccount(string email);

        Task<RepositoryPaginationResponse<UserResponse>> SearchAsync(UserFilterRequest request);

        Task<UserResponse> GetDetailUserFullField(int id);

        Task<UserViewFromOther> GetUserByIdAsync(int id);

        Task<UserViewFromOther> UpdateStatusUser(UserChangeStatusRequest request);

        Task<UserResponse> UpdateUserAsync(int userId, UserUpdateRequest request);
    }
}