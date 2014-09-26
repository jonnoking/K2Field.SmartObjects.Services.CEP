using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace K2Field.SmartObjects.Services.CEP.Data
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        protected DbSet<T> DBSet { get; set; }
        protected DbContext Context { get; set; }

        public GenericRepository(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("An instance of DbContext is " +
                    "required to use this repository.", "context");
            }

            this.Context = context;
            this.DBSet = this.Context.Set<T>();

            this.Context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
        }

        public virtual IQueryable<T> All()
        {
            return DBSet;
        }

        public virtual IQueryable<T> All(Func<T, bool> where)
        {
            return DBSet.Where(where).AsQueryable();
        }

        public virtual IQueryable<T> AllIncluding(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = DBSet;
            foreach (var includeProperty in includeProperties)
            {
                query = query.Include(includeProperty);
            }
            return DBSet;
        }

        public virtual T Find(Guid id)
        {
            return DBSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                this.DBSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.DBSet.Attach(entity);
            }

            entry.State = EntityState.Modified;
        }

        public virtual void Detach(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);

            entry.State = EntityState.Detached;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry entry = this.Context.Entry(entity);

            if (entry.State != EntityState.Deleted)
            {
                entry.State = EntityState.Deleted;
            }
            else
            {
                this.DBSet.Attach(entity);
                this.DBSet.Remove(entity);
            }
        }

        public virtual void Delete(Guid id)
        {
            var entity = this.Find(id);

            if (entity != null)
            {
                this.Delete(entity);
            }
        }


        //public virtual void InsertOrUpdate(T entity)
        //{
        //    if (entity.Id == default(int))
        //    {
        //        // New entity
        //        DBSet.Add(EventType);
        //    }
        //    else
        //    {
        //        // Existing entity
        //        context.Entry(EventType).State = EntityState.Modified;
        //    }
        //}



    }
}
