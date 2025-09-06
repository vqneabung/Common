using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Common.Repository
{
    public class GenericRepository<TContext, TEntity> : IGenericRepository<TEntity> where TContext : DbContext where TEntity : class
    {
        protected readonly TContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(TContext context) : base()
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

            if (filter is not null)
                query = query.Where(filter);

            return await query.CountAsync();
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }
    }
}
