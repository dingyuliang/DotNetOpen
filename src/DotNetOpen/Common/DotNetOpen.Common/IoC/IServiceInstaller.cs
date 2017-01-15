using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks; 

namespace DotNetOpen.Common
{
    public interface IServiceInstaller
    {
        IServiceCollection Install(IServiceCollection serviceCollection);
    }
}
