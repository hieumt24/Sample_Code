using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;

namespace MatchFinder.Application.Services
{
    public interface IAuthService
    {
        Task<UserResponse> RegisterAsync(string email, string password, string confirmPassword, string username, string phoneNumber, int role);

        Task<(string token, string refreshToken, UserResponse response)> LoginAsync(string email, string password);

        Task<(string token, string refreshToken)> RefreshTokenAsync(string refreshToken, int userId);

        Task<int> LogoutAsync(int userId);

        Task<int> ResetPasswordAsync(string email, string newPassword, string confirmPassword);

        Task<int> ChangePasswordAsync(string email, string oldPassword, string newPassword, string confirmPassword);

        Task<int> NewPasswordAsync(string email, string newPassword, string confirmPassword);

        Task<bool> VerifyOTPAsync(User user, string token);

        Task<CurrentUserResponse> GetCurrentUser(int userId);
    }
}