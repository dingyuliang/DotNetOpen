using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DotNetOpen.Common;
using System;
using System.Linq;

namespace DotNetOpen.Data.EntityFramework
{
    public static class EntityServiceExtenions
    {
        /// <summary>
        /// Add Entity Services by Entity Types 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddEntityServices(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Type[] domainTypes)
            => Check.NotNull(services, nameof(services)).Register(domainTypes.Select(a => Component.For(typeof(IEntityService<>).MakeGenericType(a)).ImplementedBy(typeof(EntityService<>).MakeGenericType(a)).SetupLifestyle(serviceLifetime)).ToArray());

        /// <summary>
        /// Add ReadOnly Entity Services by Entity Types 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddReadOnlyEntityServices(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Type[] domainTypes)
            => Check.NotNull(services, nameof(services)).Register(domainTypes.Select(a => Component.For(typeof(IReadOnlyEntityService<>).MakeGenericType(a)).ImplementedBy(typeof(ReadOnlyEntityService<>).MakeGenericType(a)).SetupLifestyle(serviceLifetime)).ToArray());

        /// <summary>
        /// Add Mapping, Repository, Entity Service by Entity Types
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="mappingLifetime"></param>
        /// <param name="repositoryLifetime"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddDbAccessService<TDbContext>(this IWindsorContainer services,
                                                                       ServiceLifetime mappingLifetime = ServiceLifetime.Singleton,
                                                                       ServiceLifetime repositoryLifetime = ServiceLifetime.Singleton,
                                                                       ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
                                                                       params Type[] domainTypes)
            where TDbContext : EfDbContext
            => services.AddMappings(mappingLifetime, domainTypes)
                       .AddRepositories<TDbContext>(repositoryLifetime, domainTypes)
                       .AddEntityServices(serviceLifetime, domainTypes);

        /// <summary>
        /// Add Mapping, Repository, Entity Service by Entity Types
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="mappingLifetime"></param>
        /// <param name="repositoryLifetime"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddReadOnlyDbAccessService<TDbContext>(this IWindsorContainer services,
                                                                       ServiceLifetime mappingLifetime = ServiceLifetime.Singleton,
                                                                       ServiceLifetime repositoryLifetime = ServiceLifetime.Singleton,
                                                                       ServiceLifetime serviceLifetime = ServiceLifetime.Singleton,
                                                                       params Type[] domainTypes)
            where TDbContext : EfDbContext
            => services.AddMappings(mappingLifetime, domainTypes)
                       .AddReadOnlyRepositories<TDbContext>(repositoryLifetime, domainTypes)
                       .AddReadOnlyEntityServices(serviceLifetime, domainTypes);
    }
}
