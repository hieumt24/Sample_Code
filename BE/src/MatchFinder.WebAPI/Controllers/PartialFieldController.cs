using MatchFinder.Application.Authorize.Attributes.PartialField;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/partial-fields")]
    [ApiController]
    public class PartialFieldController : BaseApiController
    {
        private readonly IPartialFieldService _partialFieldService;

        public PartialFieldController(IPartialFieldService partialFieldService)
        {
            _partialFieldService = partialFieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] PartialFieldFilterRequest filterRequest)
        {
            var partialFields = await _partialFieldService.GetListAsync(filterRequest);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get partial fields successfully",
                Data = partialFields.Data,
                Meta = new Meta
                {
                    Limit = filterRequest.Limit,
                    Offset = filterRequest.Offset,
                    Total = partialFields.Total
                }
            });
        }

        [HttpGet("field/{fieldId}/available")]
        public async Task<IActionResult> GetListAsync(int fieldId, [FromQuery] PartialFieldAvailableRequest filterRequest)
        {
            var partialFields = await _partialFieldService.GetListAvailable(fieldId, filterRequest);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get partial fields successfully",
                Data = partialFields.Data,
                Meta = new Meta
                {
                    Limit = filterRequest.Limit,
                    Offset = filterRequest.Offset,
                    Total = partialFields.Total
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var partialField = await _partialFieldService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get partial field successfully",
                Data = partialField
            });
        }

        [HttpGet("field/{fieldId}")]
        public async Task<IActionResult> GetListByFieldAsync(int fieldId)
        {
            var partialFields = await _partialFieldService.GetListByFieldAsync(fieldId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get partial fields successfully",
                Data = partialFields
            });
        }

        //auth
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreatePartialFieldAsync([FromForm] PartialFieldCreateRequest request)
        {
            var partialField = await _partialFieldService.CreatePartialFieldAsync(UserID, request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Partial field created successfully",
                Data = partialField
            });
        }

        [PartialFieldAuthorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePartialFieldAsync(int id, [FromForm] PartialFieldUpdateRequest request)
        {
            var partialField = await _partialFieldService.UpdatePartialFieldAsync(id, request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Partial field updated successfully",
                Data = partialField
            });
        }
    }
}