using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/slots")]
    [ApiController]
    public class SlotController : BaseApiController
    {
        private readonly ISlotService _slotService;

        public SlotController(ISlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet("field/{fieldId}")]
        public async Task<IActionResult> GetListAsync(int fieldId)
        {
            GetListSlotRequest pagination = new GetListSlotRequest();
            var slotResponse = await _slotService.GetListAsync(fieldId, pagination);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get slots successfully",
                Data = slotResponse.Data,
                Meta = new Meta
                {
                    Total = slotResponse.Total
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var slotResponse = await _slotService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get slot successfully",
                Data = slotResponse
            });
        }

        [Authorize(Roles = "Field")]
        [HttpPost]
        public async Task<IActionResult> CreateslotAsync([FromBody] CreateSlotRequest request)
        {
            var slotResponse = await _slotService.CreateSlotAsync(request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "slot created successfully",
                Data = slotResponse
            });
        }

        [Authorize(Roles = "Field")]
        [HttpPut]
        public async Task<IActionResult> UpdateslotAsync([FromBody] UpdateSlotRequest request)
        {
            var slotResponse = await _slotService.UpdateSlotAsync(request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "slot updated successfully",
                Data = slotResponse
            });
        }

        [Authorize(Roles = "Field")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _slotService.SoftDeleteSlotAsync(id);

            return Ok(new GeneralBoolResponse
            {
                success = true,
                message = "slot deleted!",
            });
        }
    }
}