using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Application.Services.Impl
{
    public class ReportService : IReportService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public ReportService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<ReportResponse> CreateReportAsync(CreateReportRequest request, int uid)
        {
            if (!Enum.GetNames(typeof(ReportType)).Contains(request.ReportType, StringComparer.OrdinalIgnoreCase))
            {
                throw new ConflictException("Invalid ReportType");
            }

            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid);
            if (user == null)
            {
                throw new NotFoundException("User not exist!");
            }

            var field = await _unitOfWork.FieldRepository.GetAsync(f => f.Id == request.FieldId
                                                                    && EF.Functions.Like(f.Status, FieldStatus.ACCEPTED));
            if (field == null)
            {
                throw new NotFoundException("Field not exist!");
            }

            var report = new Report
            {
                Reason = request.Reason,
                Type = request.ReportType.ToUpper(),
                Status = ReportStatus.PENDING.ToString().ToUpper(),
                FieldId = request.FieldId,
                UserId = uid,
                AdminNotes = string.Empty,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ReportRepository.AddAsync(report);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                var admins = await _unitOfWork.UserRepository.GetAllAsync(u => u.Role.Name == "Admin" && u.Status == Domain.Enums.UserStatus.ACTIVE);
                var receiverIds = admins.Select(a => a.Id).ToList();

                var notification = new Notification()
                {
                    Title = "Báo cáo sân",
                    Content = $"{user.UserName} đã báo cáo {field.Name}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return _mapper.Map<ReportResponse>(report);
            }

            throw new ConflictException("Reporting failed!");
        }

        public async Task<ReportResponse> UpdateReportAsync(int id, UpdateReportRequest request)
        {
            if (!Enum.GetNames(typeof(ReportStatus)).Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ConflictException("Invalid ReportStatus");
            }

            var report = await _unitOfWork.ReportRepository.GetAsync(r => r.Id == id);
            if (report == null)
            {
                throw new NotFoundException("Report not exist!");
            }

            report.Status = request.Status.ToUpper();
            report.AdminNotes = request.AdminNotes;

            _unitOfWork.ReportRepository.Update(report);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<ReportResponse>(report);
            }

            throw new ConflictException("Reporting failed!");
        }

        public async Task<ReportResponse> GetByIdAsync(int id)
        {
            var report = await _unitOfWork.ReportRepository.GetAsync(r => r.Id == id);
            if (report == null)
            {
                throw new NotFoundException("Report not exist!");
            }
            return _mapper.Map<ReportResponse>(report);
        }

        public async Task<RepositoryPaginationResponse<ReportResponse>> GetMyReport(int uid, GetListReportRequest request)
        {
            if (!string.IsNullOrEmpty(request.Status) && !Enum.GetNames(typeof(ReportStatus)).Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ConflictException("Invalid ReportStatus");
            }

            var reports = await _unitOfWork.ReportRepository.GetListAsync(r => r.UserId == uid
                                                                         && (request.FieldId == null || r.FieldId == request.FieldId)
                                                                         && (string.IsNullOrEmpty(request.Status) || EF.Functions.Like(r.Status, request.Status)),
                                                                            request.Limit,
                                                                            request.Offset);
            return new RepositoryPaginationResponse<ReportResponse>
            {
                Data = _mapper.Map<IEnumerable<ReportResponse>>(reports.Data),
                Total = reports.Total
            };
        }

        public async Task<RepositoryPaginationResponse<ReportResponse>> GetAllReport(GetListReportRequest request)
        {
            if (!string.IsNullOrEmpty(request.Status) && !Enum.GetNames(typeof(ReportStatus)).Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            {
                throw new ConflictException("Invalid ReportStatus");
            }

            var reports = await _unitOfWork.ReportRepository.GetListAsync(r => (request.FieldId == null || r.FieldId == request.FieldId)
                                                                                && (string.IsNullOrEmpty(request.Status) || EF.Functions.Like(r.Status, request.Status)),
                                                                                request.Limit,
                                                                                request.Offset);
            return new RepositoryPaginationResponse<ReportResponse>
            {
                Data = _mapper.Map<IEnumerable<ReportResponse>>(reports.Data),
                Total = reports.Total
            };
        }
    }
}