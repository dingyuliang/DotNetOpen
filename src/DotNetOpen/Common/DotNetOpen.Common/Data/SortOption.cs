using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common.Data
{
    public class SortOption
    {
        public string Field { get; set; }

        public SortDirection Direction { get; set; } = SortDirection.ASC;
    }

}
