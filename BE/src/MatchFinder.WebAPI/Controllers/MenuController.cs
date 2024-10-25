using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/menus")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMenu([FromBody] MenuCreateRequest modelRequest)
        {
            var result = await _menuService.CreateMenu(modelRequest);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Create menu successfully",
                Data = result
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMenu(int id, [FromBody] MenuUpdateRequest modelRequest)
        {
            var result = await _menuService.UpdateMenu(id, modelRequest);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Update menu successfully",
                Data = result
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetPagination([FromQuery] MenuFilterRequest filterRequest)
        {
            var menus = await _menuService.GetPagination(filterRequest);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get menus successfully",
                Data = menus.Data,
                Meta = new Meta
                {
                    Limit = filterRequest.Limit,
                    Offset = filterRequest.Offset,
                    Total = menus.Total
                }
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var menu = await _menuService.GetByIdAsync(id);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get menu successfully",
                Data = menu
            });
        }
    }
}