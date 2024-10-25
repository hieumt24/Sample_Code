using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;

namespace MatchFinder.Infrastructure.Repositories
{
    public class ImageRepository : GenericRepository<Image>, IImageRepository
    {
        public ImageRepository(MatchFinderContext context) : base(context)
        {
        }
    }
}