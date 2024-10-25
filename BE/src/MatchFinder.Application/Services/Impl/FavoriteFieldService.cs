using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Application.Services.Impl
{
    public class FavoriteFieldService : IFavoriteFieldService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public FavoriteFieldService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<RepositoryPaginationResponse<FieldResponse>> GetListFavorite(int uid, GetListFavoriteFieldRequest request)
        {
            var favorites = await _unitOfWork.FavoriteFieldRepository.GetLoadingListAsync(f => f.UserId == uid && !EF.Functions.Like(f.Field.Status, FieldStatus.REJECTED),
                                                                                          request.Limit, request.Offset,
                                                                                          f => f.Include(ffield => ffield.Field)
                                                                                                    .ThenInclude(pf => pf.PartialFields)
                                                                                                        .ThenInclude(b => b.Bookings
                                                                                                            .Where(b =>
                                                                                                                    EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false)))
                                                                                                .Include(o => o.Field.Owner)
                                                                                                .Include(r => r.Field.Rates)
                                                                                          );

            return new RepositoryPaginationResponse<FieldResponse>
            {
                Data = _mapper.Map<IEnumerable<FieldResponse>>(favorites.Data.Select(f => f.Field)),
                Total = favorites.Total
            };
        }

        public async Task<FieldResponse> AddFavoriteListAsync(int uid, int fid)
        {
            var user = await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid);
            if (user == null)
            {
                throw new NotFoundException("User not exist!");
            }

            var field = await _unitOfWork.FieldRepository.GetLoadingAsync(
                x => x.Id == fid &&
                !EF.Functions.Like(x.Status, FieldStatus.REJECTED),
                f => f.Include(pf => pf.PartialFields)
                        .ThenInclude(b => b.Bookings
                            .Where(b =>
                                    EF.Functions.Like(b.Status, BookingStatus.ACCEPTED) && (b.IsDeleted == null || b.IsDeleted == false))),
                f => f.Include(r => r.Rates),
                f => f.Include(o => o.Owner),
                f => f.Include(ff => ff.FavoriteFields));

            if (field == null)
            {
                throw new NotFoundException("Field not exist!");
            }

            if (field.FavoriteFields.Any(ff => ff.FieldId == fid && ff.UserId == uid))
            {
                throw new ConflictException("The field already exists on the favorites list!");
            }

            var favoriteField = new FavoriteField
            {
                Field = field,
                User = user,
            };

            await _unitOfWork.FavoriteFieldRepository.AddAsync(favoriteField);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<FieldResponse>(field);
            }

            throw new ConflictException("Add to favorite list failed!");
        }

        public async Task DeleteFavoriteAsync(int uid, int fid)
        {
            var favorite = await _unitOfWork.FavoriteFieldRepository.GetAsync(f => f.UserId == uid && f.FieldId == fid);
            if (favorite == null)
            {
                throw new NotFoundException("Field not exist in list.");
            }

            _unitOfWork.FavoriteFieldRepository.Delete(favorite);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return;
            }

            throw new ConflictException("Delete failed!");
        }
    }
}