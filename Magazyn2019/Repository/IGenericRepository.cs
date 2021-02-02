using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Magazyn2019.Repository
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(object id);
        void Insert(T obj);
        void Update(T obj);
        void Delete(object id);
        void Save();
        IEnumerable<T> Get(
                       Expression<Func<T, bool>> filter,
                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy,
                       string includeProperties);
        IQueryable<T1> Get<TResult, T1>(
                       Expression<Func<T, bool>> filter,
                       Expression<Func<T, TResult>> orderBy,
                       Func<IQueryable<T>, IQueryable<T1>> selector,
                       string includeProperties);
    }
}
