using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class FavoriteField : BaseEntity
    {
        public int UserId { get; set; }
        public int FieldId { get; set; }

        public virtual User User { get; set; }
        public virtual Field Field { get; set; }
    }
}