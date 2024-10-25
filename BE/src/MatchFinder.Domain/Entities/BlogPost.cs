using MatchFinder.Domain.Models;

namespace MatchFinder.Domain.Entities
{
    public class BlogPost : BaseEntity
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Category { get; set; }
        public bool IsPinned { get; set; }
        public string ThumbnailUrl { get; set; }
        public bool IsAdmin { get; set; }
        public int? FieldId { get; set; }
        public Field? Field { get; set; }
        public User Author { get; set; }
    }
}