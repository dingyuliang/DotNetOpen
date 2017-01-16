using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common.IoC
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServiceInstallerAttribute : Attribute
    {
        public ServiceInstallerAttribute(int order = 0)
        {
            Order = order;
        }

        public int Order { get; set; }
    }
}
