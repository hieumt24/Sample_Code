namespace MatchFinder.Application.Models.Responses
{
    public class MenuResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
    }
}