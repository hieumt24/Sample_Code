using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Web;
using static MatchFinder.Application.Models.Requests.UserRequest;

namespace MatchFinder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/auths")]
    public class AuthController : BaseApiController
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IVerificationService _verificationService;

        public AuthController(IAuthService authService, IEmailService emailService, IUserService userService, IVerificationService verificationService)
        {
            _authService = authService;
            _emailService = emailService;
            _userService = userService;
            _verificationService = verificationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserRegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password,
                request.ConfirmPassword, request.UserName, request.PhoneNumber, request.Role);
            var response = new GeneralCreateResponse
            {
                Message = "User registered successfully",
                Data = result
            };
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginRequest request)
        {
            var (token, refreshToken, user) = await _authService.LoginAsync(request.Email, request.Password);
            var response = new GeneralGetResponse
            {
                Message = "User logged in successfully",
                Data = new { token, refreshToken, user }
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            if (await _authService.LogoutAsync(UserID) == 0)
            {
                throw new ConflictException("User not found");
            }
            var response = new GeneralBoolResponse
            {
                message = "User logged out successfully"
            };
            return Ok(response);
        }

        [HttpGet("request-reset-password/{email}")]
        public async Task<IActionResult> RequestResetPasswordAsync(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            var isSuccess = await _emailService.SendEmailAsync(user.Email, EmailConstants.SUBJECT_RESET_PASSWORD, EmailConstants.BodyResetPasswordEmail(email));
            if (!isSuccess)
            {
                throw new ConflictException("Failed to send email");
            }
            var response = new GeneralBoolResponse
            {
                message = "Reset password email sent successfully"
            };
            return Ok(response);
        }

        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPasswordAsync([FromBody] ForgotPasswordRequest request)
        {
            string email = request.Email;
            var user = await _userService.GetUserByEmailAsync(email);

            var otp = await _verificationService.GenerateTokenAsync(user.Id);
            var isSuccess = await _emailService.SendEmailAsync(user.Email, EmailConstants.SUBJECT_FORGET_PASSWORD, EmailConstants.BodyForgetPasswordEmail(HttpUtility.UrlEncode(otp.TokenHash), user.Id));
            var response = new GeneralBoolResponse
            {
                message = "Forget password email sent successfully"
            };
            return Ok(response);
        }

        [HttpGet("verify-token")]
        public async Task<IActionResult> VerifyToken(string token, int userId)
        {
            var user = await _userService.GetUserVerificationAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var result = await _authService.VerifyOTPAsync(user, token);

            return Ok(new GeneralBoolResponse
            {
                success = true,
                message = "OTP verified!",
            });
        }

        [HttpPost("active-account")]
        public async Task<IActionResult> ActiveAccount([FromBody] ActiveAccountRequest request)
        {
            var user = await _userService.GetUserVerificationAsync(u => u.Id == request.UserId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var result = await _authService.VerifyOTPAsync(user, request.Token);

            return Ok(new GeneralBoolResponse
            {
                success = true,
                message = "OTP verified!",
            });
        }

        [HttpPost("new-password")]
        public async Task<IActionResult> NewPasswordAsync([FromBody] UserNewPasswordRequest request)
        {
            await _authService.NewPasswordAsync(request.Email, request.NewPassword, request.ConfirmPassword);
            var response = new GeneralBoolResponse
            {
                success = true,
                message = "Password changed successfully",
            };
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] UserResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request.Email, request.Password, request.ConfirmPassword);
            var response = new GeneralBoolResponse
            {
                message = "Password reset successfully",
            };
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePasswordAsync([FromBody] UserChangePasswordRequest request)
        {
            await _authService.ChangePasswordAsync(request.Email, request.OldPassword, request.NewPassword, request.ConfirmPassword);
            var response = new GeneralBoolResponse
            {
                message = "Password changed successfully",
            };
            return Ok(response);
        }

        [HttpGet("request-active-account/{email}")]
        public async Task<IActionResult> RequestActiveAccountAsync(string email)
        {
            var user = await _userService.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            var isSuccess = await _emailService.SendEmailAsync(user.Email, EmailConstants.SUBJECT_ACTIVE_ACCOUNT, EmailConstants.BodyActivationEmail(email));
            if (!isSuccess)
            {
                throw new ConflictException("Failed to send email");
            }
            var response = new GeneralBoolResponse
            {
                message = "Active account email sent successfully"
            };
            return Ok(response);
        }

        [HttpGet("active-account/{email}")]
        public async Task<IActionResult> ActiveAccountAsync(string email)
        {
            if (await _userService.ActiveAccount(email) == 0)
            {
                throw new ConflictException("Failed to active account");
            }
            var response = new GeneralBoolResponse
            {
                message = "Account activated successfully"
            };
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenRequest refreshTokenRequest)
        {
            var (token, refreshToken) = await _authService.RefreshTokenAsync(refreshTokenRequest.RefreshToken, refreshTokenRequest.userId);
            var response = new GeneralGetResponse
            {
                Message = "Token refreshed successfully",
                Data = new { token, refreshToken }
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("me")]
        public async Task<IActionResult> MeAsync()
        {
            var user = await _authService.GetCurrentUser(UserID);
            var response = new GeneralGetResponse
            {
                Message = "Get user successfully",
                Data = user
            };
            return Ok(response);
        }
    }
}