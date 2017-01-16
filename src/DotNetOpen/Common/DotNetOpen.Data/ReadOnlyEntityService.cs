using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data
{
    public class ReadOnlyEntityService<T> : IReadOnlyEntityService<T>
        where T : class
    {
        #region Ctor
        public ReadOnlyEntityService(IReadOnlyRepository<T> repository)
        {
            Repository = repository;
        }
        #endregion

        #region Properties
        public IReadOnlyRepository<T> Repository { get; private set; }
        #endregion

        #region Implement IEntityService<T> 
        public IEnumerable<T> FindAll()
        {
            return Repository.FindAll();
        }
        #endregion
    }
}
