using MatchFinder.Application.Attributes;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class UserRequest
    {
        public class UserResetPasswordRequest
        {
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            [RegularExpression(RegexConstants.PASSWORD, ErrorMessage = ErrorMessage.ERROR_PASSWORD)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Compare("Password", ErrorMessage = ErrorMessage.ERROR_CONFIRM_PASSWORD)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class UserRegisterRequest
        {
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            [RegularExpression(RegexConstants.PASSWORD, ErrorMessage = ErrorMessage.ERROR_PASSWORD)]
            public string Password { get; set; } = string.Empty;

            [Required]
            [Compare("Password", ErrorMessage = ErrorMessage.ERROR_CONFIRM_PASSWORD)]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required]
            public string UserName { get; set; } = string.Empty;

            [Required]
            public string PhoneNumber { get; set; } = string.Empty;

            [Required]
            public int Role { get; set; }
        }

        public class UserNewPasswordRequest
        {
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            [RegularExpression(RegexConstants.PASSWORD, ErrorMessage = ErrorMessage.ERROR_PASSWORD)]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [Compare("NewPassword", ErrorMessage = ErrorMessage.ERROR_CONFIRM_PASSWORD)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class ActiveAccountRequest
        {
            [Required]
            public string Token { get; set; }

            [Required]
            public int UserId { get; set; }
        }

        public class UserLoginRequest
        {
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string Password { get; set; } = string.Empty;
        }

        public class UserChangePasswordRequest
        {
            [EmailAddress]
            [Required]
            public string Email { get; set; } = string.Empty;

            [Required]
            public string OldPassword { get; set; } = string.Empty;

            [Required]
            [RegularExpression(RegexConstants.PASSWORD, ErrorMessage = ErrorMessage.ERROR_PASSWORD)]
            public string NewPassword { get; set; } = string.Empty;

            [Required]
            [Compare("NewPassword", ErrorMessage = ErrorMessage.ERROR_CONFIRM_PASSWORD)]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public class UserFilterRequest : Pagination
        {
            public string? UserName { get; set; }
            public string? Email { get; set; }
            public string? PhoneNumber { get; set; }
            public string? Status { get; set; }
            public DateOnly? FromCreateDate { get; set; } = DateOnly.MinValue;

            [GreaterThanOrEqualTo("FromCreateDate", ErrorMessage = "ToCreateDate must greater than or equal to FromCreateDate")]
            public DateOnly? ToCreateDate { get; set; } = DateOnly.MaxValue;
        }

        public class UserChangeStatusRequest
        {
            [Required]
            public int Id { get; set; }

            [Required]
            [EnumDataType(typeof(UserStatus), ErrorMessage = "Status is valid with value: ACTIVE, IN_ACTIVE or LOCKED")]
            public string Status { get; set; }
        }

        public class UserUpdateRequest
        {
            public string? UserName { get; set; }

            public string? PhoneNumber { get; set; }

            public IFormFile? Avatar { get; set; }
        }
    }
}