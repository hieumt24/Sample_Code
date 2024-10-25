namespace MatchFinder.Application.Models.Responses
{
    public class PartialFieldResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
    }

    public class PartialFieldWithNumberBookingsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public int NumberWaiting { get; set; }
        public int NumberAccepted { get; set; }
        public int NumberRejected { get; set; }
        public int NumberCanceled { get; set; }
    }
}