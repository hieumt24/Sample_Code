using MatchFinder.Application.Attributes;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MatchFinder.Application.Models.Requests
{
    public class FieldCreateRequest
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        public int OpenTime { get; set; }

        [Range(0, 86400, ErrorMessage = "CloseTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("OpenTime", ErrorMessage = "CloseTime must greater than or equal to OpenTime")]
        public int CloseTime { get; set; }

        public string Description { get; set; }
        public IFormFile Avatar { get; set; }
        public IFormFile Cover { get; set; }
        public Boolean IsFixedSlot { get; set; } = true;

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal 0")]
        public decimal Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal 0")]
        public decimal Deposit { get; set; }
    }

    public class FixedSlotRequest
    {
        public int Id { get; set; }
        public bool IsFixedSlot { get; set; }
    }

    public class HandleStatusFieldRequest
    {
        public int Id { get; set; }
        public string Status { get; set; }
    }

    public class RejectedFieldRequest
    {
        public int Id { get; set; }
    }

    public class FieldUpdateRequest
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }

        [AllowNull]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNumber { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h")]
        public int? OpenTime { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "CloseTime must be between 0h and 24h")]
        [GreaterThanOrEqualTo("OpenTime", ErrorMessage = "CloseTime must greater than or equal to OpenTime")]
        public int? CloseTime { get; set; }

        public string? Description { get; set; }
        public IFormFile? Avatar { get; set; }
        public IFormFile? Cover { get; set; }
        public Boolean? IsFixedSlot { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "SlotTime must be between 0h and 24h00")]
        public int? SlotTime { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal 0")]
        public decimal? Price { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Amount must be greater than or equal 0")]
        public decimal? Deposit { get; set; }
    }

    public class GetFieldsRequest : Pagination
    {
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Status { get; set; }

        [AllowNull]
        public decimal? FromPrice { get; set; }

        [AllowNull]
        [GreaterThanOrEqualTo("FromPrice", ErrorMessage = "ToPrice must greater than or equal to FromPrice")]
        public decimal? ToPrice { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "FromStar must be between 1 and 5")]
        public double? FromStar { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "ToStar must be between 1 and 5")]
        [GreaterThanOrEqualTo("FromStar", ErrorMessage = "ToStar must greater than or equal to FromStar")]
        public double? ToStar { get; set; }
    }

    public class SearchOptionRequest : Pagination
    {
        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public int Duration { get; set; }

        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }

        [AllowNull]
        public double? Latitude { get; set; }

        [AllowNull]
        public double? Longitude { get; set; }

        [AllowNull]
        [Range(0, double.MaxValue, ErrorMessage = "Radius must be greater than or equal 0")]
        public double? Radius { get; set; }

        [AllowNull]
        public decimal? FromPrice { get; set; }

        [AllowNull]
        [GreaterThanOrEqualTo("FromPrice", ErrorMessage = "ToPrice must greater than or equal to FromPrice")]
        public decimal? ToPrice { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "FromStar must be between 1 and 5")]
        public double? FromStar { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "ToStar must be between 1 and 5")]
        [GreaterThanOrEqualTo("FromStar", ErrorMessage = "ToStar must greater than or equal to FromStar")]
        public double? ToStar { get; set; }
    }

    public class GetFreeSlotRequest
    {
        public int Id { get; set; }
        public DateOnly FromDate { get; set; }

        [GreaterThanOrEqualTo("FromDate", ErrorMessage = "ToDate must greater than or equal to FromDate")]
        public DateOnly ToDate { get; set; }
    }

    public class GetFieldsEarlyRequest : Pagination
    {
        [AllowNull]
        public decimal? FromPrice { get; set; } = null;

        [AllowNull]
        [GreaterThanOrEqualTo("FromPrice", ErrorMessage = "ToPrice must greater than or equal to FromPrice")]
        public decimal? ToPrice { get; set; } = null;

        [AllowNull]
        [Range(1, 5, ErrorMessage = "FromStar must be between 1 and 5")]
        public double? FromStar { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "ToStar must be between 1 and 5")]
        [GreaterThanOrEqualTo("FromStar", ErrorMessage = "ToStar must greater than or equal to FromStar")]
        public double? ToStar { get; set; }
    }

    public class FieldsLocationRequest : Pagination
    {
        public double FromLatitude { get; set; }

        [GreaterThanOrEqualTo("FromLatitude", ErrorMessage = "ToLatitude must greater than or equal to FromLatitude")]
        public double ToLatitude { get; set; }

        public double FromLongitude { get; set; }

        [GreaterThanOrEqualTo("FromLongitude", ErrorMessage = "ToLongitude must greater than or equal to FromLongitude")]
        public double ToLongitude { get; set; }

        [AllowNull]
        public decimal? FromPrice { get; set; } = null;

        [AllowNull]
        [GreaterThanOrEqualTo("FromPrice", ErrorMessage = "ToPrice must greater than or equal to FromPrice")]
        public decimal? ToPrice { get; set; } = null;

        [AllowNull]
        [Range(1, 5, ErrorMessage = "FromStar must be between 1 and 5")]
        public double? FromStar { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "ToStar must be between 1 and 5")]
        [GreaterThanOrEqualTo("FromStar", ErrorMessage = "ToStar must greater than or equal to FromStar")]
        public double? ToStar { get; set; }
    }

    public class FieldsScanLocationRequest : Pagination
    {
        [Required(ErrorMessage = "Latitude is required")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Longitude is required")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Radius is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Radius must be greater than or equal 0")]
        public double Radius { get; set; }

        [AllowNull]
        public decimal? FromPrice { get; set; } = null;

        [AllowNull]
        [GreaterThanOrEqualTo("FromPrice", ErrorMessage = "ToPrice must greater than or equal to FromPrice")]
        public decimal? ToPrice { get; set; } = null;

        [AllowNull]
        [Range(1, 5, ErrorMessage = "FromStar must be between 1 and 5")]
        public double? FromStar { get; set; }

        [AllowNull]
        [Range(1, 5, ErrorMessage = "ToStar must be between 1 and 5")]
        [GreaterThanOrEqualTo("FromStar", ErrorMessage = "ToStar must greater than or equal to FromStar")]
        public double? ToStar { get; set; }
    }

    public class FieldRatingFilterRequest
    {
        public double? Lat { get; set; }
        public double? Long { get; set; }
    }
}