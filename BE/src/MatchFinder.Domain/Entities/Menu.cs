using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class Menu : BaseEntity
    {
        public int Id { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }
        public ICollection<PreOrder> PreOrders { get; set; }
    }
}