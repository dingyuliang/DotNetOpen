using Castle.Core.Internal;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DotNetOpen.Common
{
    public class WindsorContainerBootstrapper
    {
        #region Const
        /// <summary>
        /// this is required.
        /// </summary>
        const string AppKey_ContainerInstallerAssembliesRegex = "ContainerInstallerAssembliesRegex";
        #endregion

        #region Static Fields
        public static readonly IWindsorContainer Container = null;
        #endregion

        #region Static Ctor
        static WindsorContainerBootstrapper()
        {
            Container = new WindsorContainer();
        }
        #endregion

        #region Initialize
        public static void Initialize()
        {
            Container.Register(Component.For<IWindsorContainer>().Instance(Container));

            var containerInstallerAssembliesRegex = ConfigurationManager.AppSettings[AppKey_ContainerInstallerAssembliesRegex] ?? string.Empty;
            // Controll the order of each installer.
            var installFactory = new ServiceInstallerFactory();
            var assemblyFilter = new AssemblyFilter(Environment.CurrentDirectory);
            var containerAssemlbyFilter = assemblyFilter.FilterByName(a => Regex.IsMatch(a.Name, containerInstallerAssembliesRegex, RegexOptions.IgnoreCase));
            var allAssemblies = (containerAssemlbyFilter as IAssemblyProvider).GetAssemblies();
            var allInstallerTypes = allAssemblies.SelectMany(a => a.GetTypes().Where(b => typeof(IWindsorInstaller).IsAssignableFrom(b) && b.GetConstructor(Type.EmptyTypes) != null))
                                                 .ToList();
            var installers = installFactory.Select(allInstallerTypes).Select(a => installFactory.CreateInstance(a)).ToArray();
            Container.Install(installers);
        }
        #endregion
    }
}
