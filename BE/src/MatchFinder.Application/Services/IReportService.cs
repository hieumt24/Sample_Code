using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IReportService
    {
        Task<ReportResponse> CreateReportAsync(CreateReportRequest request, int uid);

        Task<ReportResponse> UpdateReportAsync(int id, UpdateReportRequest request);

        Task<ReportResponse> GetByIdAsync(int id);

        Task<RepositoryPaginationResponse<ReportResponse>> GetMyReport(int uid, GetListReportRequest request);

        Task<RepositoryPaginationResponse<ReportResponse>> GetAllReport(GetListReportRequest request);
    }
}