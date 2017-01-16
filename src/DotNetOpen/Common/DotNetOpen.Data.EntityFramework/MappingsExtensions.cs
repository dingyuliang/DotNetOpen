using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DotNetOpen.Common;
using DotNetOpen.Data.EntityFramework.Mappings;
using System;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DotNetOpen.Data.EntityFramework
{
    public static class MappingsExtensions
    {
        #region Mapping Configuration
        /// <summary>
        /// Configure Mapping with Keys
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKeys"></typeparam>
        /// <param name="typeBuilder"></param>
        /// <param name="keys">can be: new {a.ID, a.Name}</param>
        /// <returns></returns>
        public static EntityTypeConfiguration<TEntity> HasKeys<TEntity, TKeys>(this EntityTypeConfiguration<TEntity> typeBuilder, Expression<Func<TEntity, TKeys>> keys)
            where TEntity : class
            => Check.NotNull(typeBuilder, nameof(typeBuilder)).HasKey(keys);
        #endregion

        #region Add Mapping
        /// <summary>
        /// Add Mapping by Assemblies, all Entity Mapping Types which implements IEntityMapping and not abstract, and have parameterless constructor.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMappings(this IWindsorContainer services, params Assembly[] assemblies)
            => services.Register(
                assemblies.Select(a => Classes.FromAssembly(a)
                       .Where(type => typeof(IEntityMapping).IsAssignableFrom(type) && !type.IsAbstract && type.GetConstructors().Any(c => !c.GetParameters().Any()))
                       .WithService.AllInterfaces()).ToArray()
               );

        /// <summary>
        /// Add Entity Mapping by Entity Types and using EntityTableNameNameStrategy and PrimitiveColumnNameNameStrategy (LifestyleSingleton)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMappings(this IWindsorContainer services, params Type[] domainTypes)
            => Check.NotNull(services, nameof(services)).Register(domainTypes.Select(a => Component.For(typeof(IEntityMapping), typeof(IEntityMapping<>).MakeGenericType(a)).ImplementedBy(typeof(EntityMapping<>).MakeGenericType(a)).LifestyleSingleton()).ToArray());

        /// <summary>
        /// Add Entity Mapping by Entity Type and using EntityTableNameNameStrategy and PrimitiveColumnNameNameStrategy (LifestyleSingleton)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMapping<TEntity>(this IWindsorContainer services)
            where TEntity : class
            => Check.NotNull(services, nameof(services)).Register(Component.For(typeof(IEntityMapping), typeof(IEntityMapping<TEntity>)).ImplementedBy(typeof(EntityMapping<TEntity>)).LifestyleSingleton());

        /// <summary>
        /// Add Mapping by configuring Entity Type Builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMappingConfig<T>(this IWindsorContainer services, Action<EntityTypeConfiguration<T>> config)
            where T : class
            => Check.NotNull(services, nameof(services)).Register(Component.For(typeof(IEntityMapping), typeof(IEntityMapping<T>)).UsingFactoryMethod(s => new ConfigurableEntityMapping<T>(Check.NotNull(config, nameof(config)))).LifestyleSingleton());

        /// <summary>
        /// Add basic entity mapping by using PrimitiveColumnNameNameStrategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="tableNameStrategyType"></param>
        /// <param name="columnNameStrategyType"></param>
        /// <param name="mappingConfig"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMapping<T>(this IWindsorContainer services, 
                                                      TableNameStrategyType tableNameStrategyType = TableNameStrategyType.EntityTable,
                                                      ColumnNameStrategyType columnNameStrategyType = ColumnNameStrategyType.Primitive,
                                                      Action<EntityTypeConfiguration<T>> mappingConfig = null)
           where T : class
            => Check.NotNull(services, nameof(services)).AddMapping<T>(s =>
            {
                var mapping = new EntityMapping<T>(NameStrategyFactory.GetTableNameStrategy(tableNameStrategyType), NameStrategyFactory.GetColumnNameStrategy(columnNameStrategyType));
                if (mappingConfig != null)
                    mapping.Config(mappingConfig);
                return mapping;
            });

        /// <summary>
        /// Add Mapping into Service Collection base on Entity Mapping Func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="getEntityMappingFunc"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMapping<T>(this IWindsorContainer services, Func<IWindsorContainer, IEntityMapping<T>> getEntityMappingFunc)
            where T : class
        => Check.NotNull(services, nameof(services)).Register(Component.For(typeof(IEntityMapping), typeof(IEntityMapping<T>)).UsingFactoryMethod(() => getEntityMappingFunc(services)).LifestyleSingleton());

        #endregion

        #region Map 
        /// <summary>
        /// Map Entity Type Builder
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TMapping"></typeparam>
        /// <param name="typeBuilder"></param>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public static EntityTypeConfiguration<TEntity> Map<TEntity, TMapping>(this EntityTypeConfiguration<TEntity> typeBuilder, TMapping mapping)
            where TEntity : class
            where TMapping : IEntityMapping<TEntity>
        {
            Check.NotNull(mapping, nameof(mapping)).Map(Check.NotNull(typeBuilder, nameof(typeBuilder)));
            return typeBuilder;
        }
        #endregion
    }
}
