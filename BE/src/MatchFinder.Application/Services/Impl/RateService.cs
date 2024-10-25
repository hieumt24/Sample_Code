using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using System.Linq.Expressions;

namespace MatchFinder.Application.Services.Impl
{
    public class RateService : IRateService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public RateService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<RepositoryPaginationResponse<RateResponse>> GetListByFieldAsync(RateSearchByFieldRequest request)
        {
            Expression<Func<Rate, bool>> filter = r => true;

            if (request.FieldId.HasValue)
                filter = filter.And(r => r.FieldId == request.FieldId);

            if (request.Star.HasValue)
                filter = filter.And(r => r.Star == request.Star.Value);

            var ratesResult = await _unitOfWork.RateRepository.GetListAsync(
                filter,
                request.Limit,
                request.Offset,
                r => r.CreatedAt,
                true,
                r => r.Booking,
                r => r.Rater,
                r => r.Field
            );

            return new RepositoryPaginationResponse<RateResponse>
            {
                Data = _mapper.Map<IEnumerable<RateResponse>>(ratesResult.Data),
                Total = ratesResult.Total
            };
        }

        public async Task<RateResponse> UpdateRateAsync(int uid, RateUpdateRequest request)
        {
            var rate = await _unitOfWork.RateRepository.GetAsync(u => u.BookingId == request.BookingId &&
                                                                        u.UserId == uid,
                                                                        b => b.Field,
                                                                        b => b.Rater,
                                                                        b => b.Booking);
            if (rate == null)
            {
                throw new NotFoundException("Rating not found.");
            }

            rate.Star = request.Star ?? rate.Star;
            rate.Comment = request.Comment ?? rate.Comment;

            _unitOfWork.RateRepository.Update(rate);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<RateResponse>(rate);
            }

            throw new ConflictException("Update failed!");
        }

        public async Task<RateResponse> GetRateAsync(int uid, int bookingId)
        {
            var rate = await _unitOfWork.RateRepository.GetAsync(u => u.BookingId == bookingId &&
                                                                        u.UserId == uid,
                                                                        b => b.Field,
                                                                        b => b.Rater,
                                                                        b => b.Booking);
            if (rate == null)
            {
                throw new NotFoundException("Rating not found.");
            }

            return _mapper.Map<RateResponse>(rate);
        }

        public async Task DeleteRateAsync(int uid, int bid)
        {
            var rate = await _unitOfWork.RateRepository.GetAsync(u => u.BookingId == bid &&
                                                                        u.UserId == uid);
            if (rate == null)
            {
                throw new NotFoundException("Rating not found.");
            }

            _unitOfWork.RateRepository.Delete(rate);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return;
            }

            throw new ConflictException("Delete failed!");
        }

        public async Task<RateResponse> CreateRateAsync(int uid, RateCreateRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid);
            if (user == null)
            {
                throw new NotFoundException("User not exist!");
            }

            var booking = await _unitOfWork.BookingRepository.GetAsync(u => u.Id == request.BookingId &&
                                                                        !(u.PartialField.Field.IsDeleted ?? false) &&
                                                                        u.PartialField.Field.Status == "ACCEPTED" &&
                                                                        u.Status == BookingStatus.ACCEPTED,
                                                                        b => b.PartialField.Field,
                                                                        b => b.Rates);
            if (booking == null)
            {
                throw new NotFoundException("The user has not booked this course or the booking has not been completed.");
            }

            if (booking.Rates.Any())
            {
                throw new ConflictException("There is an error, maybe it is because you have rated this course!");
            }

            var rate = new Rate
            {
                Star = request.Star,
                Comment = request.Comment,
                Booking = booking,
                Field = booking.PartialField.Field,
                Rater = user,
            };

            await _unitOfWork.RateRepository.AddAsync(rate);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<RateResponse>(rate);
            }

            throw new ConflictException("Rating failed!");
        }
    }
}