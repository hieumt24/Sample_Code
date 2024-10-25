namespace MatchFinder.Application.Models.Requests
{
    public class CreateStaffRequest
    {
        public int FieldId { get; set; }
        public bool IsActive { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class UpdateStaffRequest
    {
        public int UserId { get; set; }
        public int FieldId { get; set; }
        public bool IsActive { get; set; }
    }

    public class GetStaffsRequest : Pagination
    {
        public int? FieldId { get; set; }
        public bool? IsActive { get; set; }
        public string? UserName { get; set; }
        public string? Name { get; set; }
    }
}