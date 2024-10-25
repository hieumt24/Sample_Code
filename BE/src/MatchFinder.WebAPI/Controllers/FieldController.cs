using MatchFinder.Application.Authorize.Attributes.Field;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/fields")]
    [ApiController]
    public class FieldController : BaseApiController
    {
        private readonly IFieldService _fieldService;

        public FieldController(IFieldService fieldService)
        {
            _fieldService = fieldService;
        }

        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] GetFieldsRequest request)
        {
            var fields = await _fieldService.GetFieldsAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = fields.Total
                }
            });
        }

        [HttpGet("search-options")]
        public async Task<IActionResult> Search([FromQuery] SearchOptionRequest request)
        {
            var fields = await _fieldService.SearchOptionAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields
            });
        }

        [HttpGet("free-slots")]
        public async Task<IActionResult> GetFreeSlot([FromQuery] GetFreeSlotRequest request)
        {
            var fields = await _fieldService.GetFreeSlot(request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get free slots successfully",
                Data = fields,
            });
        }

        [HttpGet("starts-all")]
        public async Task<IActionResult> CountAllRate([FromQuery] int fieldId)
        {
            var result = await _fieldService.CountAllRateAsync(fieldId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Count rated successfully",
                Data = result
            });
        }

        [Authorize]
        [HttpGet("owner")]
        public async Task<IActionResult> MyField([FromQuery] GetFieldsRequest request)
        {
            RepositoryPaginationResponse<FieldResponse> fields = new RepositoryPaginationResponse<FieldResponse>();
            if (HttpContext.User.IsInRole("Staff"))
            {
                fields = await _fieldService.GetStaffFieldsAsync(UserID, request);
            }
            else
            {
                fields = await _fieldService.GetMyFieldsAsync(UserID, request);
            }

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = fields.Total
                }
            });
        }

        [Authorize]
        [HttpGet("early")]
        public async Task<IActionResult> FieldsEarly([FromQuery] GetFieldsEarlyRequest request)
        {
            var fields = await _fieldService.GetFieldsEarlyAsync(UserID, request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = fields.Total
                }
            });
        }

        [HttpGet("{fieldId}")]
        public async Task<IActionResult> SearchById(int fieldId)
        {
            var field = await _fieldService.GetByIdAsync(fieldId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get field successfully",
                Data = field
            });
        }

        [HttpGet("map")]
        public async Task<IActionResult> SearchLocation([FromQuery] FieldsLocationRequest request)
        {
            var fields = await _fieldService.GetFieldsAsync(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = fields.Total
                }
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateFieldAsync([FromForm] FieldCreateRequest request)
        {
            var field = await _fieldService.CreateFieldAsync(request, UserID);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Field created successfully",
                Data = field
            });
        }

        [HttpGet("scan")]
        public async Task<IActionResult> SearchByScanLocation([FromQuery] FieldsScanLocationRequest request)
        {
            var fields = await _fieldService.GetFieldsByScanRadius(request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = fields.Total
                }
            });
        }

        [FieldAuthorize]
        [HttpPut]
        public async Task<IActionResult> UpdateFieldAsync([FromForm] FieldUpdateRequest request)
        {
            var fieldResponse = await _fieldService.UpdateFieldAsync(request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Field updated successfully",
                Data = fieldResponse
            });
        }

        [FieldAuthorize]
        [HttpPut("fixed-slot")]
        public async Task<IActionResult> FixedSlotFieldAsync([FromBody] FixedSlotRequest request)
        {
            var fieldResponse = await _fieldService.FixedSlotFieldAsync(request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Set FixSlot is " + request.IsFixedSlot,
                Data = fieldResponse
            });
        }

        [FieldAuthorize]
        [HttpPut("rejection")]
        public async Task<IActionResult> RejectFieldAsync([FromBody] RejectedFieldRequest request)
        {
            var fieldResponse = await _fieldService.RejectFieldAsync(request.Id);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Field rejected",
                Data = fieldResponse
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("status")]
        public async Task<IActionResult> HandleStatusAsync([FromBody] HandleStatusFieldRequest request)
        {
            var fieldResponse = await _fieldService.HandleStatusAsync(request.Id, request.Status);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Field change status to " + request.Status.ToUpper(),
                Data = fieldResponse
            });
        }

        [FieldAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _fieldService.SoftDeleteFieldAsync(id);

            return Ok(new GeneralBoolResponse
            {
                success = true,
                message = "Field deleted!",
            });
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string keyword)
        {
            var fields = await _fieldService.SearchAsync(keyword);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields
            });
        }

        [HttpGet("rating")]
        public async Task<IActionResult> GetRatingField([FromQuery] FieldRatingFilterRequest request)
        {
            var fields = await _fieldService.GetRatingField(request);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields
            });
        }

        [HttpGet("recommend/{fieldId}")]
        public async Task<IActionResult> GetRecommendField(int fieldId)
        {
            var fields = await _fieldService.GetRecommendField(fieldId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get fields successfully",
                Data = fields
            });
        }
    }
}