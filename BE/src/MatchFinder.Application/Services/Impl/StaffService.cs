using AutoMapper;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Helpers;

namespace MatchFinder.Application.Services.Impl
{
    public class StaffService : IStaffService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICryptographyHelper _cryptographyHelper;

        public StaffService(IMapper mapper, IUnitOfWork unitOfWork, ICryptographyHelper cryptographyHelper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cryptographyHelper = cryptographyHelper;
        }

        public async Task<RepositoryPaginationResponse<StaffResponse>> GetStaffsAsync(int oid, GetStaffsRequest request)
        {
            var result = await _unitOfWork.StaffRepository.GetListAsync(s => s.Field.OwnerId == oid
                                                                        && (request.FieldId == null || s.FieldId == request.FieldId)
                                                                        && (String.IsNullOrEmpty(request.UserName) || s.User.UserName.ToLower().Contains(request.UserName.ToLower()))
                                                                        && (String.IsNullOrEmpty(request.Name) || s.Name.ToLower().Contains(request.Name.ToLower()))
                                                                        && (request.IsActive == null || s.IsActive == request.IsActive),
                                                                        request.Limit,
                                                                        request.Offset,
                                                                        u => u.User,
                                                                        f => f.Field);
            return new RepositoryPaginationResponse<StaffResponse>
            {
                Data = _mapper.Map<IEnumerable<StaffResponse>>(result.Data),
                Total = result.Total
            };
        }

        public async Task<StaffResponse> CreateStaffAsync(int oid, CreateStaffRequest request)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(f => f.Id == request.FieldId && f.OwnerId == oid);
            if (field == null)
                throw new ConflictException("Không có quyền truy cập vào sân này!");

            var userNameExisted = await _unitOfWork.UserRepository.GetAsync(u => u.UserName == request.UserName);
            if (userNameExisted != null)
            {
                throw new ConflictException("Tài khoản đã tồn tại!");
            }
            var salt = _cryptographyHelper.GenerateSalt();
            var hashedPassword = _cryptographyHelper.HashPassword(request.Password, salt);

            var user = new User
            {
                Email = string.Empty,
                UserName = request.UserName,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Status = UserStatus.ACTIVE,
                RoleId = 4,
                PhoneNumber = request.PhoneNumber,
            };

            Staff staff = new Staff();
            staff.IsActive = request.IsActive;
            staff.Field = field;
            staff.User = user;
            staff.Name = request.Name;

            await _unitOfWork.UserRepository.AddAsync(user);
            await _unitOfWork.StaffRepository.AddAsync(staff);

            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<StaffResponse>(staff);
            }
            throw new ConflictException("Tạo tài khoản không thành công!");
        }

        public async Task<StaffResponse> UpdateStaffAsync(int oid, UpdateStaffRequest request)
        {
            var result = await _unitOfWork.StaffRepository.GetAsync(s => s.Field.OwnerId == oid
                                                                        && s.FieldId == request.FieldId
                                                                        && s.UserId == request.UserId,
                                                                        u => u.User,
                                                                        f => f.Field);

            if (result == null)
                throw new NotFoundException("Bạn không có nhân viên này!");

            result.IsActive = request.IsActive;

            _unitOfWork.StaffRepository.Update(result);

            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<StaffResponse>(result);
            }

            throw new ConflictException("Request failed!");
        }

        public async Task<StaffResponse> GetStaffAsync(int oid, int uid, int fid)
        {
            var result = await _unitOfWork.StaffRepository.GetAsync(s => s.Field.OwnerId == oid
                                                                        && s.FieldId == fid
                                                                        && s.UserId == uid,
                                                                        u => u.User,
                                                                        f => f.Field);
            return _mapper.Map<StaffResponse>(result);
        }
    }
}