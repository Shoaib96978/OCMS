using System.Linq.Expressions;

namespace OCMS.Repositories
{
    public interface IRepository<T> where T : class
    {
        // ===================== GET =====================
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(object id);
        Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);
        Task<IEnumerable<T>> GetWhereAsync(Expression<Func<T, bool>> filter);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> filter);
        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);
        Task<int> CountAsync();
        Task<IEnumerable<T>> GetPagedWithIncludeAsync(
                             int pageNumber,
                             int pageSize,
                             Expression<Func<T, bool>>? filter,
                             Expression<Func<T, object>>? orderBy,
                             bool isDescending,
                             params Expression<Func<T, object>>[] includes);
        // ===================== GET WITH INCLUDE =====================
        Task<IEnumerable<T>> GetAllWithIncludeAsync(
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetByIdWithIncludeAsync(object id,
            params Expression<Func<T, object>>[] includes);

        Task<IEnumerable<T>> GetWhereWithIncludeAsync(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes);

        Task<T?> GetFirstOrDefaultWithIncludeAsync(
            Expression<Func<T, bool>> filter,
            params Expression<Func<T, object>>[] includes);

        // ===================== GET WITH ORDERBY + PAGINATION =====================
        Task<IEnumerable<T>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>? orderBy = null,
            bool isDescending = false);

        // ===================== ADD =====================
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        // ===================== UPDATE =====================
        Task UpdateAsync(T entity);
        Task UpdateRangeAsync(IEnumerable<T> entities);

        // ===================== DELETE =====================
        Task DeleteAsync(object id);
        Task DeleteByEntityAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task DeleteWhereAsync(Expression<Func<T, bool>> filter);

        // ===================== SaveChanges =====================

        Task<int> SaveChangesAsync();
    }
}
