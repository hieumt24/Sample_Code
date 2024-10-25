using AutoMapper;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using System.Linq.Expressions;
using MatchFinder.Domain.Constants;
using static MatchFinder.Application.Models.Requests.UserRequest;

namespace MatchFinder.Application.Services.Impl
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileService = fileService;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email, u => u.Role);
            if (user == null)
                throw new NotFoundException("User not found");
            return user;
        }

        public async Task<User> GetUserVerificationAsync(Expression<Func<User, bool>> expression)
        {
            return await _unitOfWork.UserRepository.GetAsync(expression, u => u.Verifications);
        }

        public async Task<int> ActiveAccount(string email)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Email == email);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Status = UserStatus.ACTIVE;
            _unitOfWork.UserRepository.Update(user);
            return await _unitOfWork.CommitAsync();
        }

        public async Task<RepositoryPaginationResponse<UserResponse>> SearchAsync(UserFilterRequest request)
        {
            var users = await _unitOfWork.UserRepository
                .GetListAsync(u =>
                    (u.Role.Id == RoleConstant.PLAYER_ID) &&
                    (string.IsNullOrEmpty(request.Email) || u.Email.Contains(request.Email)) &&
                    (string.IsNullOrEmpty(request.UserName) || u.UserName.Contains(request.UserName)) &&
                    (string.IsNullOrEmpty(request.PhoneNumber) || u.PhoneNumber.Contains(request.PhoneNumber)) &&
                    (string.IsNullOrEmpty(request.Status) || u.Status.ToString() == request.Status) &&
                    (!request.FromCreateDate.HasValue || u.CreatedAt >= request.FromCreateDate.Value.ToDateTime(TimeOnly.MinValue)) &&
                    (!request.ToCreateDate.HasValue || u.CreatedAt < request.ToCreateDate.Value.ToDateTime(TimeOnly.MaxValue)),
                    request.Limit, request.Offset,
                    u => u.Role);

            return new RepositoryPaginationResponse<UserResponse>
            {
                Data = _mapper.Map<IEnumerable<UserResponse>>(users.Data),
                Total = users.Total
            };
        }

        public async Task<UserResponse> GetDetailUserFullField(int id)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == id, u => u.Role);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return _mapper.Map<UserResponse>(user);
        }

        public async Task<UserViewFromOther> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            return new UserViewFromOther
            {
                UserName = user.UserName,
                Status = user.Status.ToString()
            };
        }

        public async Task<UserViewFromOther> UpdateStatusUser(UserChangeStatusRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == request.Id);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            user.Status = Enum.Parse<UserStatus>(request.Status, true);
            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CommitAsync();

            return new UserViewFromOther
            {
                UserName = user.UserName,
                Status = user.Status.ToString()
            };
        }

        public async Task<UserResponse> UpdateUserAsync(int userId, UserUpdateRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("User not found");
            }

            var urlImage = string.Empty;
            if (request.Avatar != null && _fileService.IsImageFile(request.Avatar))
            {
                urlImage = await _fileService.SaveFileAsync(request.Avatar);
            }

            user.UserName = request.UserName ?? user.UserName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Avatar = string.IsNullOrEmpty(urlImage) ? user.Avatar : urlImage;

            _unitOfWork.UserRepository.Update(user);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<UserResponse>(user);
        }
    }
}