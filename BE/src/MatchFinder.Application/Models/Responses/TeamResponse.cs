namespace MatchFinder.Application.Models.Responses
{
    public class TeamResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int CaptainId { get; set; }
        public string CaptainName { get; set; }
    }
}