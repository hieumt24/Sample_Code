namespace MatchFinder.Application.Models.Responses
{
    public class StaffResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string Email { get; set; }
        public string PhoneNumer { get; set; }
        public string Avatar { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }
}