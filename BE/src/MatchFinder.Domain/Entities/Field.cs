using MatchFinder.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace MatchFinder.Domain.Entities
{
    public class Field : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Commune { get; set; }
        public string PhoneNumber { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Status { get; set; }

        [Range(0, 86400, ErrorMessage = "OpenTime must be between 0h and 24h00")]
        public int OpenTime { get; set; }

        [Range(0, 86400, ErrorMessage = "CloseTime must be between 0h and 24h00")]
        public int CloseTime { get; set; }

        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public Boolean IsFixedSlot { get; set; }
        public decimal Price { get; set; }
        public decimal Deposit { get; set; }
        public int OwnerId { get; set; }
        public User Owner { get; set; }
        public ICollection<PartialField> PartialFields { get; set; }
        public ICollection<Staff> Staffs { get; set; }
        public ICollection<OpponentFinding> OpponentFindings { get; set; }
        public ICollection<Menu> Menus { get; set; }
        public ICollection<InactiveTime> InactiveTimes { get; set; }
        public ICollection<Slot> Slots { get; set; }
        public ICollection<Rate> Rates { get; set; }
        public ICollection<FavoriteField> FavoriteFields { get; set; }
        public ICollection<Report> Reports { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
        public ICollection<Image> Images { get; set; }
    }
}