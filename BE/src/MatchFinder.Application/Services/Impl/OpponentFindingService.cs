using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Constants;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Enums;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services.Impl
{
    public class OpponentFindingService : IOpponentFindingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public OpponentFindingService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        public async Task<OpponentFindingResponse> CreateNewOpponentFinding(OpponentFindingCreateRequest request, int userRequestId)
        {
            var booking = await _unitOfWork.BookingRepository.GetAsync(x => x.Id == request.BookingId
                                                                        && x.UserId == userRequestId
                                                                        && x.Status == BookingStatus.ACCEPTED,
                                                                        x => x.PartialField,
                                                                        x => x.PartialField.Field);
            if (booking == null)
                throw new ConflictException("Booking is invalid");

            var existBooking = await _unitOfWork.OpponentFindingRepository.GetAsync(x => x.BookingId == request.BookingId && x.Status != OpponentFindingStatus.CANCELLED);
            if (existBooking != null)
                throw new ConflictException("Opponent finding already exist");

            var currentDateTime = DateTime.Now;
            var bookingEndDateTime = booking.Date.ToDateTime(new TimeOnly(booking.EndTime / 3600, (booking.EndTime % 3600) / 60, booking.EndTime % 60));

            if (bookingEndDateTime <= currentDateTime)
                throw new ConflictException("Booking end time is in the past, cannot create opponent finding");

            var isOverlapWithAcceptedRequests = await RequestAcceptedOverlap(userRequestId, booking.Date, booking.StartTime, booking.EndTime);
            if (isOverlapWithAcceptedRequests.Any())
                throw new ConflictException("Opponent finding is overlapped with other accepted requests");

            var isOverlapWithOtherFinding = await OpponentFindingOverlap(userRequestId, booking.Date, booking.StartTime, booking.EndTime);
            if (isOverlapWithOtherFinding.Any())
                throw new ConflictException("Opponent finding is overlapped with other opponent finding");

            var opponentFinding = new OpponentFinding
            {
                Content = request.Content,
                UserFindingId = userRequestId,
                Status = OpponentFindingStatus.FINDING,
                BookingId = request.BookingId,
                Booking = booking,
                FieldId = booking.PartialField.FieldId,
                Field = booking.PartialField.Field
            };

            await _unitOfWork.OpponentFindingRepository.AddAsync(opponentFinding);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<OpponentFindingResponse>(opponentFinding);
            }
            throw new ConflictException("Create new opponent finding failed");
        }

        public async Task<OpponentFindingResponse> CreateNewOpponentFindingNotBooking(OpponentFindingNotBookingCreateRequest request, int userRequestId)
        {
            var userFinding = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userRequestId && x.Status == UserStatus.ACTIVE);
            if (userFinding == null)
                throw new NotFoundException("User not found");
            var currentDateTime = DateTime.Now;
            var opponentFindingDateTime = request.Date.ToDateTime(new TimeOnly(request.EndTime / 3600, (request.EndTime % 3600) / 60, request.EndTime % 60));

            if (opponentFindingDateTime <= currentDateTime)
                throw new ConflictException("Opponent finding end time is in the past, cannot create opponent finding");

            var isOverlapWithAcceptedRequests = await RequestAcceptedOverlap(userRequestId, request.Date, request.StartTime, request.EndTime);
            if (isOverlapWithAcceptedRequests.Any())
                throw new ConflictException("Opponent finding is overlapped with other accepted requests");

            var isOverlapWithOtherFinding = await OpponentFindingOverlap(userRequestId, request.Date, request.StartTime, request.EndTime);
            if (isOverlapWithOtherFinding.Any())
                throw new ConflictException("Opponent finding is overlapped with other opponent finding");

            var opponentFinding = new OpponentFinding
            {
                Content = request.Content,
                UserFindingId = userRequestId,
                UserFinding = userFinding,
                Status = OpponentFindingStatus.FINDING,
                FieldName = request.FieldName,
                FieldAddress = request.FieldAddress,
                FieldProvince = request.FieldProvince,
                FieldDistrict = request.FieldDistrict,
                FieldCommune = request.FieldCommune,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Date = request.Date
            };

            await _unitOfWork.OpponentFindingRepository.AddAsync(opponentFinding);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<OpponentFindingResponse>(opponentFinding);
            }
            throw new ConflictException("Create new opponent finding failed");
        }

        public async Task<IEnumerable<OpponentFindingResponse>> CheckOpponentFindingExisted(int userId, DateOnly date, int startTime, int endTime)
        {
            var overlapWithOtherAcceptedRequest = await OpponentFindingOverlap(userId, date, startTime, endTime);

            return _mapper.Map<IEnumerable<OpponentFindingResponse>>(overlapWithOtherAcceptedRequest);
        }

        public async Task<IEnumerable<OpponentFindingResponse>> CheckOpponentFindingAccepted(int userId, DateOnly date, int startTime, int endTime)
        {
            var opponentFindingRequest = await OpponentAcceptFindingOverlap(userId, date, startTime, endTime);

            return _mapper.Map<IEnumerable<OpponentFindingResponse>>(opponentFindingRequest);
        }

        public async Task<OpponentFindingResponseWithUserRequesting> CheckRequestWasAccepted(int userId, DateOnly date, int startTime, int endTime)
        {
            var opponentFindingRequest = await RequestsOverlapOtherRequests(userId, date, startTime, endTime);
            var acceptedRequest = opponentFindingRequest.FirstOrDefault(x => x.OpponentFinding.Status == OpponentFindingStatus.ACCEPTED);

            return _mapper.Map<OpponentFindingResponseWithUserRequesting>(acceptedRequest);
        }

        public async Task<OpponentFindingResponse> GetOpponentFinding(int id)
        {
            var opponentFinding = await _unitOfWork.OpponentFindingRepository.GetAsync(x => x.Id == id,
                                                                                       x => x.UserFinding,
                                                                                       x => x.Booking,
                                                                                       x => x.Field,
                                                                                       x => x.OpponentFindingRequests);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            var opponentFindingResponse = _mapper.Map<OpponentFindingResponse>(opponentFinding);

            if (opponentFindingResponse.Status == OpponentFindingStatus.OVERLAPPED_CANCELLED)
            {
                var date = opponentFinding.Booking == null ? opponentFinding.Date : opponentFinding.Booking.Date;
                var startTime = opponentFinding.Booking == null ? opponentFinding.StartTime : opponentFinding.Booking.StartTime;
                var endTime = opponentFinding.Booking == null ? opponentFinding.EndTime : opponentFinding.Booking.EndTime;

                var opponentFindingOverlapped = await OpponentFindingOverlap(opponentFinding.UserFindingId, date.Value, startTime.Value, endTime.Value);
                opponentFindingOverlapped = opponentFindingOverlapped.Where(x => x.Id != id).ToList();
                var requestAccepted = await RequestAcceptedOverlap(opponentFinding.UserFindingId, date.Value, startTime.Value, endTime.Value);

                if (!opponentFindingOverlapped.Any() && !requestAccepted.Any())
                {
                    opponentFindingResponse.IsCanRestore = true;
                }
            }

            return opponentFindingResponse;
        }

        public async Task<OpponentFindingResponse> UpdateOpponentFinding(int id, OpponentFindingUpdateRequest request)
        {
            var opponentFinding = await _unitOfWork.OpponentFindingRepository.GetAsync(x => x.Id == id && x.Status == OpponentFindingStatus.FINDING,
                                                                                        x => x.UserFinding,
                                                                                        x => x.Booking,
                                                                                        x => x.Field,
                                                                                        x => x.OpponentFindingRequests);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (opponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            opponentFinding.Content = request.Content ?? opponentFinding.Content;

            _unitOfWork.OpponentFindingRepository.Update(opponentFinding);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<OpponentFindingResponse>(opponentFinding);
            }
            throw new ConflictException("Update opponent finding failed");
        }

        public async Task<OpponentFindingResponse> CancelPost(int id, int userFindingId)
        {
            var opponentFinding = await _unitOfWork.OpponentFindingRepository.GetAsync(x => x.Id == id && x.UserFindingId == userFindingId
                                                                                        && x.Status != OpponentFindingStatus.CANCELLED,
                                                                                        x => x.UserFinding,
                                                                                        x => x.Booking,
                                                                                        x => x.Field,
                                                                                        x => x.OpponentFindingRequests);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (opponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            if (opponentFinding.Status == OpponentFindingStatus.ACCEPTED)
            {
                var acceptedRequest = await _unitOfWork.OpponentFindingRequestRepository.GetAsync(x => x.OpponentFindingId == id && x.IsAccepted);
                acceptedRequest.Status = OpponentFindingRequestStatus.CANCELLED;
                acceptedRequest.IsAccepted = false;
                _unitOfWork.OpponentFindingRequestRepository.Update(acceptedRequest);
            }

            opponentFinding.Status = OpponentFindingStatus.CANCELLED;
            _unitOfWork.OpponentFindingRepository.Update(opponentFinding);

            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<OpponentFindingResponse>(opponentFinding);
            }
            throw new ConflictException("Cancel opponent finding failed");
        }

        public async Task<OpponentFindingResponse> CanceledMatching(int id, int userFindingId)
        {
            var opponentFinding = await _unitOfWork.OpponentFindingRepository.GetAsync(x => x.Id == id && x.UserFindingId == userFindingId && x.Status == OpponentFindingStatus.ACCEPTED,
                                                                                        x => x.UserFinding,
                                                                                        x => x.Booking,
                                                                                        x => x.Field,
                                                                                        x => x.OpponentFindingRequests);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (opponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            opponentFinding.Status = OpponentFindingStatus.FINDING;

            _unitOfWork.OpponentFindingRepository.Update(opponentFinding);

            var requestAccepted = await _unitOfWork.OpponentFindingRequestRepository.GetAsync(x => x.OpponentFindingId == id && x.IsAccepted);
            requestAccepted.IsAccepted = false;
            requestAccepted.Status = OpponentFindingRequestStatus.CANCELLED;
            _unitOfWork.OpponentFindingRequestRepository.Update(requestAccepted);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                var date = opponentFinding.Booking == null ? opponentFinding.Date : opponentFinding.Booking.Date;
                var startTime = opponentFinding.Booking == null ? opponentFinding.StartTime : opponentFinding.Booking.StartTime;
                var endTime = opponentFinding.Booking == null ? opponentFinding.EndTime : opponentFinding.Booking.EndTime;
                var receiverIds = new List<int> { requestAccepted.UserRequestingId };

                var notification = new Notification()
                {
                    Title = "Huỷ kèo ghép đối",
                    Content = $"{opponentFinding.UserFinding.UserName} đã huỷ kèo ghép đối vào ngày {date?.ToString("dd/MM/yyyy")} từ {GetTimeString(startTime ?? 0)} đến {GetTimeString(endTime ?? 0)}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return _mapper.Map<OpponentFindingResponse>(opponentFinding);
            }
            throw new ConflictException("Cancel opponent finding failed");
        }

        public async Task<OpponentFindingResponse> RestoreFinding(int oldOpponentFindingId, int userRequestId)
        {
            var opponentFinding = await _unitOfWork
                .OpponentFindingRepository
                .GetAsync(x => x.Id == oldOpponentFindingId
                && x.UserFindingId == userRequestId,
                x => x.UserFinding,
                x => x.Booking,
                x => x.Field,
                x => x.OpponentFindingRequests);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (opponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            if (opponentFinding.Status != OpponentFindingStatus.OPPONENT_CANCELLED)
            {
                throw new ConflictException("Can't restore opponent finding post because it is not cancelled");
            }

            opponentFinding.Status = OpponentFindingStatus.FINDING;
            _unitOfWork.OpponentFindingRepository.Update(opponentFinding);

            var tempList = await GetListRequestAvailableRestore(userRequestId, oldOpponentFindingId);
            if (tempList.Any())
            {
                foreach (var item in tempList)
                {
                    item.Status = null;
                    _unitOfWork.OpponentFindingRequestRepository.Update(item);
                }
            }

            await _unitOfWork.CommitAsync();

            return _mapper.Map<OpponentFindingResponse>(opponentFinding);
        }

        public async Task<IEnumerable<OpponentFindingResponse>> RestoreOverFindings(int oldOpponentFindingId, int userRequestId)
        {
            var tempList = await GetListPostAvailableRestore(oldOpponentFindingId, userRequestId);
            if (tempList.Any())
            {
                foreach (var item in tempList)
                {
                    item.Status = OpponentFindingStatus.FINDING;
                    _unitOfWork.OpponentFindingRepository.Update(item);
                }
            }
            await _unitOfWork.CommitAsync();

            return _mapper.Map<IEnumerable<OpponentFindingResponse>>(tempList);
        }

        public async Task<OpponentFindingResponse> RestoreOverlapPost(int opponentFindingId, int userRequestId)
        {
            var opponentFinding = await _unitOfWork
                .OpponentFindingRepository
                .GetAsync(x => x.Id == opponentFindingId
                && x.UserFindingId == userRequestId,
                x => x.UserFinding,
                x => x.Booking);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (opponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            if (opponentFinding.Status != OpponentFindingStatus.OVERLAPPED_CANCELLED)
            {
                throw new ConflictException("Can't restore opponent finding post because it is not cancelled");
            }

            var date = opponentFinding.Booking == null ? opponentFinding.Date : opponentFinding.Booking.Date;
            var startTime = opponentFinding.Booking == null ? opponentFinding.StartTime : opponentFinding.Booking.StartTime;
            var endTime = opponentFinding.Booking == null ? opponentFinding.EndTime : opponentFinding.Booking.EndTime;

            var opponentFindingOverlapped = await OpponentFindingOverlap(opponentFinding.UserFindingId, date.Value, startTime.Value, endTime.Value);
            opponentFindingOverlapped = opponentFindingOverlapped.Where(x => x.Id != opponentFindingId).ToList();
            var requestAccepted = await RequestAcceptedOverlap(opponentFinding.UserFindingId, date.Value, startTime.Value, endTime.Value);

            if (opponentFindingOverlapped.Any() || requestAccepted.Any())
            {
                throw new ConflictException("Can't restore opponent finding post because it is overlapped with other accepted requests or post");
            }

            opponentFinding.Status = OpponentFindingStatus.FINDING;
            _unitOfWork.OpponentFindingRepository.Update(opponentFinding);

            await _unitOfWork.CommitAsync();

            return _mapper.Map<OpponentFindingResponse>(opponentFinding);
        }

        public async Task<IEnumerable<OpponentFindingResponse>> CheckOverlapPost(int oldOpponentFindingId, int userRequestId)
        {
            var tempList = await GetListPostAvailableRestore(oldOpponentFindingId, userRequestId);

            return _mapper.Map<IEnumerable<OpponentFindingResponse>>(tempList);
        }

        private async Task<IEnumerable<OpponentFinding>> GetListPostAvailableRestore(int oldOpponentFindingId, int userRequestId)
        {
            var oldOpponentFinding = await _unitOfWork
                .OpponentFindingRepository
                .GetAsync(x => x.Id == oldOpponentFindingId,
                        x => x.UserFinding,
                        x => x.Booking,
                        x => x.Field,
                        x => x.OpponentFindingRequests);

            if (oldOpponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (oldOpponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            var date = oldOpponentFinding.Booking == null ? oldOpponentFinding.Date : oldOpponentFinding.Booking.Date;
            var startTime = oldOpponentFinding.Booking == null ? oldOpponentFinding.StartTime : oldOpponentFinding.Booking.StartTime;
            var endTime = oldOpponentFinding.Booking == null ? oldOpponentFinding.EndTime : oldOpponentFinding.Booking.EndTime;

            var previousCanceledPost = await OpponentFindingCancelOverlap(userRequestId, date.Value, startTime.Value, endTime.Value);
            previousCanceledPost = previousCanceledPost.Where(x => x.Id != oldOpponentFindingId);

            var tempList = previousCanceledPost.ToList();

            if (previousCanceledPost.Any())
            {
                for (int i = 0; i < tempList.Count(); i++)
                {
                    var dateCheck = tempList[i].Booking == null ? tempList[i].Date : tempList[i].Booking.Date;
                    var startTimeCheck = tempList[i].Booking == null ? tempList[i].StartTime : tempList[i].Booking.StartTime;
                    var endTimeCheck = tempList[i].Booking == null ? tempList[i].EndTime : tempList[i].Booking.EndTime;

                    var isOverlapWithAcceptedRequests = await RequestAcceptedOverlap(userRequestId, dateCheck.Value, startTimeCheck.Value, endTimeCheck.Value);
                    var isOverlapWithOtherFinding = await OpponentAcceptFindingOverlap(userRequestId, dateCheck.Value, startTimeCheck.Value, endTimeCheck.Value);
                    if (isOverlapWithAcceptedRequests.Any() || isOverlapWithOtherFinding.Any())
                    {
                        tempList.Remove(tempList[i]);
                    }
                }
            }

            return tempList;
        }

        private async Task<IEnumerable<OpponentFinding>> OpponentFindingCancelOverlap(int userId, DateOnly date, int startTime, int endTime)
        {
            var opponentFindingOverlap = await _unitOfWork
                .OpponentFindingRepository
                .GetAllAsync(x => x.UserFindingId == userId
                            && (x.Status == OpponentFindingStatus.OVERLAPPED_CANCELLED)
                            && (x.Date == date
                                && ((x.StartTime <= startTime && x.EndTime >= startTime)
                                || (x.StartTime <= endTime && x.EndTime >= endTime)
                                || (x.StartTime >= startTime && x.EndTime <= endTime)
                                || (x.StartTime <= startTime && x.EndTime >= endTime))
                            || x.Booking.Date == date
                                && ((x.Booking.StartTime <= startTime && x.Booking.EndTime >= startTime)
                                || (x.Booking.StartTime <= endTime && x.Booking.EndTime >= endTime)
                                || (x.Booking.StartTime >= startTime && x.Booking.EndTime <= endTime)
                                || (x.Booking.StartTime <= startTime && x.Booking.EndTime >= endTime))),
                            x => x.Booking,
                            x => x.UserFinding,
                            x => x.Field,
                            x => x.OpponentFindingRequests);

            return opponentFindingOverlap;
        }

        private async Task<IEnumerable<OpponentFindingRequest>> RequestCancelOverlap(int userId, DateOnly date, int startTime, int endTime)
        {
            var requestCancelOverlap = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAllAsync(x => x.UserRequestingId == userId
                            && x.Status == OpponentFindingRequestStatus.OVERLAPPED_CANCELLED
                            && (x.OpponentFinding.Date == date
                                && ((x.OpponentFinding.StartTime <= startTime && x.OpponentFinding.EndTime >= startTime)
                                || (x.OpponentFinding.StartTime <= endTime && x.OpponentFinding.EndTime >= endTime)
                                || (x.OpponentFinding.StartTime >= startTime && x.OpponentFinding.EndTime <= endTime)
                                || (x.OpponentFinding.StartTime <= startTime && x.OpponentFinding.EndTime >= endTime))
                            || x.OpponentFinding.Booking.Date == date
                                && ((x.OpponentFinding.Booking.StartTime <= startTime && x.OpponentFinding.Booking.EndTime >= startTime)
                                || (x.OpponentFinding.Booking.StartTime <= endTime && x.OpponentFinding.Booking.EndTime >= endTime)
                                || (x.OpponentFinding.Booking.StartTime >= startTime && x.OpponentFinding.Booking.EndTime <= endTime)
                                || (x.OpponentFinding.Booking.StartTime <= startTime && x.OpponentFinding.Booking.EndTime >= endTime))),
                            x => x.UserRequesting,
                            x => x.OpponentFinding,
                            x => x.OpponentFinding.Booking);

            return requestCancelOverlap;
        }

        public async Task<RepositoryPaginationResponse<OpponentFindingResponse>> GetRepositoryPagination(OpponentFindingFilterRequest filterRequest)
        {
            var opponentFindings = await _unitOfWork
                .OpponentFindingRepository
                .GetListAsync(x =>
                                (string.IsNullOrEmpty(filterRequest.FieldName) || x.Field.Name.Contains(filterRequest.FieldName) || x.FieldName.Contains(filterRequest.FieldName)) &&
                                (string.IsNullOrEmpty(filterRequest.Province) || x.Field.Province.Contains(filterRequest.Province) || x.FieldProvince.Contains(filterRequest.Province)) &&
                                (string.IsNullOrEmpty(filterRequest.District) || x.Field.District.Contains(filterRequest.District) || x.FieldDistrict.Contains(filterRequest.District)) &&
                                (string.IsNullOrEmpty(filterRequest.Commune) || x.Field.Commune.Contains(filterRequest.Commune) || x.FieldCommune.Contains(filterRequest.Commune)) &&
                                (string.IsNullOrEmpty(filterRequest.Address) || x.Field.Address.Contains(filterRequest.Address) || x.FieldAddress.Contains(filterRequest.Address)) &&
                                (!filterRequest.FromTime.HasValue || x.Booking.StartTime >= filterRequest.FromTime || x.StartTime >= filterRequest.FromTime) &&
                                (!filterRequest.ToTime.HasValue || x.Booking.EndTime <= filterRequest.ToTime || x.EndTime <= filterRequest.ToTime) &&
                                (!filterRequest.FromDate.HasValue || x.Booking.Date >= filterRequest.FromDate || x.Date >= filterRequest.FromDate) &&
                                (!filterRequest.ToDate.HasValue || x.Booking.Date <= filterRequest.ToDate || x.Date <= filterRequest.ToDate) &&
                                (string.IsNullOrEmpty(filterRequest.Status) || x.Status == filterRequest.Status),
                                filterRequest.Limit,
                                filterRequest.Offset,
                                x => x.UserFinding,
                                x => x.Booking,
                                x => x.Field,
                                x => x.OpponentFindingRequests);

            return new RepositoryPaginationResponse<OpponentFindingResponse>
            {
                Total = opponentFindings.Total,
                Data = _mapper.Map<IEnumerable<OpponentFindingResponse>>(opponentFindings.Data)
            };
        }

        public async Task<OpponentFindingResponseWithUserRequesting> RequestOpponentFinding(int userRequestId, RequestingOpponentFindingRequest request)
        {
            var opponentFinding = await _unitOfWork.OpponentFindingRepository.GetAsync(x => x.Id == request.OpponentFindingId &&
                                                                                            x.Status == OpponentFindingStatus.FINDING,
                                                                                            x => x.Booking);
            if (opponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (opponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            var userRequesting = await _unitOfWork.UserRepository.GetAsync(x => x.Id == userRequestId &&
                                                                                x.Id != opponentFinding.UserFindingId);
            if (userRequesting == null)
            {
                throw new NotFoundException("User requesting not found");
            }

            var opponentRequestExist = await _unitOfWork.OpponentFindingRequestRepository.GetAsync(x => x.UserRequestingId == userRequestId &&
                                                                                                        x.OpponentFindingId == request.OpponentFindingId);
            if (opponentRequestExist != null)
            {
                throw new ConflictException("Request opponent finding already exist");
            }

            var date = opponentFinding.Booking == null ? opponentFinding.Date : opponentFinding.Booking.Date;
            var startTime = opponentFinding.Booking == null ? opponentFinding.StartTime : opponentFinding.Booking.StartTime;
            var endTime = opponentFinding.Booking == null ? opponentFinding.EndTime : opponentFinding.Booking.EndTime;

            var isOverlapWithAcceptedRequests = await RequestAcceptedOverlap(userRequestId, date.Value, startTime.Value, endTime.Value);
            if (isOverlapWithAcceptedRequests.Any())
                throw new ConflictException("Request opponent finding is overlapped with other requests");

            var isOverlapWithOtherFinding = await OpponentAcceptFindingOverlap(userRequestId, date.Value, startTime.Value, endTime.Value);
            if (isOverlapWithOtherFinding.Any())
                throw new ConflictException("Request opponent finding is overlapped with other accepted opponent finding");

            var opponentFindingRequest = new OpponentFindingRequest
            {
                UserRequestingId = userRequestId,
                UserRequesting = userRequesting,
                OpponentFindingId = request.OpponentFindingId,
                OpponentFinding = opponentFinding,
                Message = request.Message
            };

            await _unitOfWork.OpponentFindingRequestRepository.AddAsync(opponentFindingRequest);

            if (await _unitOfWork.CommitAsync() > 0)
            {
                var response = _mapper.Map<OpponentFindingResponseWithUserRequesting>(opponentFindingRequest);
                var receiverIds = new List<int> { opponentFindingRequest.OpponentFinding.UserFindingId };

                var notification = new Notification()
                {
                    Title = "Yêu cầu ghép đối",
                    Content = $"{response.UserRequestingName} đã gửi yêu cầu ghép đối vào ngày {date?.ToString("dd/MM/yyyy")} từ {GetTimeString(startTime ?? 0)} đến {GetTimeString(endTime ?? 0)}"
                };
                await _notificationService.SendNotificationToListUser(receiverIds, notification);
                return response;
            }
            throw new ConflictException("Request opponent finding failed");
        }
        private string GetTimeString(int minutes)
        {
            return TimeOnly.MinValue.AddMinutes(minutes / 60).ToString("HH:mm");
        }

        public async Task<IEnumerable<OpponentFindingResponseWithUserRequesting>> GetListUserRequestByOpponentFindingId(int opponentFindingId, RequestingOpponentFindingFilter filter)
        {
            var opponentFindingRequests = await _unitOfWork
                .OpponentFindingRequestRepository.GetListUserRequestByOpponentFindingId(opponentFindingId, filter.Offset, filter.Limit, filter.IsSortDescByCreatedAt);

            return _mapper.Map<IEnumerable<OpponentFindingResponseWithUserRequesting>>(opponentFindingRequests);
        }

        public async Task<OpponentFindingResponseWithUserRequesting> AcceptRequestOpponentFinding(int id)
        {
            var opponentFindingRequest = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAsync(x => x.Id == id
                        && x.OpponentFinding.Status == OpponentFindingStatus.FINDING
                        && !x.IsAccepted && string.IsNullOrEmpty(x.Status),
                        x => x.UserRequesting,
                        x => x.OpponentFinding,
                        x => x.OpponentFinding.Booking);

            if (opponentFindingRequest == null)
            {
                throw new NotFoundException("Opponent finding request not found");
            }

            if (opponentFindingRequest.OpponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding request is overdue");
            }

            if (!string.IsNullOrEmpty(opponentFindingRequest.Status))
            {
                throw new ConflictException("Opponent finding request was cancelled before");
            }

            opponentFindingRequest.OpponentFinding.Status = OpponentFindingStatus.ACCEPTED;
            opponentFindingRequest.IsAccepted = true;

            var opponentFinding = opponentFindingRequest.OpponentFinding;
            var date = opponentFinding.Booking == null ? opponentFinding.Date : opponentFinding.Booking.Date;
            var startTime = opponentFinding.Booking == null ? opponentFinding.StartTime : opponentFinding.Booking.StartTime;
            var endTime = opponentFinding.Booking == null ? opponentFinding.EndTime : opponentFinding.Booking.EndTime;

            var overlapOpponentFindingRequest = await RequestsOverlapOtherRequests(opponentFindingRequest.UserRequestingId, date.Value, startTime.Value, endTime.Value);
            overlapOpponentFindingRequest = overlapOpponentFindingRequest.Where(x => x.OpponentFindingId != opponentFindingRequest.OpponentFindingId && string.IsNullOrEmpty(x.Status)).ToList();
            foreach (var item in overlapOpponentFindingRequest)
            {
                item.Status = OpponentFindingRequestStatus.OVERLAPPED_CANCELLED;
                _unitOfWork.OpponentFindingRequestRepository.Update(item);
            }
            var opponentFindingOverlapped = await OpponentFindingOverlap(opponentFindingRequest.UserRequestingId, date.Value, startTime.Value, endTime.Value);
            foreach (var item in opponentFindingOverlapped)
            {
                if (item != null)
                {
                    item.Status = OpponentFindingStatus.OVERLAPPED_CANCELLED;
                    _unitOfWork.OpponentFindingRepository.Update(item);
                }
            }
            await _unitOfWork.CommitAsync();
            var response = _mapper.Map<OpponentFindingResponseWithUserRequesting>(opponentFindingRequest);
            var receiverIds = new List<int> { opponentFindingRequest.UserRequestingId };

            var notification = new Notification()
            {
                Title = "Chấp nhận ghép đối",
                Content = $"{opponentFindingRequest.OpponentFinding.UserFinding.UserName} đã chấp nhận yêu cầu ghép đối vào ngày {date?.ToString("dd/MM/yyyy")} từ {GetTimeString(startTime ?? 0)} đến {GetTimeString(endTime ?? 0)}"
            };
            await _notificationService.SendNotificationToListUser(receiverIds, notification);
            return response;
        }

        public async Task<OpponentFindingResponseWithUserRequesting> GetMyRequest(int opponentFindingId, int userId)
        {
            var opponentFindingRequest = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAsync(x => x.OpponentFindingId == opponentFindingId && x.UserRequestingId == userId,
                            x => x.UserRequesting,
                            x => x.OpponentFinding);

            if (opponentFindingRequest == null)
            {
                return null;
            }

            var opponentFindingRequestResponse = _mapper.Map<OpponentFindingResponseWithUserRequesting>(opponentFindingRequest);

            if (opponentFindingRequest.Status == OpponentFindingRequestStatus.OVERLAPPED_CANCELLED)
            {
                var date = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.Date : opponentFindingRequest.OpponentFinding.Booking.Date;
                var startTime = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.StartTime : opponentFindingRequest.OpponentFinding.Booking.StartTime;
                var endTime = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.EndTime : opponentFindingRequest.OpponentFinding.Booking.EndTime;

                var acceptedOpponentFinding = await OpponentAcceptFindingOverlap(opponentFindingRequest.UserRequestingId, date.Value, startTime.Value, endTime.Value);
                var acceptedRequest = await RequestAcceptedOverlap(opponentFindingRequest.UserRequestingId, date.Value, startTime.Value, endTime.Value);
                acceptedRequest = acceptedRequest.Where(x => x.Id != opponentFindingRequest.Id).ToList();

                if (!acceptedOpponentFinding.Any() && !acceptedRequest.Any())
                {
                    opponentFindingRequestResponse.IsCanRestore = true;
                }
            }
            return opponentFindingRequestResponse;
        }

        public async Task<RepositoryPaginationResponse<OpponentFindingResponse>> GetMyHistoryOpponentFinding(int userId, OpponentFindingFilterRequest filterRequest)
        {
            var opponentFindings = await _unitOfWork
                .OpponentFindingRepository
                .GetListAsync(x => x.UserFindingId == userId &&
                                (string.IsNullOrEmpty(filterRequest.FieldName) || x.Field.Name.Contains(filterRequest.FieldName) || x.FieldName.Contains(filterRequest.FieldName)) &&
                                (string.IsNullOrEmpty(filterRequest.Province) || x.Field.Province.Contains(filterRequest.Province) || x.FieldProvince.Contains(filterRequest.Province)) &&
                                (string.IsNullOrEmpty(filterRequest.District) || x.Field.District.Contains(filterRequest.District) || x.FieldDistrict.Contains(filterRequest.District)) &&
                                (string.IsNullOrEmpty(filterRequest.Commune) || x.Field.Commune.Contains(filterRequest.Commune) || x.FieldCommune.Contains(filterRequest.Commune)) &&
                                (string.IsNullOrEmpty(filterRequest.Address) || x.Field.Address.Contains(filterRequest.Address) || x.FieldAddress.Contains(filterRequest.Address)) &&
                                (!filterRequest.FromTime.HasValue || x.Booking.StartTime >= filterRequest.FromTime || x.StartTime >= filterRequest.FromTime) &&
                                (!filterRequest.ToTime.HasValue || x.Booking.EndTime <= filterRequest.ToTime || x.EndTime <= filterRequest.ToTime) &&
                                (!filterRequest.FromDate.HasValue || x.Booking.Date >= filterRequest.FromDate || x.Date >= filterRequest.FromDate) &&
                                (!filterRequest.ToDate.HasValue || x.Booking.Date <= filterRequest.ToDate || x.Date <= filterRequest.ToDate) &&
                                (string.IsNullOrEmpty(filterRequest.Status) || x.Status == filterRequest.Status),
                                filterRequest.Limit,
                                filterRequest.Offset,
                                x => x.UserFinding,
                                x => x.Booking,
                                x => x.Field,
                                x => x.OpponentFindingRequests);

            return new RepositoryPaginationResponse<OpponentFindingResponse>
            {
                Total = opponentFindings.Total,
                Data = _mapper.Map<IEnumerable<OpponentFindingResponse>>(opponentFindings.Data)
            };
        }

        public async Task<RepositoryPaginationResponse<OpponentFindingResponseWithUserRequesting>> GetMyHistoryRequestOpponentFinding(int userId, OpponentFindingFilterRequest filterRequest)
        {
            var opponentFindingRequests = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetListAsync(x => x.UserRequestingId == userId &&
                            (string.IsNullOrEmpty(filterRequest.FieldName) || x.OpponentFinding.Field.Name.Contains(filterRequest.FieldName) || x.OpponentFinding.FieldName.Contains(filterRequest.FieldName)) &&
                                (string.IsNullOrEmpty(filterRequest.Province) || x.OpponentFinding.Field.Province.Contains(filterRequest.Province) || x.OpponentFinding.FieldProvince.Contains(filterRequest.Province)) &&
                                (string.IsNullOrEmpty(filterRequest.District) || x.OpponentFinding.Field.District.Contains(filterRequest.District) || x.OpponentFinding.FieldDistrict.Contains(filterRequest.District)) &&
                                (string.IsNullOrEmpty(filterRequest.Commune) || x.OpponentFinding.Field.Commune.Contains(filterRequest.Commune) || x.OpponentFinding.FieldCommune.Contains(filterRequest.Commune)) &&
                                (string.IsNullOrEmpty(filterRequest.Address) || x.OpponentFinding.Field.Address.Contains(filterRequest.Address) || x.OpponentFinding.FieldAddress.Contains(filterRequest.Address)) &&
                                (!filterRequest.FromTime.HasValue || x.OpponentFinding.Booking.StartTime >= filterRequest.FromTime || x.OpponentFinding.StartTime >= filterRequest.FromTime) &&
                                (!filterRequest.ToTime.HasValue || x.OpponentFinding.Booking.EndTime <= filterRequest.ToTime || x.OpponentFinding.EndTime <= filterRequest.ToTime) &&
                                (!filterRequest.FromDate.HasValue || x.OpponentFinding.Booking.Date >= filterRequest.FromDate || x.OpponentFinding.Date >= filterRequest.FromDate) &&
                                (!filterRequest.ToDate.HasValue || x.OpponentFinding.Booking.Date <= filterRequest.ToDate || x.OpponentFinding.Date <= filterRequest.ToDate) &&
                                (string.IsNullOrEmpty(filterRequest.Status) || x.OpponentFinding.Status == filterRequest.Status),
                                filterRequest.Limit,
                                filterRequest.Offset,
                                x => x.UserRequesting,
                                x => x.OpponentFinding,
                                x => x.OpponentFinding.Booking);

            return new RepositoryPaginationResponse<OpponentFindingResponseWithUserRequesting>
            {
                Total = opponentFindingRequests.Total,
                Data = _mapper.Map<IEnumerable<OpponentFindingResponseWithUserRequesting>>(opponentFindingRequests.Data)
            };
        }

        public async Task<OpponentFindingResponseWithUserRequesting> CancelRequestOpponentFinding(int id)
        {
            var opponentFindingRequest = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAsync(x => x.Id == id
                && (x.OpponentFinding.Status == OpponentFindingStatus.FINDING
                    || x.OpponentFinding.Status == OpponentFindingStatus.ACCEPTED),
                        x => x.UserRequesting,
                        x => x.OpponentFinding,
                        x => x.OpponentFinding.Booking,
                        x => x.OpponentFinding.UserFinding);

            if (opponentFindingRequest == null)
            {
                throw new NotFoundException("Opponent finding request not found");
            }

            if (opponentFindingRequest.OpponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding request is overdue");
            }

            if (!string.IsNullOrEmpty(opponentFindingRequest.Status))
            {
                throw new ConflictException("Opponent finding request was cancelled before");
            }

            opponentFindingRequest.Status = OpponentFindingRequestStatus.SELF_CANCELLED;
            //opponentFindingRequest.IsAccepted = false;
            _unitOfWork.OpponentFindingRequestRepository.Update(opponentFindingRequest);

            if (opponentFindingRequest.OpponentFinding.Status == OpponentFindingStatus.ACCEPTED)
            {
                opponentFindingRequest.OpponentFinding.Status = OpponentFindingStatus.OPPONENT_CANCELLED;
                _unitOfWork.OpponentFindingRepository.Update(opponentFindingRequest.OpponentFinding);
            }

            await _unitOfWork.CommitAsync();
            var date = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.Date : opponentFindingRequest.OpponentFinding.Booking.Date;
            var startTime = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.StartTime : opponentFindingRequest.OpponentFinding.Booking.StartTime;
            var endTime = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.EndTime : opponentFindingRequest.OpponentFinding.Booking.EndTime;
            var receiverIds = new List<int> { opponentFindingRequest.OpponentFinding.UserFindingId };

            var notification = new Notification()
            {
                Title = "Huỷ kèo ghép đối",
                Content = $"{opponentFindingRequest.UserRequesting.UserName} đã huỷ kèo ghép đối vào ngày {date?.ToString("dd/MM/yyyy")} từ {GetTimeString(startTime ?? 0)} đến {GetTimeString(endTime ?? 0)}"
            };
            await _notificationService.SendNotificationToListUser(receiverIds, notification);
            return _mapper.Map<OpponentFindingResponseWithUserRequesting>(opponentFindingRequest);
        }

        public async Task<IEnumerable<OpponentFindingResponseWithUserRequesting>> RestoreRequestOpponentFinding(int userRequestId, int oldOpponentFindingId)
        {
            var tempList = await GetListRequestAvailableRestore(userRequestId, oldOpponentFindingId);
            if (tempList.Any())
            {
                foreach (var item in tempList)
                {
                    item.Status = null;
                    _unitOfWork.OpponentFindingRequestRepository.Update(item);
                }

                await _unitOfWork.CommitAsync();
            }
            return _mapper.Map<IEnumerable<OpponentFindingResponseWithUserRequesting>>(tempList);
        }

        public async Task<OpponentFindingResponseWithUserRequesting> RestoreOverlapRequest(int requestId, int userRequestId)
        {
            var opponentFindingRequest = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAsync(x => x.Id == requestId
                && x.UserRequestingId == userRequestId,
                x => x.UserRequesting,
                x => x.OpponentFinding);

            if (opponentFindingRequest == null)
            {
                throw new NotFoundException("Opponent finding request not found");
            }

            if (opponentFindingRequest.OpponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding request is overdue");
            }

            if (opponentFindingRequest.Status != OpponentFindingRequestStatus.OVERLAPPED_CANCELLED)
            {
                throw new ConflictException("Can't restore opponent finding request because it is not cancelled");
            }

            var date = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.Date : opponentFindingRequest.OpponentFinding.Booking.Date;
            var startTime = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.StartTime : opponentFindingRequest.OpponentFinding.Booking.StartTime;
            var endTime = opponentFindingRequest.OpponentFinding.Booking == null ? opponentFindingRequest.OpponentFinding.EndTime : opponentFindingRequest.OpponentFinding.Booking.EndTime;

            var acceptedOpponentFinding = await OpponentAcceptFindingOverlap(opponentFindingRequest.UserRequestingId, date.Value, startTime.Value, endTime.Value);
            var acceptedRequest = await RequestAcceptedOverlap(opponentFindingRequest.UserRequestingId, date.Value, startTime.Value, endTime.Value);
            acceptedRequest = acceptedRequest.Where(x => x.Id != requestId);

            if (acceptedOpponentFinding.Any() || acceptedRequest.Any())
            {
                throw new ConflictException("Can't restore opponent finding request because it is overlapped with other accepted requests or post");
            }

            opponentFindingRequest.Status = null;
            _unitOfWork.OpponentFindingRequestRepository.Update(opponentFindingRequest);

            await _unitOfWork.CommitAsync();
            return _mapper.Map<OpponentFindingResponseWithUserRequesting>(opponentFindingRequest);
        }

        public async Task<IEnumerable<OpponentFindingResponseWithUserRequesting>> CheckOverlapRequest(int userRequestId, int oldOpponentFindingId)
        {
            var listAvailableRestore = await GetListRequestAvailableRestore(userRequestId, oldOpponentFindingId);
            return _mapper.Map<IEnumerable<OpponentFindingResponseWithUserRequesting>>(listAvailableRestore);
        }

        private async Task<IEnumerable<OpponentFindingRequest>> GetListRequestAvailableRestore(int userRequestId, int oldOpponentFindingId)
        {
            var oldOpponentFinding = await _unitOfWork
                .OpponentFindingRepository
                .GetAsync(x => x.Id == oldOpponentFindingId,
                        x => x.UserFinding,
                        x => x.Booking,
                        x => x.Field,
                        x => x.OpponentFindingRequests);

            if (oldOpponentFinding == null)
            {
                throw new NotFoundException("Opponent finding not found");
            }

            if (oldOpponentFinding.IsOverdue)
            {
                throw new ConflictException("Opponent finding is overdue");
            }

            var date = oldOpponentFinding.Booking == null ? oldOpponentFinding.Date : oldOpponentFinding.Booking.Date;
            var startTime = oldOpponentFinding.Booking == null ? oldOpponentFinding.StartTime : oldOpponentFinding.Booking.StartTime;
            var endTime = oldOpponentFinding.Booking == null ? oldOpponentFinding.EndTime : oldOpponentFinding.Booking.EndTime;

            var previousCanceledPost = await RequestCancelOverlap(userRequestId, date.Value, startTime.Value, endTime.Value);
            previousCanceledPost = previousCanceledPost.Where(x => x.Id != oldOpponentFindingId
                                                                && x.OpponentFinding.Status == OpponentFindingStatus.FINDING);

            var tempList = previousCanceledPost.ToList();

            if (previousCanceledPost.Any())
            {
                for (int i = 0; i < tempList.Count(); i++)
                {
                    var dateCheck = tempList[i].OpponentFinding.Booking == null ? tempList[i].OpponentFinding.Date : tempList[i].OpponentFinding.Booking.Date;
                    var startTimeCheck = tempList[i].OpponentFinding.Booking == null ? tempList[i].OpponentFinding.StartTime : tempList[i].OpponentFinding.Booking.StartTime;
                    var endTimeCheck = tempList[i].OpponentFinding.Booking == null ? tempList[i].OpponentFinding.EndTime : tempList[i].OpponentFinding.Booking.EndTime;

                    var isOverlapWithAcceptedRequests = await RequestAcceptedOverlap(userRequestId, dateCheck.Value, startTimeCheck.Value, endTimeCheck.Value);
                    var isOverlapWithOtherFinding = await OpponentAcceptFindingOverlap(userRequestId, dateCheck.Value, startTimeCheck.Value, endTimeCheck.Value);
                    if (isOverlapWithAcceptedRequests.Any() || isOverlapWithOtherFinding.Any())
                    {
                        tempList.Remove(tempList[i]);
                    }
                }
            }

            return tempList;
        }

        public async Task<IEnumerable<OpponentFindingRequest>> RequestsOverlapOtherRequests(int userId, DateOnly date, int startTime, int endTime)
        {
            var requestOverlap = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAllAsync(x => x.UserRequestingId == userId
                            && x.Status != OpponentFindingRequestStatus.CANCELLED
                            && (x.OpponentFinding.Date == date
                                && ((x.OpponentFinding.StartTime <= startTime && x.OpponentFinding.EndTime >= startTime)
                                || (x.OpponentFinding.StartTime <= endTime && x.OpponentFinding.EndTime >= endTime)
                                || (x.OpponentFinding.StartTime >= startTime && x.OpponentFinding.EndTime <= endTime)
                                || (x.OpponentFinding.StartTime <= startTime && x.OpponentFinding.EndTime >= endTime))
                            || x.OpponentFinding.Booking.Date == date
                                && ((x.OpponentFinding.Booking.StartTime <= startTime && x.OpponentFinding.Booking.EndTime >= startTime)
                                || (x.OpponentFinding.Booking.StartTime <= endTime && x.OpponentFinding.Booking.EndTime >= endTime)
                                || (x.OpponentFinding.Booking.StartTime >= startTime && x.OpponentFinding.Booking.EndTime <= endTime)
                                || (x.OpponentFinding.Booking.StartTime <= startTime && x.OpponentFinding.Booking.EndTime >= endTime))),
                            x => x.UserRequesting,
                            x => x.OpponentFinding,
                            x => x.OpponentFinding.Booking);

            return requestOverlap;
        }

        private async Task<IEnumerable<OpponentFinding>> OpponentFindingOverlap(int userId, DateOnly date, int startTime, int endTime)
        {
            var opponentFindingOverlap = await _unitOfWork
                .OpponentFindingRepository
                .GetAllAsync(x => x.UserFindingId == userId
                            && (x.Status != OpponentFindingStatus.CANCELLED)
                            && (x.Date == date
                                && ((x.StartTime <= startTime && x.EndTime >= startTime)
                                || (x.StartTime <= endTime && x.EndTime >= endTime)
                                || (x.StartTime >= startTime && x.EndTime <= endTime)
                                || (x.StartTime <= startTime && x.EndTime >= endTime))
                            || x.Booking.Date == date
                                && ((x.Booking.StartTime <= startTime && x.Booking.EndTime >= startTime)
                                || (x.Booking.StartTime <= endTime && x.Booking.EndTime >= endTime)
                                || (x.Booking.StartTime >= startTime && x.Booking.EndTime <= endTime)
                                || (x.Booking.StartTime <= startTime && x.Booking.EndTime >= endTime))),
                            x => x.Booking,
                            x => x.UserFinding,
                            x => x.Field,
                            x => x.OpponentFindingRequests);

            return opponentFindingOverlap;
        }

        private async Task<IEnumerable<OpponentFinding>> OpponentAcceptFindingOverlap(int userId, DateOnly date, int startTime, int endTime)
        {
            var opponentFindingOverlap = await _unitOfWork
                .OpponentFindingRepository
                .GetAllAsync(x => x.UserFindingId == userId
                            && x.Status == OpponentFindingStatus.ACCEPTED
                            && (x.Date == date
                                && ((x.StartTime <= startTime && x.EndTime >= startTime)
                                || (x.StartTime <= endTime && x.EndTime >= endTime)
                                || (x.StartTime >= startTime && x.EndTime <= endTime)
                                || (x.StartTime <= startTime && x.EndTime >= endTime))
                            || x.Booking.Date == date
                                && ((x.Booking.StartTime <= startTime && x.Booking.EndTime >= startTime)
                                || (x.Booking.StartTime <= endTime && x.Booking.EndTime >= endTime)
                                || (x.Booking.StartTime >= startTime && x.Booking.EndTime <= endTime)
                                || (x.Booking.StartTime <= startTime && x.Booking.EndTime >= endTime))),
                            x => x.Booking,
                            x => x.UserFinding,
                            x => x.Field,
                            x => x.OpponentFindingRequests);

            return opponentFindingOverlap;
        }

        private async Task<IEnumerable<OpponentFindingRequest>> RequestAcceptedOverlap(int userRequestId, DateOnly date, int startTime, int endTime)
        {
            var requestOverlap = await _unitOfWork
                .OpponentFindingRequestRepository
                .GetAllAsync(x => x.UserRequestingId == userRequestId
                            && x.IsAccepted && x.Status == null
                            && (x.OpponentFinding.Date == date
                                && ((x.OpponentFinding.StartTime <= startTime && x.OpponentFinding.EndTime >= startTime)
                                || (x.OpponentFinding.StartTime <= endTime && x.OpponentFinding.EndTime >= endTime)
                                || (x.OpponentFinding.StartTime >= startTime && x.OpponentFinding.EndTime <= endTime)
                                || (x.OpponentFinding.StartTime <= startTime && x.OpponentFinding.EndTime >= endTime))
                            || x.OpponentFinding.Booking.Date == date
                                && ((x.OpponentFinding.Booking.StartTime <= startTime && x.OpponentFinding.Booking.EndTime >= startTime)
                                || (x.OpponentFinding.Booking.StartTime <= endTime && x.OpponentFinding.Booking.EndTime >= endTime)
                                || (x.OpponentFinding.Booking.StartTime >= startTime && x.OpponentFinding.Booking.EndTime <= endTime)
                                || (x.OpponentFinding.Booking.StartTime <= startTime && x.OpponentFinding.Booking.EndTime >= endTime))),
                            x => x.UserRequesting,
                            x => x.OpponentFinding,
                            x => x.OpponentFinding.Booking);

            return requestOverlap;
        }
    }
}