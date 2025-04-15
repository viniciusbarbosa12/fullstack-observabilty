using EmployeeManagement.Domain.Entities;
using EmployeeManagement.Domain.Models;
using System.Linq.Expressions;

namespace EmployeeManagement.Domain.Interfaces.Base
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<T> CreateAsync(T entity);
        Task<T> CreateAndSaveAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> GetOneAsync(Expression<Func<T, bool>> expression);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> expression);

        Task<PaginatedResult<T>> GetAllPagedAsync<PagedFilter>(
                PagedQuery<PagedFilter> query,
                Func<IQueryable<T>, PagedFilter?, IQueryable<T>>? filterLogic = null);

        Task SaveChangesAsync();
        T Update(T entity);
    }

}
