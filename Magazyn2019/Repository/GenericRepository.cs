using Magazyn2019.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace Magazyn2019.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected Magazyn2019Entities _context = null;
        protected DbSet<T> table = null;

        public GenericRepository()
        {
            this._context = new Magazyn2019Entities();
            table = _context.Set<T>();
        }
        public GenericRepository(Magazyn2019Entities _context)
        {
            this._context = _context;
            table = _context.Set<T>();
        }
        public IEnumerable<T> GetAll()
        {
            return table.ToList();
        }
        public T GetById(object id)
        {
            return table.Find(id);
        }
        public void Insert(T obj)
        {
            table.Add(obj);
        }
        public void Update(T obj)
        {
            table.Attach(obj);
            _context.Entry(obj).State = EntityState.Modified;
        }
        public void Delete(object id)
        {
            T existing = table.Find(id);
            table.Remove(existing);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
        public IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }
        public virtual IQueryable<T1> Get<TResult, T1>(
         Expression<Func<T, bool>> filter = null,
         Expression<Func<T, TResult>> orderBy = null,
         Func<IQueryable<T>, IQueryable<T1>> selector = null,
         string includeProperties = "")
        {
            IQueryable<T> query = table;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }

            if (selector != null)
            {
                return selector(query);
            }
            else
            {
                return (IQueryable<T1>)query.ToList();
            }
        }

    }
}