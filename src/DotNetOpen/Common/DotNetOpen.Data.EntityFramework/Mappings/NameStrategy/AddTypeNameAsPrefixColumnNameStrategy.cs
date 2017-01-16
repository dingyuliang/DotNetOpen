using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework.Mappings.NameStrategy
{
    public class AddTypeNameAsPrefixColumnNameStrategy : IColumnNameStrategy
    {
        #region Implement IColumnNameStrategy
        public string ToColumn(Type entityType, PropertyInfo propertyInfo)
        {
            var prefix = entityType.Name;
            var from = propertyInfo.Name;
            return prefix == null ? from : prefix + from;
        }
        #endregion
    }
}
