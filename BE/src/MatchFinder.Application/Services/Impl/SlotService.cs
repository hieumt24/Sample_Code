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
    public class SlotService : ISlotService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public SlotService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<SlotResponse> CreateSlotAsync(CreateSlotRequest request)
        {
            var field = await _unitOfWork.FieldRepository.GetAsync(
                f => f.Id == request.FieldId &&
                        !EF.Functions.Like(f.Status, FieldStatus.REJECTED),
                f => f.Slots.Where(s => s.IsDeleted == false)
            );

            if (field == null)
                throw new ConflictException("Field not exist!");

            if (request.StartTime <= field.OpenTime || request.EndTime >= field.CloseTime)
                throw new ConflictException("The slot time confict with open and close time.");

            foreach (var slot in field.Slots)
            {
                if (IsOverlapping(slot, request.StartTime, request.EndTime))
                {
                    throw new ConflictException("The slot time overlaps with an existing slot.");
                }
            }

            var newSlot = new Slot
            {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Field = field,
            };

            await _unitOfWork.SlotRepository.AddAsync(newSlot);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<SlotResponse>(newSlot);
            }

            throw new ConflictException("The request failed!");
        }

        public async Task<SlotResponse> UpdateSlotAsync(UpdateSlotRequest request)
        {
            var slot = await _unitOfWork.SlotRepository.GetAsync(
                s => s.Id == request.Id &&
                        s.Field.Status == "ACCEPTED",
                f => f.Field.Slots
            );

            if (slot == null)
                throw new ConflictException("Slot not exist!");

            int start = request.StartTime ?? slot.StartTime;
            int end = request.EndTime ?? slot.EndTime;

            if (start <= slot.Field.OpenTime || end >= slot.Field.CloseTime)
                throw new ConflictException("The slot time confict with open and close time.");

            foreach (var item in slot.Field.Slots)
            {
                if (IsOverlapping(item, start, end) && item.Id != slot.Id)
                {
                    throw new ConflictException("The slot time overlaps with an existing slot.");
                }
            }

            slot.StartTime = start;
            slot.EndTime = end;

            _unitOfWork.SlotRepository.Update(slot);
            if (await _unitOfWork.CommitAsync() > 0)
            {
                return _mapper.Map<SlotResponse>(slot);
            }

            throw new ConflictException("The request failed!");
        }

        public async Task SoftDeleteSlotAsync(int id)
        {
            var slot = await _unitOfWork.SlotRepository.GetAsync(s => s.Id == id,
                                                                    f => f.Field);

            if (slot == null)
                throw new ConflictException("Slot not exist!");

            slot.IsDeleted = true;
            _unitOfWork.SlotRepository.Update(slot);
            await _unitOfWork.CommitAsync();
        }

        public async Task<SlotResponse> GetByIdAsync(int id)
        {
            var slot = await _unitOfWork.SlotRepository.GetAsync(s => s.Id == id,
                                                                    f => f.Field);

            if (slot == null)
                throw new ConflictException("Slot not exist!");

            return _mapper.Map<SlotResponse>(slot);
        }

        public async Task<RepositoryPaginationResponse<SlotResponse>> GetListAsync(int fieldId, GetListSlotRequest pagination)
        {
            var slots = await _unitOfWork.SlotRepository.GetAllAsync(s => s.FieldId == fieldId,
                                                                            //pagination.Limit, pagination.Offset,
                                                                            s => s.StartTime, false,
                                                                            f => f.Field);

            return new RepositoryPaginationResponse<SlotResponse>
            {
                Data = _mapper.Map<IEnumerable<SlotResponse>>(slots),
                Total = slots.Count()
            };
        }

        private bool IsOverlapping(Slot existingSlot, int start, int end)
        {
            return start < existingSlot.EndTime && end > existingSlot.StartTime;
        }
    }
}