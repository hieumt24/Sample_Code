namespace MatchFinder.Application.Models.Responses
{
    public class FieldResponse
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
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string Description { get; set; }
        public string Avatar { get; set; }
        public string Cover { get; set; }
        public string Rating { get; set; }
        public int NumberOfBookings { get; set; }
        public Boolean IsFixedSlot { get; set; }
        public decimal Price { get; set; }
        public decimal Deposit { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
    }

    public class FreeSlotsResponse
    {
        public DateOnly Date { get; set; }
        public List<SlotResponse> Slots { get; set; }
    }
}