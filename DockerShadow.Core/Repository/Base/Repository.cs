using DockerShadow.Core.Contract.Repository;
using DockerShadow.Domain.Entities.Base;
using DockerShadow.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace DockerShadow.Core.Repository.Base
{
    public class Repository<T, TId> : RepositoryBase, IRepository<T, TId>
        where T : class, IEntityBase<TId>
    {
        public Repository(IServiceScopeFactory serviceScopeFactory, Func<ApplicationDbContext, DbSet<T>> getDbSet)
        : base(serviceScopeFactory)
        {
            _getDbSet = getDbSet;
        }

        protected Func<ApplicationDbContext, DbSet<T>> _getDbSet { get; private set; }

        public async virtual Task<T> GetByIdAsync(TId id, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                return await _getDbSet(dbContext).FindAsync(new object[] { id }, cancellationToken: cancellationToken);
            }
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                _getDbSet(dbContext).AddRange(entities);
                await dbContext.SaveChangesAsync(cancellationToken);

                return entities;
            }
        }

        public async Task<IEnumerable<T>> UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                _getDbSet(dbContext).UpdateRange(entities);
                await dbContext.SaveChangesAsync(cancellationToken);

                return entities;
            }
        }

        public async Task<T> SaveAsync(T entity, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                var _entities = _getDbSet(dbContext);
                _entities.Add(entity);

                await dbContext.SaveChangesAsync(cancellationToken);

                return entity;
            }
        }

        public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                var _entities = _getDbSet(dbContext);
                dbContext.Entry(entity).State = EntityState.Modified;

                await dbContext.SaveChangesAsync(cancellationToken);

                return entity;
            }
        }

        public async Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                _getDbSet(dbContext).Remove(entity);

                await dbContext.SaveChangesAsync(cancellationToken);
            }
        }

        public async virtual Task<IReadOnlyList<T>> ListAllAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                return await _getDbSet(dbContext).ToListAsync(cancellationToken);
            }
        }

        public async virtual Task RefreshDb(CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                await dbContext.Database.EnsureDeletedAsync(cancellationToken);
                await dbContext.Database.EnsureCreatedAsync(cancellationToken);
            }
        }

        public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeString = null,
            List<Expression<Func<T, object>>> includes = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = GetDatabaseContext(scope);
                var _entities = _getDbSet(dbContext);

                var query = disableTracking ? _entities.AsNoTracking() : _entities;

                if (!string.IsNullOrWhiteSpace(includeString))
                {
                    query = query.Include(includeString);
                }
                else if (includes != null)
                {
                    query = includes.Aggregate(query, (current, include) => current.Include(include));
                }

                if (predicate != null)
                {
                    query = query.Where(predicate);
                }

                if (orderBy != null)
                {
                    return await orderBy(query).ToListAsync(cancellationToken);
                }

                return await query.ToListAsync(cancellationToken);
            }
        }
    }
}
