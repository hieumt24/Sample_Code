using MatchFinder.Application.Authorize.Attributes;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/bookings")]
    [ApiController]
    public class BookingController : BaseApiController
    {
        private readonly IBookingService _bookingService;
        private readonly IPartialFieldService _partialFieldService;

        public BookingController
            (
            IBookingService bookingService,
            IPartialFieldService partialFieldService
            )
        {
            _bookingService = bookingService;
            _partialFieldService = partialFieldService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListAsync([FromQuery] SearchBookingRequest pagination)
        {
            var bookingResponse = await _bookingService.GetListAsync(pagination);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = bookingResponse.Data,
                Meta = new Meta
                {
                    Limit = pagination.Limit,
                    Offset = pagination.Offset,
                    Total = bookingResponse.Total
                }
            });
        }

        [HttpGet("field/{fieldId}")]
        public async Task<IActionResult> GetListByFieldAsync(int fieldId, [FromQuery] SearchBookingByFieldRequest request)
        {
            var bookingResponse = await _bookingService.GetListAsync(fieldId, request);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = bookingResponse.Data,
                Meta = new Meta
                {
                    Limit = request.Limit,
                    Offset = request.Offset,
                    Total = bookingResponse.Total
                }
            });
        }

        [HttpGet("busy")]
        public async Task<IActionResult> GetListAsync([FromQuery] ListBookingBusyRequest request)
        {
            var bookingResponse = await _bookingService.GetListAsync(request, UserID);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = bookingResponse.Data,
                Meta = new Meta
                {
                    Total = bookingResponse.Total
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var bookingResponse = await _bookingService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get booking successfully",
                Data = bookingResponse
            });
        }

        [Authorize]
        [HttpGet("history")]
        public async Task<IActionResult> MyBooking([FromQuery] MyBookingRequest request)
        {
            var bookingResponse = await _bookingService.GetMyBooking(UserID, request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = bookingResponse.Data,
                Meta = new Meta
                {
                    Total = bookingResponse.Total,
                    Limit = request.Limit,
                    Offset = request.Offset
                }
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateBookingAsync([FromBody] BookingCreateRequest request)
        {
            var bookingResponse = await _bookingService.CreateBookingAsync(request, UserID);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Booking created successfully",
                Data = bookingResponse
            });
        }

        [Authorize]
        [HttpPost("book-from-field")]
        public async Task<IActionResult> CreateBookingAsync([FromBody] FieldAutoBookingRequest request)
        {
            var bookingResponse = await _bookingService.CreateBookingAsync(request, UserID);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Booking created successfully",
                Data = bookingResponse
            });
        }

        [BookingAuthorize]
        [HttpPut]
        public async Task<IActionResult> UpdateBookingAsync([FromBody] BookingUpdateRequest request)
        {
            var bookingResponse = await _bookingService.UpdateBookingAsync(request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Booking updated successfully",
                Data = bookingResponse
            });
        }

        [BookingAuthorize]
        [HttpPut("rejection")]
        public async Task<IActionResult> RejectBookingAsync([FromBody] RejectionRequest request)
        {
            var bookingResponse = await _bookingService.RejectBookingAsync(request.Id);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Booking rejected",
                Data = bookingResponse
            });
        }

        //author field owner
        [HandleStatusAuthorize]
        [HttpPut("status")]
        public async Task<IActionResult> HandleStatusAsync([FromBody] HandleStatusRequest request)
        {
            var bookingResponse = await _bookingService.HandleStatusAsync(request);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Booking change status to " + request.Status.ToUpper(),
                Data = bookingResponse
            });
        }

        [BookingAuthorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _bookingService.SoftDeleteBookingAsync(id);

            return Ok(new GeneralBoolResponse
            {
                success = true,
                message = "Booking deleted!",
            });
        }

        [Authorize]
        [HttpGet("free-for-finding")]
        public async Task<IActionResult> GetFutureBookingWithStatusWaitingOrAccepted([FromQuery] Pagination pagination)
        {
            var bookingResponse = await _bookingService.GetFutureBookingWithStatusWaitingOrAccepted(UserID, pagination);

            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = bookingResponse.Data,
                Meta = new Meta
                {
                    Limit = pagination.Limit,
                    Offset = pagination.Offset,
                    Total = bookingResponse.Total
                }
            });
        }

        [HttpGet("status/waiting-or-accepted")]
        public async Task<IActionResult> GetListByFieldAndDatesAsync([FromQuery] GetBookingByFieldAndDatesRequest request)
        {
            var bookingResponse = await _bookingService.GetListByFieldAndDatesAsync(request);

            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get bookings successfully",
                Data = bookingResponse
            });
        }
    }
}