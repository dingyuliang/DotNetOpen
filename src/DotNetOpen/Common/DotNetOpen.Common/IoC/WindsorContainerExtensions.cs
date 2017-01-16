using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common
{
    public static class WindsorContainerExtensions
    {
        #region Register
        /// <summary>
        /// Register all types from assemblies
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        /// <param name="typePredicate"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IWindsorContainer Register<TService>(this IWindsorContainer container, Predicate<Type> typePredicate, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies)
        => container.Register(
                assemblies.Select(a => Classes.FromAssembly(a)
                       .Where(type => typeof(TService).IsAssignableFrom(type) && !type.IsAbstract && (typePredicate == null || typePredicate(type)))
                       .WithServiceAllInterfaces().SetupLifestyle(serviceLifetime)
                ).ToArray()
               );

        /// <summary>
        /// Register all types from assemblies
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="container"></param>
        /// <param name="serviceLifetime"></param>
        /// <param name="assemblies"></param>
        /// <returns></returns>
        public static IWindsorContainer Register<TService>(this IWindsorContainer container, ServiceLifetime serviceLifetime = ServiceLifetime.Singleton, params Assembly[] assemblies)
        => container.Register<TService>(null, serviceLifetime, assemblies);
        #endregion

        #region LifeStyle
        /// <summary>
        /// Setup basic LifeStyle, if you want to setup Custom LifeStyle, please use BasedOnDescriptor.LifestyleCustom
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static BasedOnDescriptor SetupLifestyle(this BasedOnDescriptor descriptor, ServiceLifetime serviceLifeTime)
        {
            switch (serviceLifeTime)
            {
                case ServiceLifetime.Scoped:
                    return descriptor.LifestyleScoped();
                case ServiceLifetime.Singleton:
                    return descriptor.LifestyleSingleton();
                case ServiceLifetime.Transient:
                    return descriptor.LifestyleTransient();
                case ServiceLifetime.WebRequest:
                    return descriptor.LifestylePerWebRequest();
                default:
                    return descriptor.LifestyleSingleton();
            }
        }

        /// <summary>
        ///  Setup basic LifeStyle, if you want to setup Custom LifeStyle, please use ComponentRegistration<>.LifestyleCustom
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static ComponentRegistration<object> SetupLifestyle(this ComponentRegistration registration, ServiceLifetime serviceLifeTime)
        {
            switch (serviceLifeTime)
            {
                case ServiceLifetime.Scoped:
                    return registration.LifestyleScoped();
                case ServiceLifetime.Singleton:
                    return registration.LifestyleSingleton();
                case ServiceLifetime.Transient:
                    return registration.LifestyleTransient();
                case ServiceLifetime.WebRequest:
                    return registration.LifestylePerWebRequest();
                default:
                    return registration.LifestyleSingleton();
            }
        }
        /// <summary>
        ///  Setup basic LifeStyle, if you want to setup Custom LifeStyle, please use ComponentRegistration<>.LifestyleCustom
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="registration"></param>
        /// <param name="serviceLifeTime"></param>
        /// <returns></returns>
        public static ComponentRegistration<T> SetupLifestyle<T>(this ComponentRegistration<T> registration, ServiceLifetime serviceLifeTime)
            where T : class
        {
            switch (serviceLifeTime)
            {
                case ServiceLifetime.Scoped:
                    return registration.LifestyleScoped();
                case ServiceLifetime.Singleton:
                    return registration.LifestyleSingleton();
                case ServiceLifetime.Transient:
                    return registration.LifestyleTransient();
                case ServiceLifetime.WebRequest:
                    return registration.LifestylePerWebRequest();
                default:
                    return registration.LifestyleSingleton();
            }
        }
        #endregion
    }
}
