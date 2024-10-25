using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class MenuCreateRequest
    {
        [Required]
        public int FieldId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal 0")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal 0")]
        public int Quantity { get; set; }
    }

    public class MenuUpdateRequest
    {
        public int? FieldId { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal 0")]
        public decimal? Price { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be greater than or equal 0")]
        public int? Quantity { get; set; }
    }

    public class MenuFilterRequest : Pagination
    {
        public string? Name { get; set; } = string.Empty;

        public decimal? Price { get; set; }

        public int? FieldId { get; set; }
    }
}