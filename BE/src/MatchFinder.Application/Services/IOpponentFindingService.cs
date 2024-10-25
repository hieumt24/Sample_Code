using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services
{
    public interface IOpponentFindingService
    {
        Task<OpponentFindingResponse> CreateNewOpponentFinding(OpponentFindingCreateRequest request, int userRequestId);

        Task<OpponentFindingResponse> CreateNewOpponentFindingNotBooking(OpponentFindingNotBookingCreateRequest request, int userRequestId);

        Task<IEnumerable<OpponentFindingResponse>> CheckOpponentFindingExisted(int userId, DateOnly date, int startTime, int endTime);

        Task<OpponentFindingResponseWithUserRequesting> CheckRequestWasAccepted(int userId, DateOnly date, int startTime, int endTime);

        Task<IEnumerable<OpponentFindingResponse>> CheckOpponentFindingAccepted(int userId, DateOnly date, int startTime, int endTime);

        Task<OpponentFindingResponse> GetOpponentFinding(int id);

        Task<OpponentFindingResponse> UpdateOpponentFinding(int id, OpponentFindingUpdateRequest request);

        Task<OpponentFindingResponse> CancelPost(int id, int userFindingId);

        Task<OpponentFindingResponse> CanceledMatching(int id, int userFindingId);

        Task<OpponentFindingResponse> RestoreFinding(int oldOpponentFindingId, int userRequestId);

        Task<IEnumerable<OpponentFindingResponse>> RestoreOverFindings(int oldOpponentFindingId, int userRequestId);

        Task<OpponentFindingResponse> RestoreOverlapPost(int opponentFindingId, int userRequestId);

        Task<IEnumerable<OpponentFindingResponse>> CheckOverlapPost(int oldOpponentFindingId, int userRequestId);

        Task<RepositoryPaginationResponse<OpponentFindingResponse>> GetRepositoryPagination(OpponentFindingFilterRequest filterRequest);

        Task<OpponentFindingResponseWithUserRequesting> RequestOpponentFinding(int userRequestId, RequestingOpponentFindingRequest request);

        Task<IEnumerable<OpponentFindingResponseWithUserRequesting>> GetListUserRequestByOpponentFindingId(int opponentFindingId, RequestingOpponentFindingFilter filter);

        Task<OpponentFindingResponseWithUserRequesting> AcceptRequestOpponentFinding(int id);

        Task<OpponentFindingResponseWithUserRequesting> GetMyRequest(int opponentFindingId, int userId);

        Task<RepositoryPaginationResponse<OpponentFindingResponse>> GetMyHistoryOpponentFinding(int userId, OpponentFindingFilterRequest filterRequest);

        Task<RepositoryPaginationResponse<OpponentFindingResponseWithUserRequesting>> GetMyHistoryRequestOpponentFinding(int userId, OpponentFindingFilterRequest filterRequest);

        Task<OpponentFindingResponseWithUserRequesting> CancelRequestOpponentFinding(int id);

        Task<IEnumerable<OpponentFindingResponseWithUserRequesting>> RestoreRequestOpponentFinding(int userRequestId, int oldOpponentFindingId);

        Task<OpponentFindingResponseWithUserRequesting> RestoreOverlapRequest(int requestId, int userRequestId);

        Task<IEnumerable<OpponentFindingResponseWithUserRequesting>> CheckOverlapRequest(int userRequestId, int oldOpponentFindingId);

        Task<IEnumerable<OpponentFindingRequest>> RequestsOverlapOtherRequests(int userId, DateOnly date, int startTime, int endTime);
    }
}