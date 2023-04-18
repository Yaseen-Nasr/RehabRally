using RehabRally.Core.Consts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RehabRally.Core.Abstractions
{
    public interface IBaseRespository<T> where T : class
    {
        T GetById(int id);
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> Find(Expression<Func<T, bool>> criteria, string[] includes = null);
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> criteria, string[] includes = null);
        IQueryable<T> GetQueryable(string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, string[] includes = null);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> criteria, int? take, int? skip,
         Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        Task<T> Add(T entity);
        Task<IEnumerable<T>> AddRange(IEnumerable<T> entity);
        T Update(T entity);
        void Delete(T entity);
        void DeleteRange(IEnumerable<T> entities);
    }
}
