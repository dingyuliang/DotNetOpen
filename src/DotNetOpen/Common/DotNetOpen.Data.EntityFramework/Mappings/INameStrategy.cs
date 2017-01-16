using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework.Mappings
{
    public interface INameStrategy
    {
        string ToName(string from);
    }

    public interface ITableNameStrategy
    {
        string ToTable(Type entityType);
    }

    public interface IColumnNameStrategy
    {
        string ToColumn(Type entityType, PropertyInfo propertyInfo);
    }
}
