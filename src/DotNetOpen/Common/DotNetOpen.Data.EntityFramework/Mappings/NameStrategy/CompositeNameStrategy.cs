using System.Collections.Generic;
using System.Linq;

namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class CompositeNameStrategy : INameStrategy
    {
        public ICollection<INameStrategy> Strategies { get; set; }

        public string ToName(string from)
        {
            return Strategies.Aggregate(from, (current, strategy) => strategy.ToName(current));
        }
    }
}