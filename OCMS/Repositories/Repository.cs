using Microsoft.EntityFrameworkCore;
using OCMS.Data;
using System.Linq.Expressions;

namespace OCMS.Repositories
{
    public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
    {
        private readonly AppDbContext _context = context;
        private readonly DbSet<T> _dbSet = context.Set<T>();

        // ===================== GET =====================

        public async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(object id)
            => await _dbSet.FindAsync(id);

        public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter)
            => await _dbSet.FirstOrDefaultAsync(filter);

        public async Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> filter)
            => await _dbSet.Where(filter).ToListAsync();

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> filter)
            => await _dbSet.AnyAsync(filter);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
            => filter == null
                ? await _dbSet.CountAsync()
                : await _dbSet.CountAsync(filter);
        public async Task<IEnumerable<T>> GetPagedWithIncludeAsync(int pageNumber,
                                                                   int pageSize,
                                                                   Expression<Func<T, bool>>? filter,
                                                                   Expression<Func<T, object>>? orderBy,
                                                                   bool isDescending,
                                                                   params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = isDescending ? query.OrderByDescending(orderBy) : query.OrderBy(orderBy);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        // ===================== GET WITH INCLUDE =====================

        public async Task<IEnumerable<T>> GetAllWithIncludeAsync(
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdWithIncludeAsync(object id,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
                query = query.Include(include);

            // Primary key se filter
            var keyName = _context.Model
                .FindEntityType(typeof(T))!
                .FindPrimaryKey()!
                .Properties
                .Select(x => x.Name)
                .First();

            return await query.FirstOrDefaultAsync(e =>
                EF.Property<object>(e, keyName).Equals(id));
        }
        // 
        public async Task<IEnumerable<T>> GetWhereWithIncludeAsync(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet.Where(filter);
            foreach (var include in includes)
                query = query.Include(include);

            return await query.ToListAsync();
        }

        public async Task<T?> GetFirstOrDefaultWithIncludeAsync(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            foreach (var include in includes)
                query = query.Include(include);

            return await query.FirstOrDefaultAsync(filter);
        }

        // ===================== GET WITH ORDERBY + PAGINATION =====================

        public async Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isDescending = false)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = isDescending
                    ? query.OrderByDescending(orderBy)
                    : query.OrderBy(orderBy);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // ===================== ADD =====================

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        // ===================== UPDATE =====================

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        // ===================== DELETE =====================

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task DeleteByEntityAsync(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        public async Task DeleteWhereAsync(Expression<Func<T, bool>> filter)
        {
            var entities = await _dbSet.Where(filter).ToListAsync();
            if (entities.Any())
            {
                _dbSet.RemoveRange(entities);
            }
        }

        //====================== SaveChanges ========================
        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public Task<int> CountAsync()
        {
            return _dbSet.CountAsync();
        }
    }
}
