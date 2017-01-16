using System.Collections.Generic;

namespace DotNetOpen.Data
{
    public interface IReadOnlyRepository<T>: IRepository
      where T : class
    {
        IEnumerable<T> FindAll();
    }
}
