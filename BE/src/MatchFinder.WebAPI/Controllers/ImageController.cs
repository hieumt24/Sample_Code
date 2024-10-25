using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Services;
using MatchFinder.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MatchFinder.WebAPI.Controllers
{
    [Route("api/images-field")]
    [ApiController]
    public class ImageController : BaseApiController
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("{imageId}")]
        public async Task<IActionResult> GetByImageIdAsync(int imageId)
        {
            var imageUrl = await _imageService.GetByIdAsync(imageId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get image successfully",
                Data = imageUrl,
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetByFieldIdAsync([FromQuery] int fieldId)
        {
            var imageUrls = await _imageService.GetListByFieldIdAsync(fieldId);
            return Ok(new GeneralGetResponse
            {
                Success = true,
                Message = "Get list image successfully",
                Data = imageUrls,
            });
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromForm] ImageCreateRequest request)
        {
            var imageUrls = await _imageService.UploadAsync(request);
            return Ok(new GeneralCreateResponse
            {
                Success = true,
                Message = "Upload images successfully",
                Data = imageUrls
            });
        }

        [Authorize]
        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteAsync(int imageId)
        {
            var result = await _imageService.DeleteAsync(imageId);
            return Ok(new GeneralBoolResponse
            {
                success = result,
                message = result ? "Delete image successfully" : "Delete image fail"
            });
        }
    }
}