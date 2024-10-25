using MatchFinder.Domain.Entities;

namespace MatchFinder.Domain.Interfaces
{
    public interface IFieldRepository : IGenericRepository<Field>
    {
        Task<int> SoftDeleteFieldAndPartialFieldsAsync(int fieldId);

        Task<IEnumerable<Field>> GetFieldByDistance(double? latitude, double? longitude, int? radius);
    }
}