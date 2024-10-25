using MatchFinder.Application.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MatchFinder.Application.Models.Requests
{
    public class CreateSlotRequest
    {
        public int FieldId { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int StartTime { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int EndTime { get; set; }
    }

    public class GetListSlotRequest
    {
    }

    public class UpdateSlotRequest
    {
        public int Id { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int? StartTime { get; set; }

        [AllowNull]
        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        [GreaterThanOrEqualTo("StartTime", ErrorMessage = "EndTime must greater than or equal to StartTime")]
        public int? EndTime { get; set; }
    }

    public class DeleteSlotRequest
    {
        public int Id { get; set; }
    }
}