using AutoMapper;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.Helpers;
using MatchFinder.Infrastructure.Services;
using System.Web;

namespace MatchFinder.Application.Services.Impl
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly ICryptographyHelper _cryptographyHelper;
        private readonly IEmailService _emailService;
        private readonly IVerificationService _verificationService;
        private readonly IMapper _mapper;

        public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, ICryptographyHelper cryptographyHelper, IEmailService emailService, IVerificationService verificationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _cryptographyHelper = cryptographyHelper;
            _emailService = emailService;
            _verificationService = verificationService;
            _mapper = mapper;
        }

        public async Task<UserResponse> RegisterAsync(string email, string password, string confirmPassword, string username, string phoneNumber, int role)
        {
            var emailExisted = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email || u.UserName == username);
            if (emailExisted != null)
            {
                throw new ConflictException("Tên đăng nhập hoặc email đã tồn tại, hãy thử tên hoặc email khác!");
            }

            var salt = _cryptographyHelper.GenerateSalt();
            var hashedPassword = _cryptographyHelper.HashPassword(password, salt);

            var user = new User
            {
                Email = email,
                UserName = username,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Status = UserStatus.IN_ACTIVE,
                RoleId = role == RoleConstant.FIELD_ID ? role : (RoleConstant.STAFF == role ? role : RoleConstant.PLAYER_ID),
                PhoneNumber = phoneNumber,
            };

            await _unitOfWork.UserRepository.AddAsync(user);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                var otp = await _verificationService.GenerateTokenAsync(user.Id);
                var isSuccess = await _emailService.SendEmailAsync(user.Email, EmailConstants.SUBJECT_ACTIVE_ACCOUNT,
                                                                    EmailConstants.BodyActivationEmail(HttpUtility.UrlEncode(otp.TokenHash), user.Id));
                if (!isSuccess)
                {
                    throw new ConflictException("Failed to send email");
                }
            }
            return _mapper.Map<UserResponse>(user);
        }

        public async Task<(string token, string refreshToken, UserResponse response)> LoginAsync(string email, string password)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => (u.Email == email || u.UserName == email) && !u.Staffs.Any(s => s.IsActive == false), u => u.Role);
            if (user == null || !_cryptographyHelper.VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
            {
                throw new ConflictException("Email, username or password incorrect!!!");
            }

            if (user.Status != UserStatus.ACTIVE)
            {
                throw new ConflictException("Account is not activated");
            }

            var token = _tokenService.GenerateToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            refreshToken.UserId = user.Id;

            await _unitOfWork.RefreshTokenRepository.AddAsync(refreshToken);
            await _unitOfWork.CommitAsync();

            return (token, refreshToken.TokenHash, _mapper.Map<UserResponse>(user));
        }

        public async Task<(string token, string refreshToken)> RefreshTokenAsync(string refreshToken, int userId)
        {
            var token = await _unitOfWork.RefreshTokenRepository.GetAsync(rt => rt.TokenHash == refreshToken && rt.UserId == userId)
                .ConfigureAwait(false);
            if (token == null || token.ExpireAt <= DateTime.UtcNow)
            {
                throw new ConflictException("Invalid refresh token");
            }

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == token.UserId, u => u.Role);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var newJwtToken = _tokenService.GenerateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            newRefreshToken.UserId = user.Id;

            await _unitOfWork.RefreshTokenRepository.AddAsync(newRefreshToken);
            await _unitOfWork.CommitAsync();

            return (newJwtToken, newRefreshToken.TokenHash);
        }

        public async Task<int> LogoutAsync(int userId)
        {
            var tokens = await _unitOfWork.RefreshTokenRepository.GetAllAsync(rt => rt.UserId == userId);
            //_unitOfWork.RefreshTokenRepository.RemoveRange(tokens);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> ResetPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                throw new ConflictException("Passwords do not match");
            }
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var passwordSalt = _cryptographyHelper.GenerateSalt();
            var passwordHash = _cryptographyHelper.HashPassword(newPassword, passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _unitOfWork.UserRepository.Update(user);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> ChangePasswordAsync(string email, string oldPassword, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                throw new ConflictException("Passwords do not match");
            }
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            if (!_cryptographyHelper.VerifyPassword(oldPassword, user.PasswordHash, user.PasswordSalt))
            {
                throw new ConflictException("Old password is incorrect");
            }

            var passwordSalt = _cryptographyHelper.GenerateSalt();
            var passwordHash = _cryptographyHelper.HashPassword(newPassword, passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;

            _unitOfWork.UserRepository.Update(user);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<int> NewPasswordAsync(string email, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
                throw new ConflictException("Passwords do not match");

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email, v => v.Verifications);
            if (user == null)
                throw new NotFoundException("User not found");

            var verify = user.Verifications
                 .Where(v => v.ExpireAt > DateTime.UtcNow && v.IsVerified)
                 .OrderByDescending(v => v.CreatedAt)
                 .FirstOrDefault();

            if (verify == null)
                throw new ConflictException("Account don't verified");

            var passwordSalt = _cryptographyHelper.GenerateSalt();
            var passwordHash = _cryptographyHelper.HashPassword(newPassword, passwordSalt);

            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            verify.IsDeleted = true;
            _unitOfWork.VerificationRepository.Update(verify);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<bool> VerifyOTPAsync(User user, string token)
        {
            if (string.IsNullOrEmpty(token))
                throw new ConflictException("Không tìm thấy mã, vui lòng thử lại");

            var verify = user.Verifications
                 .Where(v => v.ExpireAt > DateTime.UtcNow)
                 .OrderByDescending(v => v.CreatedAt)
                 .FirstOrDefault();

            if (verify == null)
                throw new ConflictException("Mã này đã hết hạn, vui lòng thử ");

            bool isValid = token == verify.TokenHash;
            if (!isValid)
                throw new ConflictException("Không tìm thấy mã, vui lòng thử lại");

            verify.IsVerified = true;
            if (await _unitOfWork.CommitAsync() > 0)
            {
                user.Status = UserStatus.ACTIVE;
                await _unitOfWork.CommitAsync();
                return isValid;
            };
            return false;
        }

        public async Task<CurrentUserResponse> GetCurrentUser(int userId)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId, u => u.Role);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }
            return _mapper.Map<CurrentUserResponse>(user);
        }
    }
}