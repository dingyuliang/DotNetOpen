using System;
using System.Linq.Expressions;

namespace DotNetOpen.Common.Extensions
{
    public static class ExpressionsHelper
    {
        public static Expression<Func<T, TResult>> BuildPropertyExpression<T, TResult>(string propertyName)
        {
            var entityExpression = Expression.Parameter(typeof(T), "t");
            var propertyExpression = Expression.Property(entityExpression, propertyName);
            return Expression.Lambda<Func<T, TResult>>(Expression.Convert(propertyExpression, typeof(TResult)), entityExpression);
        }

        public static Expression BuildPropertyExpression(Type entityType, string propertyName)
        {
            var propertyInfo = entityType.GetProperty(propertyName);
            var entityExpression = Expression.Parameter(entityType, "t");
            var propertyExpression = Expression.Property(entityExpression, propertyName);
            return Expression.Lambda(Expression.Convert(propertyExpression, propertyInfo.PropertyType), entityExpression);
        }

        public static string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member.MemberType != System.Reflection.MemberTypes.Property)
                throw new ArgumentOutOfRangeException("memberExpression");
            return memberExpression.Member.Name;
        }
    }
}
