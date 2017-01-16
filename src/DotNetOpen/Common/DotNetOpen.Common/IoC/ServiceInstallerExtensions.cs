
using Castle.Windsor;

namespace DotNetOpen.Common
{
    public static class ServiceInstallerExtensions
    {
        public static IWindsorContainer AddInstaller<TServiceInstaller>(this IWindsorContainer container)
         where TServiceInstaller : IServiceInstaller, new()
            => new TServiceInstaller().Install(container);
    }
}
