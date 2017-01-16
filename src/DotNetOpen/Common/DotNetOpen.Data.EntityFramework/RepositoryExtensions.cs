using Castle.MicroKernel.Registration;
using Castle.Windsor;
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
        /// <returns></returns>
        public static IWindsorContainer AddRepository<TDbContext, TEntity>(this IWindsorContainer services)
            where TEntity : class
            where TDbContext : EfDbContext
        => services.Register(Component.For<IRepository<TEntity>>().ImplementedBy<EfRepository<TDbContext, TEntity>>().LifestyleTransient());

        /// <summary>
        /// Register Repositories by Assembly. (LifestyleTransient)
        /// All Repository Types which implement IRepository and is not abstract
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IWindsorContainer AddRepositories(this IWindsorContainer services, params Assembly[] assemblies)
            => services.Register(
                assemblies.Select(a => Classes.FromAssembly(a)
                       .Where(type => typeof(IRepository).IsAssignableFrom(type)
                                  && !type.IsAbstract)
                       .WithServiceAllInterfaces()).ToArray());

        /// <summary>
        /// Register Repositories by Entity Type. (LifestyleTransient)
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <param name="entityTypes"></param>
        /// <returns></returns>
        public static IWindsorContainer AddRepositories<TDbContext>(this IWindsorContainer services, params Type[] entityTypes)
            where TDbContext : EfDbContext
            => services.Register(entityTypes.Select(t => Component.For(typeof(IRepository<>).MakeGenericType(t)).ImplementedBy(typeof(EfRepository<,>).MakeGenericType(typeof(TDbContext), t)).LifestyleTransient()).ToArray());
        #endregion

        #region DbContext
        /// <summary>
        /// Add DbContext (LifestylePerWebRequest)
        /// </summary>
        /// <typeparam name="TDbContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IWindsorContainer AddDbContext<TDbContext>(this IWindsorContainer services)
            where TDbContext : EfDbContext
            => services.Register(Component.For(typeof(EfDbContext), typeof(TDbContext)).ImplementedBy<TDbContext>().LifestylePerWebRequest());
        #endregion
    }
}
