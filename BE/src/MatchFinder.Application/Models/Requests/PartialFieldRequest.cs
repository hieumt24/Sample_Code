using MatchFinder.Application.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class PartialFieldCreateRequest
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        public IFormFile? Image_1 { get; set; }

        public IFormFile? Image_2 { get; set; }

        [Required]
        public int FieldId { get; set; }
    }

    public class PartialFieldUpdateRequest
    {
        public string? Name { get; set; }

        public string? Description { get; set; }
        public IFormFile? Image_1 { get; set; }
        public IFormFile? Image_2 { get; set; }

        public int? FieldId { get; set; }

        //[Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal 0")]
        //public decimal? Amount { get; set; }

        [RegularExpression(@"^(ACTIVE|INACTIVE)$", ErrorMessage = "Status must be ACTIVE or INACTIVE")]
        public string? Status { get; set; }

        //public float? Deposit { get; set; }
    }

    public class PartialFieldFilterRequest : Pagination
    {
        public string? FieldName { get; set; }
        public string? PartialFieldName { get; set; }
        public string? Status { get; set; }
        public DateOnly? FromDate { get; set; } = DateOnly.MinValue;

        [GreaterThanOrEqualTo("FromDate", ErrorMessage = "ToDate must greater than or equal to FromDate")]
        public DateOnly? ToDate { get; set; } = DateOnly.MaxValue;
    }

    public class PartialFieldAvailableRequest : Pagination
    {
        [Required]
        public DateOnly Date { get; set; }

        public int? StartTime { get; set; }
        public int Duration { get; set; }
    }
}