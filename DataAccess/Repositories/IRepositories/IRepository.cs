using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<bool> CreateAsync(T entity);

        Task<bool> EditAsync(T entity);

        Task<bool> DeleteAsync(T entity);
        Task<bool> DeleteAllAsync(IEnumerable<T> entities);
        IQueryable<T> GetQueryable(Expression<Func<T, bool>>? expression = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool tracked = true);

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool tracked = true
            , int? skip = null, int? take = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null);

        Task<T?> GetOneAsync(Expression<Func<T, bool>>? expression = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool tracked = true);

        Task<bool> CommitAsync();

        Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

        Task<bool> CreateAllAsync(IEnumerable<T> entities);
        Task<bool> EditAllAsync(IEnumerable<T> entities);
    }
}
