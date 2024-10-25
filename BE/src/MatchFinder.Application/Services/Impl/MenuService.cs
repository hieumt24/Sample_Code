using AutoMapper;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;

namespace MatchFinder.Application.Services.Impl
{
    public class MenuService : IMenuService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MenuService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MenuResponse> CreateMenu(MenuCreateRequest modelRequest)
        {
            var menu = new Menu
            {
                ItemName = modelRequest.Name,
                ItemDescription = modelRequest.Description,
                Price = modelRequest.Price,
                Quantity = modelRequest.Quantity,
                FieldId = modelRequest.FieldId
            };
            await _unitOfWork.MenuRepository.AddAsync(menu);
            if (await _unitOfWork.CommitAsync() > 0)
                return _mapper.Map<MenuResponse>(menu);
            throw new ConflictException("Failed to create menu");
        }

        public async Task<MenuResponse> UpdateMenu(int id, MenuUpdateRequest modelRequest)
        {
            var menu = await _unitOfWork.MenuRepository.GetAsync(x => x.Id == id, x => x.Field);
            if (menu == null)
                throw new NotFoundException("Menu not found");
            menu.ItemName = modelRequest.Name ?? menu.ItemName;
            menu.ItemDescription = modelRequest.Description ?? menu.ItemDescription;
            menu.Price = modelRequest.Price ?? menu.Price;
            menu.Quantity = modelRequest.Quantity ?? menu.Quantity;
            menu.FieldId = modelRequest.FieldId ?? menu.FieldId;
            _unitOfWork.MenuRepository.Update(menu);
            if (await _unitOfWork.CommitAsync() > 0)
                return _mapper.Map<MenuResponse>(menu);
            throw new ConflictException("Failed to update menu");
        }

        public async Task<RepositoryPaginationResponse<MenuResponse>> GetPagination(MenuFilterRequest filterRequest)
        {
            var menus = await _unitOfWork.MenuRepository.GetListAsync(x =>
                (string.IsNullOrEmpty(filterRequest.Name) || x.ItemName.Contains(filterRequest.Name)) &&
                (!filterRequest.Price.HasValue || x.Price == filterRequest.Price.Value) &&
                (!filterRequest.FieldId.HasValue || x.FieldId == filterRequest.FieldId.Value),
                filterRequest.Limit, filterRequest.Offset,
                x => x.Field);
            return new RepositoryPaginationResponse<MenuResponse>
            {
                Data = _mapper.Map<IEnumerable<MenuResponse>>(menus.Data),
                Total = menus.Total
            };
        }

        public async Task<MenuResponse> GetByIdAsync(int id)
        {
            var menu = await _unitOfWork.MenuRepository.GetAsync(x => x.Id == id, x => x.Field);
            if (menu == null)
                throw new NotFoundException("Menu not found");
            return _mapper.Map<MenuResponse>(menu);
        }
    }
}