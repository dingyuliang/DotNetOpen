using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common
{
    public enum ServiceLifetime
    { 
        /// <summary>
        /// Specifies that a single instance of the service will be created. 
        /// </summary>        
        Singleton = 0,
        /// <summary>
        /// Specifies that a new instance of the service will be created for each scope.
        /// </summary>
        Scoped = 1,
        /// <summary>
        /// Specifies that a new instance of the service will be created every time it is requested.
        /// </summary>
        Transient = 2,
        /// <summary>
        /// Specifies that a new instance of the service will be created every request; for ASP.NET, need to configure Module.
        /// </summary>
        WebRequest = 3
    }
}
