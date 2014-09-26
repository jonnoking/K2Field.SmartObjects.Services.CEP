using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Data
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> All();
        IQueryable<T> All(Func<T, bool> where);
        IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties);
        T Find(Guid id);
        //void InsertOrUpdate(T entity);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Guid id);
        void Detach(T entity);
    }
}
