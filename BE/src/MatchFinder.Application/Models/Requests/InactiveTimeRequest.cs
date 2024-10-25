using MatchFinder.Application.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class InactiveTimeCreateRequest : IValidatableObject
    {
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public string Reason { get; set; }

        [Required]
        public int FieldId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime < DateTime.Now)
            {
                yield return new ValidationResult("Start time must be from now", new[] { "StartTime" });
            }
            if (EndTime <= StartTime)
            {
                yield return new ValidationResult("End time must be greater than Start time.", new[] { "Endtime" });
            }
        }
    }

    public class InactiveTimeUpdateRequest : IValidatableObject
    {
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? Reason { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartTime.HasValue && EndTime.HasValue && EndTime <= StartTime)
            {
                yield return new ValidationResult("End time must be greater than Start time.", new[] { "Endtime" });
            }
        }
    }

    public class InactiveTimeGetRequest : Pagination
    {
        public int FieldId { get; set; }
        public DateOnly? StartDate { get; set; }

        [GreaterThanOrEqualTo("StartDate", ErrorMessage = "EndDate must greater than or equal to StartDate")]
        public DateOnly? EndDate { get; set; }

        public bool? IsPaging { get; set; }
    }
}