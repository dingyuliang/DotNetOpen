using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetOpen.Data
{
    /// <summary>
    /// for IoC purpose
    /// </summary>
    public interface IRepository
    {
    }

    public interface IRepository<T>: IRepository, IReadOnlyRepository<T>
        where T : class
    {
        bool Add( T entity);
        void AddRange(params T[] entities);
        bool Update(T entity);
        void UpdateRange(params T[] entities);
        bool Delete(T entity);
        void DeleteRange(params T[] entities); 
    }
}
