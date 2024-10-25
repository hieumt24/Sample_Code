using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Models;
using Net.payOS.Types;

namespace MatchFinder.Application.Services
{
    public interface IBookingService
    {
        Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request, int uid);

        Task<BookingResponse> CreateBookingAsync(FieldAutoBookingRequest request, int uid);

        Task<BookingResponse> UpdateBookingAsync(BookingUpdateRequest request);

        Task<BookingResponse> RejectBookingAsync(int id);

        Task<BookingResponse> HandleStatusAsync(HandleStatusRequest request);

        Task SoftDeleteBookingAsync(int id);

        Task<BookingResponse> GetByIdAsync(int id);

        Task<RepositoryPaginationResponse<BookingResponse>> GetMyBooking(int uid, MyBookingRequest request);

        Task<RepositoryPaginationResponse<BookingResponse>> GetListAsync(SearchBookingRequest pagination);

        Task<RepositoryPaginationResponse<BookingResponse>> GetListAsync(int fieldId, SearchBookingByFieldRequest request);

        Task<RepositoryPaginationResponse<BookingResponse>> GetListAsync(ListBookingBusyRequest request, int uid);

        Task VerifyPaymentWebhookData(WebhookType hook);

        Task<RepositoryPaginationResponse<BookingResponse>> GetFutureBookingWithStatusWaitingOrAccepted(int uid, Pagination pagination);

        Task<IEnumerable<BookingResponse>> GetListByFieldAndDatesAsync(GetBookingByFieldAndDatesRequest request);
    }
}