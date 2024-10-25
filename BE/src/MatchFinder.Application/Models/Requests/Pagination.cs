using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Application.Models.Requests
{
    public class Pagination
    {
        [Range(1, 50, ErrorMessage = "Limit must be greater than zero.")]
        public int Limit { get; set; } = 10;

        [Range(0, int.MaxValue, ErrorMessage = "Offset must be greater than or equal to zero.")]
        public int Offset { get; set; } = 0;
    }
}