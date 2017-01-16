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
        /// <param name="serviceLifetime"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMappings(this IWindsorContainer services, ServiceLifetime serviceLifetime= ServiceLifetime.Singleton,  params Assembly[] assemblies)
            => services.Register<IEntityMapping>(t => t.GetConstructors().Any(c => !c.GetParameters().Any()), serviceLifetime, assemblies);

        /// <summary>
        /// Add Entity Mapping by Entity Types and using EntityTableNameNameStrategy and PrimitiveColumnNameNameStrategy (LifestyleSingleton)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMappings(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Type[] domainTypes)
            => Check.NotNull(services, nameof(services)).Register(domainTypes.Select(a => Component.For(typeof(IEntityMapping), typeof(IEntityMapping<>).MakeGenericType(a)).ImplementedBy(typeof(EntityMapping<>).MakeGenericType(a)).SetupLifestyle(serviceLifetime)).ToArray());

        /// <summary>
        /// Add Entity Mapping by Entity Type and using EntityTableNameNameStrategy and PrimitiveColumnNameNameStrategy (LifestyleSingleton)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMapping<TEntity>(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEntity : class
            => Check.NotNull(services, nameof(services)).Register(Component.For(typeof(IEntityMapping), typeof(IEntityMapping<TEntity>)).ImplementedBy(typeof(EntityMapping<TEntity>)).SetupLifestyle(serviceLifetime));

        /// <summary>
        /// Add Mapping by configuring Entity Type Builder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMappingConfig<T>(this IWindsorContainer services, Action<EntityTypeConfiguration<T>> config, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where T : class
            => Check.NotNull(services, nameof(services)).Register(Component.For(typeof(IEntityMapping), typeof(IEntityMapping<T>)).UsingFactoryMethod(s => new ConfigurableEntityMapping<T>(Check.NotNull(config, nameof(config)))).SetupLifestyle(serviceLifetime));

        /// <summary>
        /// Add basic entity mapping by using PrimitiveColumnNameNameStrategy
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="tableNameStrategyType"></param>
        /// <param name="columnNameStrategyType"></param>
        /// <param name="mappingConfig"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMapping<T>(this IWindsorContainer services, 
                                                      TableNameStrategyType tableNameStrategyType = TableNameStrategyType.EntityTable,
                                                      ColumnNameStrategyType columnNameStrategyType = ColumnNameStrategyType.Primitive,
                                                      Action<EntityTypeConfiguration<T>> mappingConfig = null, 
                                                      ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
           where T : class
            => Check.NotNull(services, nameof(services)).AddMapping<T>(s =>
            {
                var mapping = new EntityMapping<T>(NameStrategyFactory.GetTableNameStrategy(tableNameStrategyType), NameStrategyFactory.GetColumnNameStrategy(columnNameStrategyType));
                if (mappingConfig != null)
                    mapping.Config(mappingConfig);
                return mapping;
            }, serviceLifetime);

        /// <summary>
        /// Add Mapping into Service Collection base on Entity Mapping Func
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="getEntityMappingFunc"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddMapping<T>(this IWindsorContainer services, Func<IWindsorContainer, IEntityMapping<T>> getEntityMappingFunc,
                                                      ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where T : class
        => Check.NotNull(services, nameof(services)).Register(Component.For(typeof(IEntityMapping), typeof(IEntityMapping<T>)).UsingFactoryMethod(() => getEntityMappingFunc(services)).SetupLifestyle(serviceLifetime));

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
