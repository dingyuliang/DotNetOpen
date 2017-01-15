using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNetOpen.Common
{
    /// <summary>
    /// Is Used to convert an value to required type.
    /// </summary>
    public static class ConvertHelper
    {
        #region Static Ctor
        static ConvertHelper()
        { 
        }
        #endregion

        #region Change Type Methods
        public static IEnumerable<T> ChangeListType<T>(IEnumerable<object> value)
        {
            var list = new List<T>();
            foreach (var item in value)
            {
                try
                {
                    list.Add(ChangeType<T>(item));
                }
                catch
                {
                }
            }
            return list;
        }

        public static object ChangeType(object value, Type conversionType)
        {
            if (IsNullableType(conversionType))
            {
                if (value == null)
                    return null;
                else
                { 
                    return Convert.ChangeType(value, conversionType.GetGenericArguments()[0]);
                }
            }
            else
            {
                return Convert.ChangeType(value, conversionType);
            }
        }

        public static T ChangeType<T>(object value)
        {
            var conversionType = typeof(T);
            if (conversionType.IsByRef)
            {
                if (value != null && conversionType.IsAssignableFrom(value.GetType()))
                {
                    return (T)value;
                }
                return default(T);
            }
            else
            {
                return (T)ChangeType(value, conversionType);
            }
        }
        #endregion

        #region Helper Methods
        private static bool IsNullableType(Type type)
        {
            var genericArgues = type.GetGenericArguments();
            if (genericArgues.Count() == 1 && !genericArgues[0].IsByRef)
                return type == typeof(Nullable<>).MakeGenericType(genericArgues);
            return false;
        }
        #endregion
    }
}