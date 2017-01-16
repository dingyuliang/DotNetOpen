using System;
using System.Linq;
using DotNetOpen.Data.EntityFramework.Mappings;
using System.Reflection;
using System.Data.Entity.ModelConfiguration;
using System.Linq.Expressions;
using DotNetOpen.Common;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace DotNetOpen.Data.EntityFramework.Mappings
{
    public class EntityMapping<T> : IEntityMapping<T>
        where T : class
    {
        #region Const
        const string COLUMN_ID = "ID";
        #endregion
        
        #region Ctor
        public EntityMapping()
            :this(NameStrategyFactory.EntityTableNameStrategy, null)
        {
        }
        
        public EntityMapping(ITableNameStrategy tableNameStrategy, IColumnNameStrategy columnNameStrategy = null)
        {
            TableNameStrategy = tableNameStrategy;
            ColumnNameStrategy = columnNameStrategy ?? NameStrategyFactory.PrimitiveColumnNameStrategy;
        }
        #endregion

        #region Properties
        public ITableNameStrategy TableNameStrategy { get; protected set; }
        public IColumnNameStrategy ColumnNameStrategy { get; protected set; }
        protected Action<EntityTypeConfiguration<T>> TypeBuilderAction { get; private set; }
        #endregion

        #region Methods
        public void Config(Action<EntityTypeConfiguration<T>> typeBuilderAction)
        {
            TypeBuilderAction = typeBuilderAction;
        }
        #endregion

        #region Implement IEntityMapping<T>
        public Type EntityType { get { return typeof(T); } }
        #endregion

        #region Implement IEntityMapping<T>
        public virtual void Map(EntityTypeConfiguration<T> typeBuilder)
        {
            typeBuilder.ToTable(TableNameStrategy.ToTable(EntityType));
            var properties = EntityType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.SetProperty)
                                       .Where(a => a.GetCustomAttribute<IgnoreMappingAttribute>() == null
                                                  && a.SetMethod != null
                                                  && !a.GetMethod.IsAbstract)
                                       .ToList();

            var primaryColumnsProperties = properties.Where(a =>//!a.GetMethod.IsVirtual &&
                                                                !a.PropertyType.IsArray)
                                                     .ToList();

            MapPrimitivePropertiesToColumns(typeBuilder, ColumnNameStrategy);

            if (TypeBuilderAction != null)
            {
                TypeBuilderAction(typeBuilder);
            }
        }
        #endregion

        #region Helper Methods
        protected virtual void MapPrimitivePropertiesToColumns(EntityTypeConfiguration<T> configuration, IColumnNameStrategy columnNameMappingStrategy)
        {
            if (configuration == null || columnNameMappingStrategy == null)
                return;
            var entityType = typeof(T);
            var properties =
                entityType.GetProperties().Where(
                    p => p.CanRead
                      && p.CanWrite
                      && p.GetCustomAttributes(typeof(IgnoreMappingAttribute), false).Count() == 0
                      );
            var isNonAutoIncrementIDType = typeof(INonAutoIncrementID).IsAssignableFrom(entityType);
            foreach (var propertyInfo in properties)
            {
                var primaryPropertyConfiguration = default(PrimitivePropertyConfiguration);
                var entityExpression = Expression.Parameter(typeof(T), "t");
                var propertyExpression = Expression.Property(entityExpression, propertyInfo);
                var columnName = columnNameMappingStrategy.ToColumn(EntityType, propertyInfo);

                if (propertyInfo.PropertyType.Equals(typeof(Int16)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Int16>>(
                        Expression.Convert(propertyExpression, typeof(Int16)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Int16?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Int16?>>(
                        Expression.Convert(propertyExpression, typeof(Int16?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Int32)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Int32>>(
                        Expression.Convert(propertyExpression, typeof(Int32)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Int32?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Int32?>>(
                        Expression.Convert(propertyExpression, typeof(Int32?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Int64)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Int64>>(
                        Expression.Convert(propertyExpression, typeof(Int64)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Int64?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Int64?>>(
                        Expression.Convert(propertyExpression, typeof(Int64?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Single)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Single>>(
                        Expression.Convert(propertyExpression, typeof(Single)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Single?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Single?>>(
                        Expression.Convert(propertyExpression, typeof(Single?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Double)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Double>>(
                        Expression.Convert(propertyExpression, typeof(Double)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Double?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Double?>>(
                        Expression.Convert(propertyExpression, typeof(Double?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(DateTime)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, DateTime>>(
                        Expression.Convert(propertyExpression, typeof(DateTime)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(DateTime?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, DateTime?>>(
                        Expression.Convert(propertyExpression, typeof(DateTime?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(DateTimeOffset)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, DateTimeOffset>>(
                        Expression.Convert(propertyExpression, typeof(DateTimeOffset)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(DateTimeOffset?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, DateTimeOffset?>>(
                        Expression.Convert(propertyExpression, typeof(DateTimeOffset?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(TimeSpan)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, TimeSpan>>(
                        Expression.Convert(propertyExpression, typeof(TimeSpan)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(TimeSpan?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, TimeSpan?>>(
                        Expression.Convert(propertyExpression, typeof(TimeSpan?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Decimal)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Decimal>>(
                        Expression.Convert(propertyExpression, typeof(Decimal)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Decimal?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Decimal?>>(
                        Expression.Convert(propertyExpression, typeof(Decimal?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Boolean)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Boolean>>(
                        Expression.Convert(propertyExpression, typeof(Boolean)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Boolean?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Boolean?>>(
                        Expression.Convert(propertyExpression, typeof(Boolean?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Char)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Char>>(
                        Expression.Convert(propertyExpression, typeof(Char)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Char?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Char?>>(
                        Expression.Convert(propertyExpression, typeof(Char?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Guid)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Guid>>(
                        Expression.Convert(propertyExpression, typeof(Guid)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(Guid?)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, Guid?>>(
                        Expression.Convert(propertyExpression, typeof(Guid?)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(String)))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, String>>(
                        Expression.Convert(propertyExpression, typeof(String)), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else if (propertyInfo.PropertyType.Equals(typeof(byte[])))
                {
                    var propertyLambdaExpression = Expression.Lambda<Func<T, byte[]>>(
                        Expression.Convert(propertyExpression, typeof(byte[])), entityExpression);
                    primaryPropertyConfiguration = configuration.Property(propertyLambdaExpression).HasColumnName(columnName);
                }
                else
                {
                    //do nothing
                    //throw new NotSupportedException(
                    //    "Can not map property {0} to column as the type of the property {1} is not supported");
                }

                if (isNonAutoIncrementIDType && primaryPropertyConfiguration != null)
                {
                    primaryPropertyConfiguration.HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);
                }
            }

        }
        #endregion
    }
}
