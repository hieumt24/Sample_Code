using Microsoft.AspNetCore.Http;

namespace MatchFinder.Application.Models.Requests
{
    public class ImageCreateRequest
    {
        public int FieldId { get; set; }
        public IFormFile[] Images { get; set; }
    }
}