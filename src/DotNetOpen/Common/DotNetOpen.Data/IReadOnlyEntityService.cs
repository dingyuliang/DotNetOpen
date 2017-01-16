using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data
{
    public interface IReadOnlyEntityService<T>
        where T : class
    {
        IEnumerable<T> FindAll();
    }
}
