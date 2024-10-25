using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/opponent-findings")]
    [ApiController]
    public class OpponentFindingController : BaseApiController
    {
        private readonly IOpponentFindingService _opponentFindingService;

        public OpponentFindingController(IOpponentFindingService opponentFindingService)
        {
            _opponentFindingService = opponentFindingService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateNewOpponentFinding([FromBody] OpponentFindingCreateRequest request)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.CreateNewOpponentFinding(request, userRequestId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Create new opponent finding successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPost("not-booking")]
        public async Task<IActionResult> CreateNewOpponentFindingNotBooking([FromBody] OpponentFindingNotBookingCreateRequest request)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.CreateNewOpponentFindingNotBooking(request, userRequestId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Create new opponent finding not booking successfully",
                Data = response
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOpponentFinding(int id)
        {
            var response = await _opponentFindingService.GetOpponentFinding(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get opponent finding successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOpponentFinding(int id, [FromBody] OpponentFindingUpdateRequest request)
        {
            var response = await _opponentFindingService.UpdateOpponentFinding(id, request);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Update opponent finding successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{id}/cancel-post")]
        public async Task<IActionResult> CancelPost(int id)
        {
            var userFindingId = UserID;
            var response = await _opponentFindingService.CancelPost(id, userFindingId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Cancel post successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{id}/cancel-matching")]
        public async Task<IActionResult> CanceledMatching(int id)
        {
            var userFindingId = UserID;
            var response = await _opponentFindingService.CanceledMatching(id, userFindingId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Canceled matching successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{oldOpponentFindingId}/restore-finding")]
        public async Task<IActionResult> RestoreFinding(int oldOpponentFindingId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.RestoreFinding(oldOpponentFindingId, userRequestId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Restore finding successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{oldOpponentFindingId}/restore-overlap-findings")]
        public async Task<IActionResult> RestoreOverlapFindings(int oldOpponentFindingId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.RestoreOverFindings(oldOpponentFindingId, userRequestId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Restore finding successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{opponentFindingId}/restore-overlap-post")]
        public async Task<IActionResult> RestoreOverlapPost(int opponentFindingId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.RestoreOverlapPost(opponentFindingId, userRequestId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Restore overlap post successfully",
                Data = response
            });
        }

        [HttpGet("{oldOpponentFindingId}/check-can-restore-overlap-post")]
        public async Task<IActionResult> CheckOverlapPost(int oldOpponentFindingId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.CheckOverlapPost(oldOpponentFindingId, userRequestId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Check overlap post successfully",
                Data = response
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetRepositoryPagination([FromQuery] OpponentFindingFilterRequest filterRequest)
        {
            var response = await _opponentFindingService.GetRepositoryPagination(filterRequest);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get opponent finding successfully",
                Data = response.Data,
                Meta = new Meta
                {
                    Limit = filterRequest.Limit,
                    Offset = filterRequest.Offset,
                    Total = response.Total
                }
            });
        }

        [Authorize]
        [HttpPost("requesting")]
        public async Task<IActionResult> RequestOpponentFinding([FromBody] RequestingOpponentFindingRequest request)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.RequestOpponentFinding(userRequestId, request);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Request opponent finding successfully",
                Data = response
            });
        }

        [HttpGet("{id}/requesting")]
        public async Task<IActionResult> GetListUserRequestOpponentFinding(int id, [FromQuery] RequestingOpponentFindingFilter filter)
        {
            var response = await _opponentFindingService.GetListUserRequestByOpponentFindingId(id, filter);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get list user request opponent finding successfully",
                Data = response
            });
        }

        [HttpPost("requesting/{id}/accept")]
        [Authorize]
        public async Task<IActionResult> AcceptRequestOpponentFinding(int id)
        {
            var response = await _opponentFindingService.AcceptRequestOpponentFinding(id);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Accept request opponent finding successfully",
                Data = response
            });
        }

        [HttpPost("requesting/{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelRequestOpponentFinding(int id)
        {
            var response = await _opponentFindingService.CancelRequestOpponentFinding(id);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Cancel request opponent finding successfully",
                Data = response
            });
        }

        [HttpGet("my-request")]
        [Authorize]
        public async Task<IActionResult> GetMyRequest([FromQuery] int opponentFindingId)
        {
            var userId = UserID;
            var response = await _opponentFindingService.GetMyRequest(opponentFindingId, userId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get my request successfully",
                Data = response
            });
        }

        [HttpGet("my-history")]
        [Authorize]
        public async Task<IActionResult> GetMyHistoryOpponentFinding([FromQuery] OpponentFindingFilterRequest filterRequest)
        {
            var userId = UserID;
            var response = await _opponentFindingService.GetMyHistoryOpponentFinding(userId, filterRequest);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get my history opponent finding successfully",
                Data = response.Data,
                Meta = new Meta
                {
                    Limit = filterRequest.Limit,
                    Offset = filterRequest.Offset,
                    Total = response.Total
                }
            });
        }

        [HttpGet("my-history-request")]
        [Authorize]
        public async Task<IActionResult> GetMyHistoryRequestOpponentFinding([FromQuery] OpponentFindingFilterRequest filterRequest)
        {
            var userId = UserID;
            var response = await _opponentFindingService.GetMyHistoryRequestOpponentFinding(userId, filterRequest);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get my history request opponent finding successfully",
                Data = response.Data,
                Meta = new Meta
                {
                    Limit = filterRequest.Limit,
                    Offset = filterRequest.Offset,
                    Total = response.Total
                }
            });
        }

        [HttpGet("requests-overlap")]
        [Authorize]
        public async Task<IActionResult> RequestsOverlapOtherRequests([FromQuery] DateOnly date, [FromQuery] int startTime, [FromQuery] int endTime)
        {
            var userId = UserID;
            var response = await _opponentFindingService.RequestsOverlapOtherRequests(userId, date, startTime, endTime);
            if (response.Any(x => string.IsNullOrEmpty(x.Status)))
            {
                return Ok(new GeneralGetResponse
                {
                    Success = true,
                    Message = "Yêu cầu tìm đối hiện tại của bạn đang trùng với cái trước đấy",
                    Data = true
                });
            }
            else
            {
                return Ok(new GeneralGetResponse
                {
                    Success = true,
                    Message = "Không bị trùng",
                    Data = false
                });
            }
        }

        [HttpGet("opponent-finding-overlap")]
        [Authorize]
        public async Task<IActionResult> CheckOpponentFindingExisted([FromQuery] DateOnly date, [FromQuery] int startTime, [FromQuery] int endTime)
        {
            var userId = UserID;
            var response = await _opponentFindingService.CheckOpponentFindingExisted(userId, date, startTime, endTime);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Check opponent finding existed successfully",
                Data = response
            });
        }

        [HttpGet("request-was-accepted")]
        [Authorize]
        public async Task<IActionResult> CheckRequestWasAccepted([FromQuery] DateOnly date, [FromQuery] int startTime, [FromQuery] int endTime)
        {
            var userId = UserID;
            var response = await _opponentFindingService.CheckRequestWasAccepted(userId, date, startTime, endTime);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Check request was accepted successfully",
                Data = response
            });
        }

        [HttpGet("opponent-finding-accepted")]
        [Authorize]
        public async Task<IActionResult> CheckOpponentFindingAccepted([FromQuery] DateOnly date, [FromQuery] int startTime, [FromQuery] int endTime)
        {
            var userId = UserID;
            var response = await _opponentFindingService.CheckOpponentFindingAccepted(userId, date, startTime, endTime);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Check opponent finding accepted successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{oldOpponentFindingId}/restore-overlap-request")]
        public async Task<IActionResult> RestoreRequestOpponentFinding(int oldOpponentFindingId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.RestoreRequestOpponentFinding(userRequestId, oldOpponentFindingId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Restore request opponent finding successfully",
                Data = response
            });
        }

        [Authorize]
        [HttpPut("{requestId}/restore-single-overlap-request")]
        public async Task<IActionResult> RestoreOverlapRequest(int requestId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.RestoreOverlapRequest(requestId, userRequestId);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Restore overlap request successfully",
                Data = response
            });
        }

        [HttpGet("{oldOpponentFindingId}/check-can-restore-overlap-request")]
        public async Task<IActionResult> CheckOverlapRequest(int oldOpponentFindingId)
        {
            var userRequestId = UserID;
            var response = await _opponentFindingService.CheckOverlapRequest(userRequestId, oldOpponentFindingId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Check overlap request successfully",
                Data = response
            });
        }
    }
}