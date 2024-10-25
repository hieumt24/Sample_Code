using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MatchFinder.Application.Models.Requests
{
    public class RateCreateRequest
    {
        public int BookingId { get; set; }

        [Range(0, 5, ErrorMessage = "Star must be from 0 to 5")]
        public int Star { get; set; }

        public string Comment { get; set; }
    }

    public class RateDeleteRequest
    {
        public int BookingId { get; set; }
    }

    public class RateUpdateRequest
    {
        public int BookingId { get; set; }

        [AllowNull]
        [Range(0, 5, ErrorMessage = "Star must be from 0 to 5")]
        public int? Star { get; set; }

        public string? Comment { get; set; }
    }

    public class RateSearchByFieldRequest : Pagination
    {
        public int? FieldId { get; set; }

        [AllowNull]
        [Range(0, 5, ErrorMessage = "Star must be from 0 to 5")]
        public int? Star { get; set; }
    }
}