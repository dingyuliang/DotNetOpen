using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetOpen.Common
{
    public static class ServiceInstallerExtensions
    {
        public static IServiceCollection AddInstaller<TServiceInstaller>(this IServiceCollection services)
         where TServiceInstaller : IServiceInstaller, new()
            => new TServiceInstaller().Install(services);
    }
}
