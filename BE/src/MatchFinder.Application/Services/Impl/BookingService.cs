using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;

namespace MatchFinder.Application.Services.Impl
{
    public class BookingService : IBookingService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITransactionService _transactionService;
        private readonly IPayOSPaymentService _payOSPaymentService;
        private readonly INotificationService _notificationService;

        public BookingService(IMapper mapper, IUnitOfWork unitOfWork, ITransactionService transactionService, IPayOSPaymentService payOSPaymentService, INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _transactionService = transactionService;
            _payOSPaymentService = payOSPaymentService;
            _notificationService = notificationService;
        }

        public async Task<BookingResponse> CreateBookingAsync(BookingCreateRequest request, int uid)
        {
            var bookingDate = request.Date.ToDateTime(TimeOnly.MinValue);
            var bookingStartTime = bookingDate.AddSeconds(request.StartTime);
            var bookingEndTime = bookingDate.AddSeconds(request.EndTime);

            if (DateTime.Now >= bookingEndTime)
            {
                throw new ConflictException("Thời gian đặt sân không được nhỏ hơn thời gian hiện tại!");
            }

            var team = await _unitOfWork.TeamRepository.GetAsync(t => t.Id == request.TeamId);

            var user = await _unitOfWork.UserRepository.GetAsync(p => p.Id == uid);
            if (user == null)
                throw new ConflictException("User request not exist!");

            var pfield = await _unitOfWork.PartialFieldRepository.GetAsync(
                p => p.Id == request.PartialFieldId &&
                        !(p.Field.IsDeleted ?? false) &&
                        p.Field.Status == "ACCEPTED",
                p => p.Field.InactiveTimes.Where(it => it.IsDeleted != true),
                p => p.Field.Staffs.Where(s => s.IsDeleted != true && s.IsActive == true)
            );

            if (pfield == null)
                throw new ConflictException("Field not exist!");

            foreach (var inactiveTime in pfield.Field.InactiveTimes)
            {
                if (IsOverlapping(inactiveTime.StartTime, inactiveTime.EndTime, bookingStartTime, bookingEndTime))
                {
                    throw new ConflictException("Không thể thực hiện đặt sân do trùng với lịch nghỉ!");
                }
            }

            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.PartialFieldId == request.PartialFieldId
                                                                            && b.Date == request.Date
                                                                            && !EF.Functions.Like(b.Status, BookingStatus.CANCELED)
                                                                            && !EF.Functions.Like(b.Status, BookingStatus.REJECTED));

            foreach (var booking in bookings)
            {
                if (IsOverlapping(booking, request.StartTime, request.EndTime))
                {
                    throw new ConflictException("Đã có người đặt sân trong khoảng thời gian này!");
                }
            }

            decimal amountRequire = (pfield.Field.OwnerId == uid || pfield.Field.Staffs.Any(s => s.UserId == uid)) ?
                        0 : pfield.Field.Deposit;

            decimal userAmount = await _transactionService.GetAmountAsync(uid);

            if (amountRequire > userAmount)
                throw new ConflictException("Số dư không đủ!");

            var newBooking = new Booking
            {
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                DepositAmount = amountRequire,
                PartialField = pfield,
                User = user,
                Team = team,
                Status = pfield.Field.OwnerId == uid ? BookingStatus.ACCEPTED : BookingStatus.WAITING,
            };

            await _unitOfWork.BookingRepository.AddAsync(newBooking);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                var response = _mapper.Map<BookingResponse>(newBooking);

                await CreateAndSaveTransactionAsync(newBooking);

                //var paymentLink = await _payOSPaymentService.createPaymentLink(newBooking);
                //response.PaymentLink = paymentLink;
                var receiverIds = new List<int> { pfield.Field.OwnerId };
                if (pfield.Field.Staffs != null)
                {
                    receiverIds.AddRange(pfield.Field.Staffs.Select(staff => staff.UserId));

                }
                var notification = new Notification()
                {
                    Title = "Đặt sân",
                    Content = $"{pfield.Name} của {pfield.Field.Name} đã được đặt vào ngày {response.Date.ToString("dd/MM/yyyy")} từ {response.StartTime} đến {response.EndTime}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return response;
            }

