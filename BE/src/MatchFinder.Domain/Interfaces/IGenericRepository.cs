using MatchFinder.Domain.Models;
using System.Linq.Expressions;

namespace MatchFinder.Domain.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> expression = null);

        Task<IEnumerable<T>> GetAllAsync();

        Task<IEnumerable<T>> GetAllIncludingDeletedAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, object>> orderBy,
            bool isDescending = false,
            params Expression<Func<T, object>>[] includeProperties
            );

        Task<RepositoryPaginationResponse<T>> GetListAsync(
            Expression<Func<T, bool>> expression,
            int limit,
            int offset,
            params Expression<Func<T, object>>[] includeProperties
        );

        Task<RepositoryPaginationResponse<T>> GetLoadingListAsync(
            Expression<Func<T, bool>> expression,
            int limit,
            int offset,
            Expression<Func<T, object>> orderBy,
            bool isDescending = false,
            params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions
        );

        Task<RepositoryPaginationResponse<T>> GetLoadingListAsync(
            Expression<Func<T, bool>> expression,
            int limit,
            int offset,
            params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions
        );

        Task<RepositoryPaginationResponse<T>> GetLoadingListAsync(
            Expression<Func<T, bool>> expression,
            params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions
        );

        Task<RepositoryPaginationResponse<T>> GetListAsync(
            Expression<Func<T, bool>> expression,
            int limit,
            int offset,
            Expression<Func<T, object>> orderBy,
            bool isDescending = false,
            params Expression<Func<T, object>>[] includeProperties
        );

        Task<T> GetAsync(Expression<Func<T, bool>> expression);

        Task<T> GetAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetIncludingDeleteAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties);

        Task<T> GetLoadingAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);

        void SoftDelete(T entity);

        void RemoveRange(IEnumerable<T> entities);
    }
}