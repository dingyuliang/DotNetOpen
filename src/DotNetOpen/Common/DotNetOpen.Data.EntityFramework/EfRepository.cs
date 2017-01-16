using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework
{
    public class EfRepository<TDbContext, T> : IRepository<T>
        where T : class
        where TDbContext : EfDbContext
    {
        #region Ctor
        public EfRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
        #endregion

        #region Properties
        public TDbContext DbContext { get; private set; }
        #endregion

        #region Implement IRepository<T>
        public virtual bool Add(T entity)
        {
            DbContext.Set<T>().Add(entity);
            return SaveChanges() > 0;
        }

        public virtual void AddRange(params T[] entities)
        {
            DbContext.Set<T>().AddRange(entities);
            SaveChanges();
        }

        public virtual bool Delete(T entity)
        {
            DbContext.Set<T>().Remove(entity);
            return SaveChanges() > 0;
        }

        public virtual void DeleteRange(params T[] entities)
        {
            DbContext.Set<T>().RemoveRange(entities);
            SaveChanges();
        }

        public virtual IEnumerable<T> FindAll()
            => DbContext.Set<T>().AsQueryable();

        public virtual bool Update(T entity)
        {
            return SaveChanges() > 0;
        }

        public virtual void UpdateRange(params T[] entities)
        {
            SaveChanges();
        }

        protected virtual int SaveChanges()
            => DbContext.SaveChanges();
        #endregion
    }
}
