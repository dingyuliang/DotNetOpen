using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data
{
    public class EntityService<T> : IEntityService<T>, IReadOnlyEntityService<T>
        where T : class
    {
        #region Ctor
        public EntityService(IRepository<T> repository)
        {
            Repository = repository;
        }
        #endregion

        #region Properties
        public IRepository<T> Repository { get; private set; }
        #endregion

        #region Implement IEntityService<T>
        public bool Add(T entity)
        {
            return Repository.Add(entity);
        }
        public void AddRange(params T[] entities)
        {
            Repository.AddRange(entities);
        }
        public bool Update(T entity)
        {
            return Repository.Update(entity);
        }
        public void UpdateRange(params T[] entities)
        {
            Repository.UpdateRange(entities);
        }
        public bool Delete(T entity)
        {
            return Repository.Delete(entity);
        }
        public void DeleteRange(params T[] entities)
        {
            Repository.DeleteRange(entities);
        }
        public IEnumerable<T> FindAll()
        {
            return Repository.FindAll();
        }
        #endregion
    }
}
