using Common.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Common.Repository
{
    public class GenericRepositoryWithContext<TContext, TEntity> : IGenericRepository<TEntity> where TContext : DbContext where TEntity : class
    {
        protected readonly TContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepositoryWithContext(TContext context) : base()
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            bool asNoTracking = false,
            int? skip = null,
            int? take = null)
        {
            IQueryable<TEntity> query = _dbSet;

            query = await IsActiveQuery(query);

            if (filter is not null)
                query = query.Where(filter);

            if (include is not null)
                query = include(query);

            if (orderBy is not null)
                query = orderBy(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int pageNumber = 1,
            int pageSize = 10,
            bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            query = await IsActiveQuery(query);

            if (filter is not null)
                query = query.Where(filter);

            if (include is not null)
                query = include(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            if (orderBy is not null)
                query = orderBy(query); // apply sorting

            int totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<TEntity?> GetByIdAsync(
            Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            bool asNoTracking = false)
        {
            IQueryable<TEntity> query = _dbSet;

            query = await IsActiveQuery(query);

            if (include is not null)
                query = include(query);

            if (asNoTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task AddAsync(TEntity entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>>? filter = null)
        {
            IQueryable<TEntity> query = _dbSet;

            query = await IsActiveQuery(query);

            if (filter is not null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IQueryable<TEntity> query = _dbSet;

            query = await IsActiveQuery(query);

            return await query.AnyAsync(predicate);
        }

        public async Task<bool> SoftRemove(TEntity entity)
        {
            IQueryable<TEntity> query = _dbSet;

            query = await IsActiveQuery(query);

            if (entity is CommonEntity commonEntity)
            {
                commonEntity.IsActive = false;
                _dbSet.Update(entity);
                return await SaveChangesAsync();
            }

            return false;

        }

        public async Task<IQueryable<TEntity>> IsActiveQuery(IQueryable<TEntity> query)
        {
            // Check if TEntity inherits from CommonEntity
            Type type = typeof(TEntity);
            Type extType = typeof(CommonEntity);

            // If it does, apply the IsActive filter
            if (type.IsAssignableFrom(extType))
            {
                // Use reflection to get the IsActive property
                var prop = type.GetProperty("IsActive");

                // If the property exists and is of type bool, apply the filter
                if (prop != null && prop.PropertyType == typeof(bool))
                {
                    // Apply the IsActive filter
                    //query = query.Where(e => (bool)prop.GetValue(e)!);
                    //query = query.Where(e => (e as CommonEntity)!.IsActive == true);
                    query = query.Where(e => EF.Property<bool>(e, "IsActive") == true);
                    return query;

                }
            }
            return query;

        }

        public async Task<bool> AddAsyncWithSave(TEntity entity)
        {
           await _dbSet.AddAsync(entity);
           return await SaveChangesAsync();
        }

        public async Task<bool> AddRangeAsyncWithSave(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return await SaveChangesAsync();
        }

        public async Task<bool> UpdateAsyncWithSave(TEntity entity)
        {
            await Task.Run(() => _dbSet.Update(entity));
            return await SaveChangesAsync();
        }

        public async Task<bool> RemoveAsyncWithSave(TEntity entity)
        {
            await Task.Run(() => _dbSet.Remove(entity));
            return await SaveChangesAsync();
        }

        public async Task<bool> RemoveRangeAsyncWithSave(IEnumerable<TEntity> entities)
        {
            await Task.Run(() => _dbSet.RemoveRange(entities));
            return await SaveChangesAsync();
        }
    }
}
