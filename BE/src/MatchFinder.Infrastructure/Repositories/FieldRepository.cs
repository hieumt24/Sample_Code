using MatchFinder.Domain.Entities;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace MatchFinder.Infrastructure.Repositories
{
    public class FieldRepository : GenericRepository<Field>, IFieldRepository
    {
        public FieldRepository(MatchFinderContext context) : base(context)
        {
        }

        public async Task<int> SoftDeleteFieldAndPartialFieldsAsync(int fieldId)
        {
            var context = _context as MatchFinderContext;

            var fieldResult = await context.Fields
                .Where(f => f.Id == fieldId)
                .ExecuteUpdateAsync(f => f
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.LastUpdatedAt, DateTime.UtcNow)
                );

            var partialFieldResult = await context.PartialFields
                .Where(pf => pf.FieldId == fieldId)
                .ExecuteUpdateAsync(pf => pf
                    .SetProperty(x => x.IsDeleted, true)
                    .SetProperty(x => x.LastUpdatedAt, DateTime.UtcNow)
                );

            return fieldResult + partialFieldResult;
        }

        public async Task<IEnumerable<Field>> GetFieldByDistance(double? latitude, double? longitude, int? radius)
        {
            var context = _context as MatchFinderContext;

            var fields = context.Fields
                .Where(f => f.IsDeleted == false && f.Status == "ACCEPTED")
                .Include(f => f.PartialFields)
                .ThenInclude(pf => pf.Bookings)
                .Include(r => r.Rates)
                .Include(o => o.Owner)
                .AsEnumerable()
                .Where(f => (!latitude.HasValue || !longitude.HasValue) ||
                            (CalculateHaversineDistance(f.Latitude, f.Longitude, latitude.Value, longitude.Value) <= radius));

            return fields;
        }

        private double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double EarthRadiusKm = 6371.0;

            double lat1Rad = ConvertToRadians(lat1);
            double lon1Rad = ConvertToRadians(lon1);
            double lat2Rad = ConvertToRadians(lat2);
            double lon2Rad = ConvertToRadians(lon2);

            double dLat = lat2Rad - lat1Rad;
            double dLon = lon2Rad - lon1Rad;

            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Pow(Math.Sin(dLon / 2), 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private double ConvertToRadians(double degree)
        {
            return degree * Math.PI / 180.0;
        }
    }
}