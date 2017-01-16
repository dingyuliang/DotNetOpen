using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class CompositeColumnNameStrategy : IColumnNameStrategy
    {
        public CompositeColumnNameStrategy()
        {
            Strategies = new List<INameStrategy>();
        }

        /// <summary>
        /// get or set column name strategy which will be always the first one.
        /// </summary>
        public IColumnNameStrategy ColumnNameStrategy { get; set; }

        /// <summary>
        /// get or set the following name strategories
        /// </summary>
        public ICollection<INameStrategy> Strategies { get; set; }

        public string ToColumn(Type entityType, PropertyInfo propertyInfo)
        {
            var columnName = ColumnNameStrategy != null ? ColumnNameStrategy.ToColumn(entityType, propertyInfo) : propertyInfo.Name;
            return Strategies.Aggregate(columnName, (current, strategy) => strategy.ToName(columnName));
        }
    }
}
