using Castle.MicroKernel.Registration;
using Castle.Windsor;
using DotNetOpen.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Data.EntityFramework
{
    public static class RepositoryExtensions
    {
        #region Repository Extensions
        /// <summary>
        /// Register Repository by Entity Type. (LifestyleTransient)
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddRepository<TDbContext, TEntity>(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TEntity : class
            where TDbContext : EfDbContext
        => services.Register(Component.For<IRepository<TEntity>>().ImplementedBy<EfRepository<TDbContext, TEntity>>().SetupLifestyle(serviceLifetime));

        /// <summary>
        /// Register Repositories by Assembly. (LifestyleTransient)
        /// All Repository Types which implement IRepository and is not abstract
        /// </summary>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IWindsorContainer AddRepositories(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies)
            => services.Register<IRepository>(serviceLifetime, assemblies);

        /// <summary>
        /// Register Repositories by Entity Type. (LifestyleTransient)
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="entityTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddRepositories<TDbContext>(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Type[] entityTypes)
            where TDbContext : EfDbContext
            => services.Register(entityTypes.Select(t => Component.For(typeof(IRepository<>).MakeGenericType(t)).ImplementedBy(typeof(EfRepository<,>).MakeGenericType(typeof(TDbContext), t)).SetupLifestyle(serviceLifetime)).ToArray());

        /// <summary>
        /// Register Repositories by Entity Type. (LifestyleTransient)
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="entityTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddReadOnlyRepositories<TDbContext>(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Type[] entityTypes)
            where TDbContext : EfDbContext
            => services.Register(entityTypes.Select(t => Component.For(typeof(IReadOnlyRepository<>).MakeGenericType(t)).ImplementedBy(typeof(EfReadOnlyRepository<,>).MakeGenericType(typeof(TDbContext), t)).SetupLifestyle(serviceLifetime)).ToArray());
        #endregion

        #region DbContext
        /// <summary>
        /// Add DbContext (LifestylePerWebRequest)
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="serviceLifetime"></param>
        /// <returns></returns>
        public static IWindsorContainer AddDbContext<TDbContext>(this IWindsorContainer services, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton)
            where TDbContext : EfDbContext
            => services.Register(Component.For(typeof(EfDbContext), typeof(TDbContext)).ImplementedBy<TDbContext>().SetupLifestyle(serviceLifetime));

        /// <summary>
        /// Add Mapping and Repository to Container.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="mappingLifetime"></param>
        /// <param name="repositoryLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddDbAccess<TDbContext>(this IWindsorContainer services,
                                                                     ServiceLifetime mappingLifetime = ServiceLifetime.Singleton,
                                                                     ServiceLifetime repositoryLifetime = ServiceLifetime.Singleton,
                                                                     params Type[] domainTypes)
            where TDbContext : EfDbContext
            => services.AddMappings(mappingLifetime, domainTypes).AddRepositories<TDbContext>(repositoryLifetime, domainTypes);

        /// <summary>
        /// Add Mapping and Repository to Container.
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="mappingLifetime"></param>
        /// <param name="repositoryLifetime"></param>
        /// <param name="domainTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddReadOnlyDbAccess<TDbContext>(this IWindsorContainer services,
                                                                     ServiceLifetime mappingLifetime = ServiceLifetime.Singleton,
                                                                     ServiceLifetime repositoryLifetime = ServiceLifetime.Singleton,
                                                                     params Type[] domainTypes)
            where TDbContext : EfDbContext
            => services.AddMappings(mappingLifetime, domainTypes).AddReadOnlyRepositories<TDbContext>(repositoryLifetime, domainTypes);
        #endregion
    }
}
