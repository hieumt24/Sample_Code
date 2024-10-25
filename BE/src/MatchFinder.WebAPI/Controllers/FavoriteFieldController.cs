using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Authorize]
    [Route("api/favorite-fields")]
    [ApiController]
    public class FavoriteFieldController : BaseApiController
    {
        private readonly IFavoriteFieldService _favoriteService;

        public FavoriteFieldController(IFavoriteFieldService favoriteService)
        {
            _favoriteService = favoriteService;
        }

        [HttpGet]
        public async Task<IActionResult> GetListFavorite([FromQuery] GetListFavoriteFieldRequest request)
        {
            var fields = await _favoriteService.GetListFavorite(UserID, request);
            return Ok(new PaginationResponse
            {
                Success = true,
                Message = "Get Favorites successfully",
                Data = fields.Data,
                Meta = new Meta
                {
                    Total = fields.Total
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddFavoriteListAsync([FromBody] AddFavoriteRequest request)
        {
            var field = await _favoriteService.AddFavoriteListAsync(UserID, request.FieldId);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Add to list successfully",
                Data = field
            });
        }

        [Authorize]
        [HttpDelete("field/{fid}")]
        public async Task<IActionResult> DeleteFavoriteAsync(int fid)
        {
            await _favoriteService.DeleteFavoriteAsync(UserID, fid);

            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Removed!",
            });
        }
    }
}