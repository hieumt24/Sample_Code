namespace MatchFinder.Application.Models.Requests
{
    public class AddFavoriteRequest
    {
        public int FieldId { get; set; }
    }

    public class GetListFavoriteFieldRequest : Pagination
    {
    }
}