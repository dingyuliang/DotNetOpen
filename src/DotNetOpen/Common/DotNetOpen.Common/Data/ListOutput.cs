using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common.Data
{
    public class ListOutput<TModel, TDataInfo>
    {
        public long TotalCount { get; set; }
        public long MatchCount { get; set; }
        public IEnumerable<TModel> Items { get; set; } = new List<TModel>();
        public TDataInfo Data { get; set; }
    }
}
