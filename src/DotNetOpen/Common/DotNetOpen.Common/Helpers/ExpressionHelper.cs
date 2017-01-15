using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetOpen.Common
{
    public static class ExpressionHelper
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
            return Expression.Lambda(propertyExpression, entityExpression);
            // don't need to convert
            // return Expression.Lambda(Expression.Convert(propertyExpression, propertyInfo.PropertyType), entityExpression);
        }

        public static string GetPropertyName<T, TResult>(Expression<Func<T, TResult>> propertyExpression)
        {
            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null || memberExpression.Member.MemberType != System.Reflection.MemberTypes.Property)
                throw new ArgumentOutOfRangeException(nameof(propertyExpression), $"{propertyExpression} is not a property expression.");
            return memberExpression.Member.Name;
        }

        public static Type GetPropertyType(LambdaExpression lambdaExpression)
        {
            if (lambdaExpression.Body is MemberExpression)
                return ((lambdaExpression.Body as MemberExpression).Member as PropertyInfo).PropertyType;
            return null;
        }
    }
}
