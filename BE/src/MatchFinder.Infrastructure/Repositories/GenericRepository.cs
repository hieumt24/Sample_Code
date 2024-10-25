using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace MatchFinder.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly MatchFinderContext _context;

        public GenericRepository(MatchFinderContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>> expression = null)
        {
            IQueryable<T> query = _context.Set<T>();

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null
                ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters)
                : isNotDeletedExpression;

            return query.Where(combinedExpression);
        }

        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        public void SoftDelete(T entity)
        {
            PropertyInfo propertyInfo = entity.GetType().GetProperty("IsDeleted");
            propertyInfo.SetValue(entity, true);
            _context.Set<T>().Update(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>()
                         .Where(entity => EF.Property<bool>(entity, "IsDeleted") == false)
                         .OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"))
                         .ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression,
            params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeExpressions != null)
            {
                foreach (var include in includeExpressions)
                {
                    query = include(query);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null
                ? Expression.Lambda<Func<T, bool>>(
                    Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)),
                    expression.Parameters)
                : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllIncludingDeletedAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (expression != null)
            {
                query = query.Where(expression);
            }

            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> expression,
            Expression<Func<T, object>> orderBy,
            bool isDescending = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);

            if (orderBy != null)
            {
                query = isDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }

            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));
            return await query.ToListAsync();
        }

        public async Task<T> GetLoadingAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeExpressions != null)
            {
                foreach (var include in includeExpressions)
                {
                    query = include(query);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }
            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> GetIncludingDeleteAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (expression != null)
            {
                query = query.Where(expression);
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _context.Set<T>();
            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            return await query.FirstOrDefaultAsync();
        }

        public void Update(T entity)
        {
            PropertyInfo propertyInfo = entity.GetType().GetProperty("LastUpdatedAt");
            propertyInfo.SetValue(entity, DateTime.Now);
            _context.Set<T>().Update(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
        }

        public async Task<RepositoryPaginationResponse<T>> GetListAsync(Expression<Func<T, bool>> expression, int limit, int offset, params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            var total = await query.CountAsync();
            query = query.Skip(offset).Take(limit);
            var data = await query.ToListAsync();
            return new RepositoryPaginationResponse<T>
            {
                Data = data,
                Total = total
            };
        }

        public async Task<RepositoryPaginationResponse<T>> GetLoadingListAsync(
            Expression<Func<T, bool>> expression,
            int limit,
            int offset,
            Expression<Func<T, object>> orderBy,
            bool isDescending = false,
            params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions
        )
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeExpressions != null)
            {
                foreach (var include in includeExpressions)
                {
                    query = include(query);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            if (orderBy != null)
            {
                query = isDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }

            var total = await query.CountAsync();
            query = query.Skip(offset).Take(limit);
            var data = await query.ToListAsync();
            return new RepositoryPaginationResponse<T>
            {
                Data = data,
                Total = total
            };
        }

        public async Task<RepositoryPaginationResponse<T>> GetLoadingListAsync(Expression<Func<T, bool>> expression, int limit, int offset, params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeExpressions != null)
            {
                foreach (var include in includeExpressions)
                {
                    query = include(query);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            var total = await query.CountAsync();
            query = query.Skip(offset).Take(limit);
            var data = await query.ToListAsync();
            return new RepositoryPaginationResponse<T>
            {
                Data = data,
                Total = total
            };
        }

        public async Task<RepositoryPaginationResponse<T>> GetLoadingListAsync(Expression<Func<T, bool>> expression, params Func<IQueryable<T>, IQueryable<T>>[] includeExpressions)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeExpressions != null)
            {
                foreach (var include in includeExpressions)
                {
                    query = include(query);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters) : isNotDeletedExpression;

            query = query.Where(combinedExpression);
            query = query.OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedAt"));

            var total = await query.CountAsync();
            var data = await query.ToListAsync();
            return new RepositoryPaginationResponse<T>
            {
                Data = data,
                Total = total
            };
        }

        public async Task<RepositoryPaginationResponse<T>> GetListAsync(
            Expression<Func<T, bool>> expression,
            int limit,
            int offset,
            Expression<Func<T, object>> orderBy,
            bool isDescending = false,
            params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            Expression<Func<T, bool>> isNotDeletedExpression = entity => EF.Property<bool>(entity, "IsDeleted") == false;
            var combinedExpression = expression != null
                ? Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expression.Body, Expression.Invoke(isNotDeletedExpression, expression.Parameters)), expression.Parameters)
                : isNotDeletedExpression;

            query = query.Where(combinedExpression);

            if (orderBy != null)
            {
                query = isDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);
            }

            var total = await query.CountAsync();

            query = query.Skip(offset).Take(limit);

            var data = await query.ToListAsync();

            return new RepositoryPaginationResponse<T>
            {
                Data = data,
                Total = total
            };
        }
    }
}