            throw new ConflictException("Đặt sân không thành công!");
        }

        public async Task<BookingResponse> CreateBookingAsync(FieldAutoBookingRequest request, int uid)
        {
            var bookingDate = request.Date.ToDateTime(TimeOnly.MinValue);
            var bookingStartTime = bookingDate.AddSeconds(request.StartTime);
            var bookingEndTime = bookingDate.AddSeconds(request.EndTime);

            if (DateTime.Now >= bookingEndTime)
            {
                throw new ConflictException("Thời gian đặt sân không được nhỏ hơn thời gian hiện tại!");
            }

            var user = await _unitOfWork.UserRepository.GetAsync(p => p.Id == uid);
            if (user == null)
                throw new ConflictException("User request not exist!");

            var pfield = await _unitOfWork.PartialFieldRepository.GetAsync(
                p => p.FieldId == request.FieldId &&
                        !(p.Field.IsDeleted ?? false) &&
                        !p.Bookings.Any(b => b.Date == request.Date &&
                              ((b.StartTime <= request.StartTime && b.EndTime > request.StartTime) ||
                               (b.StartTime < request.EndTime && b.EndTime >= request.EndTime) ||
                               (b.StartTime >= request.StartTime && b.EndTime <= request.EndTime))) &&
                        p.Field.Status == "ACCEPTED",
                p => p.Field.InactiveTimes.Where(it => it.IsDeleted != true),
                 p => p.Field.Staffs.Where(s => s.IsDeleted != true && s.IsActive == true)
            );

            if (pfield == null)
                throw new ConflictException("There is no PartialField to book!");

            foreach (var inactiveTime in pfield.Field.InactiveTimes)
            {
                if (IsOverlapping(inactiveTime.StartTime, inactiveTime.EndTime, bookingStartTime, bookingEndTime))
                {
                    throw new ConflictException("The requested booking time overlaps with an inactive time period.");
                }
            }

            var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.PartialFieldId == pfield.Id
                                                                            && b.Date == request.Date
                                                                            && !EF.Functions.Like(b.Status, BookingStatus.CANCELED)
                                                                            && !EF.Functions.Like(b.Status, BookingStatus.REJECTED));

            foreach (var booking in bookings)
            {
                if (IsOverlapping(booking, request.StartTime, request.EndTime))
                {
                    throw new ConflictException("The requested booking time overlaps with an existing booking.");
                }
            }

            decimal amountRequire = (pfield.Field.OwnerId == uid || pfield.Field.Staffs.Any(s => s.UserId == uid)) ?
                        0 : pfield.Field.Deposit;

            decimal userAmount = await _transactionService.GetAmountAsync(uid);

            if (amountRequire > userAmount)
                throw new ConflictException("Amount not enough!");

            var newBooking = new Booking
            {
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                DepositAmount = amountRequire,
                PartialField = pfield,
                User = user,
                Status = pfield.Field.OwnerId == uid ? BookingStatus.ACCEPTED : BookingStatus.WAITING,
            };

            await _unitOfWork.BookingRepository.AddAsync(newBooking);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                var response = _mapper.Map<BookingResponse>(newBooking);

                await CreateAndSaveTransactionAsync(newBooking);

                //var paymentLink = await _payOSPaymentService.createPaymentLink(newBooking);
                //response.PaymentLink = paymentLink;
                var receiverIds = new List<int> { pfield.Field.OwnerId };
                if (pfield.Field.Staffs != null)
                {
                    receiverIds.AddRange(pfield.Field.Staffs.Select(staff => staff.UserId));

                }
                var notification = new Notification()
                {
                    Title = "Đặt sân",
                    Content = $"{pfield.Name} của {pfield.Field.Name} đã được đặt vào ngày {response.Date.ToString("dd/MM/yyyy")} từ {response.StartTime} đến {response.EndTime}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return response;
            }

            throw new ConflictException("The requested booking failed!");
        }

        private bool IsOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && start2 < end1;
        }

        public async Task<RepositoryPaginationResponse<BookingResponse>> GetListAsync(SearchBookingRequest pagination)
        {
            var bookings = await _unitOfWork.BookingRepository
                .GetListAsync(x =>
                    (!pagination.Date.HasValue || x.Date == pagination.Date.Value) &&
                    (!pagination.StartTime.HasValue || x.StartTime >= pagination.StartTime.Value) &&
                    (!pagination.EndTime.HasValue || x.EndTime <= pagination.EndTime.Value) &&
                    (!pagination.PartialFieldId.HasValue || x.PartialFieldId == pagination.PartialFieldId.Value) &&
                    !(x.PartialField.Field.IsDeleted ?? false) &&
                    x.PartialField.Field.Status == "ACCEPTED" &&
                    (!pagination.UserId.HasValue || x.UserId == pagination.UserId.Value) &&
                    (string.IsNullOrEmpty(pagination.Status) || x.Status == pagination.Status),
                    pagination.Limit,
                    pagination.Offset,
                    x => x.User,
                    x => x.PartialField.Field.Owner,
                    x => x.PartialField.Field.Rates,
                    x => x.Team,
                    x => x.Rates,
                    x => x.OpponentFindings);

            return new RepositoryPaginationResponse<BookingResponse>
            {
                Data = _mapper.Map<IEnumerable<BookingResponse>>(bookings.Data),
                Total = bookings.Total
            };
        }

        public async Task<RepositoryPaginationResponse<BookingResponse>> GetListAsync(int fieldId, SearchBookingByFieldRequest request)
        {
            var bookings = await _unitOfWork.BookingRepository
                .GetListAsync(x =>
                    x.PartialField.FieldId == fieldId &&
                    !(x.PartialField.Field.IsDeleted ?? false) &&
                    x.PartialField.Field.Status == "ACCEPTED" &&
                    (!request.Date.HasValue || x.Date == request.Date.Value) &&
                    (string.IsNullOrEmpty(request.Status) || x.Status.ToLower().Equals(request.Status)) &&
                    (!request.StartTime.HasValue || x.StartTime >= request.StartTime.Value) &&
                    (!request.EndTime.HasValue || x.EndTime <= request.EndTime.Value),
                    request.Limit,
                    request.Offset,
                    x => x.CreatedAt,
                    true,
                    x => x.User,
                    x => x.PartialField.Field.Owner,
                    x => x.PartialField.Field.Rates,
                    x => x.Team,
                    x => x.Rates,
                    x => x.OpponentFindings);

            return new RepositoryPaginationResponse<BookingResponse>
            {
                Data = _mapper.Map<IEnumerable<BookingResponse>>(bookings.Data),
                Total = bookings.Total
            };
        }

        public async Task<RepositoryPaginationResponse<BookingResponse>> GetListAsync(ListBookingBusyRequest request, int uid)
        {
            var bookings = await _unitOfWork.BookingRepository
                .GetAllAsync(x =>
                    (
                        (!request.Date.HasValue && !request.StartDate.HasValue && !request.EndDate.HasValue) ||
                        (request.Date.HasValue && x.Date == request.Date) ||
                        (x.Date >= (request.StartDate ?? DateOnly.MinValue) && x.Date <= (request.EndDate ?? DateOnly.MaxValue))
                    ) &&
                    (
                        (!request.StartTime.HasValue && !request.EndTime.HasValue) ||
                        (request.StartTime.HasValue && request.EndTime.HasValue &&
                         (x.StartTime < request.EndTime &&
                          x.EndTime > request.StartTime))
                    ) &&
                    (!request.PartialFieldId.HasValue || x.PartialFieldId == request.PartialFieldId.Value) &&
                    !(x.PartialField.Field.IsDeleted ?? false) &&
                    x.PartialField.Field.Status == "ACCEPTED" &&
                    ((!EF.Functions.Like(x.Status, BookingStatus.REJECTED) && !EF.Functions.Like(x.Status, BookingStatus.CANCELED))
                    || (x.UserId == uid && EF.Functions.Like(x.Status, BookingStatus.WAITING))),
                    x => x.User,
                    x => x.PartialField.Field.Owner,
                    x => x.PartialField.Field.Rates,
                    x => x.Team,
                    x => x.Rates,
                    x => x.OpponentFindings);

            return new RepositoryPaginationResponse<BookingResponse>
            {
                Data = _mapper.Map<IEnumerable<BookingResponse>>(bookings),
                Total = bookings.Count()
            };
        }

        public async Task<BookingResponse> GetByIdAsync(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetAsync(b => b.Id == id &&
                                                                        !(b.PartialField.Field.IsDeleted ?? false) &&
                                                                        b.PartialField.Field.Status == "ACCEPTED",
                                                                        x => x.User,
                                                                        p => p.PartialField.Field.Owner,
                                                                        t => t.Team,
                                                                        x => x.Rates,
                                                                        x => x.OpponentFindings);
            if (booking == null)
                throw new NotFoundException("Booking not exist!");

            return _mapper.Map<BookingResponse>(booking);
        }

        public async Task<RepositoryPaginationResponse<BookingResponse>> GetMyBooking(int uid, MyBookingRequest request)
        {
            var bookings = await _unitOfWork.BookingRepository.GetListAsync(b => b.UserId == uid &&
                                                                            (!request.FieldId.HasValue || b.PartialField.FieldId == request.FieldId) &&
                                                                            (string.IsNullOrEmpty(request.Status) || b.Status.ToLower().Equals(request.Status)) &&
                                                                            (!request.StartTime.HasValue || b.Date >= request.StartTime.Value) &&
                                                                            (!request.EndTime.HasValue || b.Date <= request.EndTime.Value),
                                                                            request.Limit, request.Offset,
                                                                            x => x.User,
                                                                            p => p.PartialField.Field.Owner,
                                                                            t => t.Team,
                                                                            x => x.Rates,
                                                                            x => x.OpponentFindings);

            return new RepositoryPaginationResponse<BookingResponse>
            {
                Data = _mapper.Map<IEnumerable<BookingResponse>>(bookings.Data),
                Total = bookings.Total
            };
        }

        public async Task<BookingResponse> RejectBookingAsync(int id)
        {
            var booking = await _unitOfWork.BookingRepository.GetAsync(b => b.Id == id
                                                                        && b.Status.ToLower() == BookingStatus.WAITING.ToLower(),
                                                                        x => x.User,
                                                                        p => p.PartialField.Field.Owner,
                                                                        t => t.Team,
                                                                        x => x.Rates,
                                                                        x => x.OpponentFindings);
            if (booking == null)
                throw new NotFoundException("Booking not exist!");

            booking.Status = BookingStatus.CANCELED;

            _unitOfWork.BookingRepository.Update(booking);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                await CreateAndSaveTransactionAsync(booking);
                var receiverIds = new List<int> { booking.PartialField.Field.OwnerId };
                if (booking.PartialField.Field.Staffs != null)
                {
                    receiverIds.AddRange(booking.PartialField.Field.Staffs.Select(staff => staff.UserId));

                }
                var notification = new Notification()
                {
                    Title = "Khách hàng huỷ đặt sân",
                    Content = $"Yêu cầu đặt sân đã bị huỷ: {booking.PartialField.Name} của {booking.PartialField.Field.Name} đã đặt vào ngày {booking.Date.ToString("dd/MM/yyyy")} từ {GetTimeString(booking.StartTime)} đến {GetTimeString(booking.EndTime)}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return _mapper.Map<BookingResponse>(booking);
            }
            throw new ConflictException("The requested booking failed!");
        }

        public async Task<BookingResponse> HandleStatusAsync(HandleStatusRequest request)
        {
            var booking = await GetBookingValidAsync(request.Id);
            if (booking == null)
                throw new NotFoundException("Booking not exist!");

            var newStatus = GetNewStatus(request.Status, booking.Status);
            if (newStatus == null)
                throw new ConflictException("Invalid status change");

            booking.Status = newStatus;
            _unitOfWork.BookingRepository.Update(booking);

            if (await _unitOfWork.CommitAsync() <= 0)
                throw new ConflictException("The requested booking update failed!");

            await CreateAndSaveTransactionAsync(booking);
            var receiverIds = new List<int> { booking.UserId };
            if (newStatus == BookingStatus.ACCEPTED)
            {
                var notification = new Notification()
                {
                    Title = "Chấp nhận đặt sân",
                    Content = $"Yêu cầu đặt sân của bạn đã được chấp nhận: {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào ngày {booking.Date.ToString("dd/MM/yyyy")} từ {GetTimeString(booking.StartTime)} đến {GetTimeString(booking.EndTime)}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
            }
            else if (newStatus == BookingStatus.REJECTED)
            {
                var notification = new Notification()
                {
                    Title = "Từ chối đặt sân",
                    Content = $"Yêu cầu đặt sân của bạn đã bị từ chối: {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào ngày {booking.Date.ToString("dd/MM/yyyy")} từ {GetTimeString(booking.StartTime)} đến {GetTimeString(booking.EndTime)}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
            }
            return _mapper.Map<BookingResponse>(booking);
        }

        private async Task<Booking> GetBookingValidAsync(int id)
        {
            return await _unitOfWork.BookingRepository.GetAsync(
                b => b.Id == id &&
                !EF.Functions.Like(b.Status, BookingStatus.CANCELED) &&
                !(b.PartialField.Field.IsDeleted ?? false) &&
                EF.Functions.Like(b.PartialField.Field.Status, FieldStatus.ACCEPTED),
                x => x.User,
                p => p.PartialField.Field.Owner,
                t => t.Team,
                x => x.Rates,
                x => x.OpponentFindings
            );
        }

        private string GetNewStatus(string requestStatus, string currentStatus)
        {
            var status = requestStatus.ToLower();
            var currentStatusLower = currentStatus.ToLower();

            if (status == BookingStatus.REJECTED.ToLower() && (currentStatusLower == BookingStatus.WAITING.ToLower() || currentStatusLower == BookingStatus.ACCEPTED.ToLower()))
                return BookingStatus.REJECTED;
            else if (status == BookingStatus.ACCEPTED.ToLower() && currentStatusLower == BookingStatus.WAITING.ToLower())
                return BookingStatus.ACCEPTED;

            return null;
        }

        private async Task CreateAndSaveTransactionAsync(Booking booking)
        {
            var trans = new MatchFinder.Domain.Entities.Transaction
            {
                Status = TransactionStatus.SUCCESSFUL,
                Amount = booking.DepositAmount,
                Booking = booking,
                UserId = 1
            };

            if (booking.Status == BookingStatus.REJECTED)
            {
                trans.Type = TransactionType.REFUND;
                trans.Description = $"Hoàn tiền đặt sân tại {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào {booking.Date.ToString("dd/MM/yyyy")} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                trans.ReciverId = booking.UserId;
            }
            else if (booking.Status == BookingStatus.ACCEPTED)
            {
                trans.Type = TransactionType.BOOKING;
                trans.Description = $"Chuyển tiền đặt sân {booking.PartialField.Field.Name} {booking.Date} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                trans.ReciverId = booking.PartialField.Field.OwnerId;

                var bookingOverlap = await _unitOfWork.BookingRepository.GetAllAsync(b => EF.Functions.Like(b.Status, BookingStatus.WAITING) &&
                                                                                        b.PartialFieldId == booking.PartialFieldId &&
                                                                                        b.Date == booking.Date &&
                                                                                        b.StartTime < booking.EndTime && b.EndTime > booking.StartTime &&
                                                                                        !(b.PartialField.Field.IsDeleted ?? false) &&
                                                                                        EF.Functions.Like(b.PartialField.Field.Status, FieldStatus.ACCEPTED),
                                                                                        x => x.User,
                                                                                        p => p.PartialField.Field.Owner,
                                                                                        t => t.Team,
                                                                                        x => x.Rates,
                                                                                        x => x.OpponentFindings
                                                                                    );
                foreach (var bookingItem in bookingOverlap)
                {
                    HandleStatusRequest request = new HandleStatusRequest
                    {
                        Id = bookingItem.Id,
                        Status = BookingStatus.REJECTED,
                    };
                    await HandleStatusAsync(request);
                }
            }
            else if (booking.Status == BookingStatus.CANCELED)
            {
                trans.Type = TransactionType.BOOKING;
                trans.Description = $"Hoàn tiền đặt sân tại {booking.PartialField.Name} của {booking.PartialField.Field.Name} vào {booking.Date.ToString("dd/MM/yyyy")} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                trans.ReciverId = booking.UserId;
            }
            else if (booking.Status == BookingStatus.WAITING)
            {
                trans.Type = TransactionType.BOOKING;
                trans.Description = $"Đặt sân {booking.PartialField.Field.Name} {booking.Date} {GetTimeString(booking.StartTime)} - {GetTimeString(booking.EndTime)}";
                trans.UserId = booking.UserId;
                trans.ReciverId = 1;
            }

            await _unitOfWork.TransactionRepository.AddAsync(trans);
            await _unitOfWork.CommitAsync();
        }

        private string GetTimeString(int minutes)
        {
            return TimeOnly.MinValue.AddMinutes(minutes / 60).ToString("HH:mm");
        }

        public async Task SoftDeleteBookingAsync(int id)
        {
            //var booking = await _unitOfWork.BookingRepository.GetAsync(b => b.Id == id,
            //                                                            x => x.User,
            //                                                            p => p.PartialField.Field.Owner,
            //                                                            p => p.PartialField.Field.Rates,
            //                                                            t => t.Team);
            //if (booking == null)
            //    throw new NotFoundException("Booking not exist!");

            //if (booking.Status.ToLower().Equals(BookingStatus.WAITING.ToLower()))
            //{
            //    var trans = await _unitOfWork.TransactionRepository.GetAsync(t => t.BookingId == id);
            //    trans.Status = TransactionStatus.SUCCESSFUL;
            //    trans.Type = TransactionType.REFUND;
            //    trans.Description = "BOOKING DELETED!";

            //    _unitOfWork.TransactionRepository.Update(trans);
            //}

            //booking.IsDeleted = true;
            //_unitOfWork.BookingRepository.Update(booking);
            //await _unitOfWork.CommitAsync();
        }

        public async Task<BookingResponse> UpdateBookingAsync(BookingUpdateRequest request)
        {
            //var booking = await _unitOfWork.BookingRepository.GetAsync(b => b.Id == request.Id &&
            //                                                            !(b.PartialField.Field.IsDeleted ?? false) &&
            //                                                            b.PartialField.Field.Status == "ACCEPTED"
            //                                                            && b.Status.ToLower() == BookingStatus.WAITING.ToLower(),
            //                                                            x => x.User,
            //                                                            p => p.PartialField.Field.Rates,
            //                                                            p => p.PartialField.Field.Owner, t => t.Team,
            //                                                            x => x.Rates,
            //                                                            x => x.OpponentFindings);
            //if (booking == null)
            //    throw new NotFoundException("Booking not exist!");

            //var bookings = await _unitOfWork.BookingRepository.GetAllAsync(b => b.Id != booking.Id &&
            //                                                                !(b.PartialField.Field.IsDeleted ?? false) &&
            //                                                                b.PartialField.Field.Status == "ACCEPTED"
            //                                                                && b.PartialFieldId == booking.PartialFieldId
            //                                                                && b.Date == request.Date
            //                                                                && b.Status.ToLower() != BookingStatus.REJECTED.ToLower());

            //int start = request.StartTime ?? booking.StartTime;
            //int end = request.EndTime ?? booking.EndTime;
            //foreach (var item in bookings)
            //{
            //    if (IsOverlapping(item, start, end))
            //    {
            //        throw new ConflictException("The requested booking time overlaps with an existing booking.");
            //    }
            //}

            //var transUser = await _unitOfWork.TransactionRepository.GetAsync(t => t.BookingId == request.Id
            //                                                             && t.UserId == booking.UserId);

            //if (transUser == null)
            //    throw new ConflictException("Booking No Transaction.");

            //booking.Date = request.Date ?? booking.Date;
            //booking.StartTime = start;
            //booking.EndTime = end;
            //decimal amountRequest = (decimal)(booking.PartialField.Deposit ?? 0) * ((decimal)(end - start) / Timing.HOUR) * booking.PartialField.Amount;
            //decimal amountRequire = booking.DepositAmount - amountRequest;

            //decimal userAmount = await _transactionService.GetAmountAsync(booking.UserId);

            //if (amountRequire > userAmount)
            //    throw new ConflictException("Amount not enough!");

            //booking.DepositAmount = amountRequest;
            //transUser.Amount = -amountRequest;
            //_unitOfWork.BookingRepository.Update(booking);
            //_unitOfWork.TransactionRepository.Update(transUser);
            //if (await _unitOfWork.CommitAsync() > 0)
            //{
            //    return _mapper.Map<BookingResponse>(booking);
            //}
            //throw new ConflictException("The requested booking failed!");
            return null;
        }

        private bool IsOverlapping(Booking existingBooking, int start, int end)
        {
            return start < existingBooking.EndTime && end > existingBooking.StartTime;
        }

        public async Task VerifyPaymentWebhookData(WebhookType hook)
        {
            var webhookData = _payOSPaymentService.verifyPaymentWebhookData(hook);
            if (webhookData.code == "00")
            {
                HandleStatusRequest request = new HandleStatusRequest();
                request.Id = unchecked((int)webhookData.orderCode);
                request.Status = BookingStatus.ACCEPTED;
                await this.HandleStatusAsync(request);
            }
        }

        public async Task<RepositoryPaginationResponse<BookingResponse>> GetFutureBookingWithStatusWaitingOrAccepted(int uid, Pagination pagination)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            var currentTimeInSeconds = (int)DateTime.Now.TimeOfDay.TotalSeconds;

            var bookings = await _unitOfWork.BookingRepository.GetListAsync(b => b.UserId == uid &&
                                                                                (b.Status.ToLower().Equals(BookingStatus.WAITING.ToLower()) ||
                                                                                 b.Status.ToLower().Equals(BookingStatus.ACCEPTED.ToLower())) &&
                                                                                (b.Date > currentDate ||
                                                                                 (b.Date == currentDate && b.StartTime > currentTimeInSeconds)),
                                                                            pagination.Limit, pagination.Offset,
                                                                            x => x.User,
                                                                            p => p.PartialField.Field.Owner,
                                                                            p => p.PartialField.Field.Rates,
                                                                            t => t.Team);

            return new RepositoryPaginationResponse<BookingResponse>
            {
                Data = _mapper.Map<IEnumerable<BookingResponse>>(bookings.Data),
                Total = bookings.Total
            };
        }

        public async Task<IEnumerable<BookingResponse>> GetListByFieldAndDatesAsync(GetBookingByFieldAndDatesRequest request)
        {
            if (request.EndDate.DayNumber - request.StartDate.DayNumber > 7)
            {
                throw new ConflictException("Số ngày vượt quá số ngày hệ thống cho phép. Hãy lấy thời gian trong khoảng 7 ngày!");
            }
            var bookings = await _unitOfWork.BookingRepository
            .GetAllAsync(x =>
                (
                    (x.Date >= (request.StartDate) && x.Date <= (request.EndDate))
                ) &&
                (x.PartialField.FieldId == request.FieldId) &&
                !(x.PartialField.Field.IsDeleted ?? false) &&
                x.PartialField.Field.Status == "ACCEPTED" &&
                (x.Status.ToLower().Equals(BookingStatus.ACCEPTED.ToLower()) || (x.Status.ToLower().Equals(BookingStatus.WAITING.ToLower()))),
                x => x.User,
                x => x.PartialField.Field.Owner);

            return _mapper.Map<IEnumerable<BookingResponse>>(bookings);
        }
    }
}