using MatchFinder.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int Id { get; set; }
        public DateOnly Date { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int StartTime { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int EndTime { get; set; }

        public string Status { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Deposit amount must be non-negative")]
        public decimal DepositAmount { get; set; }

        public int PartialFieldId { get; set; }
        public PartialField PartialField { get; set; }
        public int? TeamId { get; set; }
        public Team? Team { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<PreOrder> PreOrders { get; set; }
        public ICollection<OpponentFinding> OpponentFindings { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
    }
}