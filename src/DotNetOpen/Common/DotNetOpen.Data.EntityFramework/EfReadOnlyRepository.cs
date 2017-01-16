using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework
{ 
     public class EfReadOnlyRepository<TDbContext, T> : IReadOnlyRepository<T>
        where T : class
        where TDbContext : EfDbContext
    {
        #region Ctor
        public EfReadOnlyRepository(TDbContext dbContext)
        {
            DbContext = dbContext;
        }
        #endregion

        #region Properties
        public TDbContext DbContext { get; private set; }
        #endregion

        #region Implement IReadOnlyRepository<T>
        public virtual IEnumerable<T> FindAll()
            => DbContext.Set<T>().AsQueryable();
        #endregion
    }
}
