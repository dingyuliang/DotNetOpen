using Castle.Windsor.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common
{
    public class ServiceInstallerFactory : InstallerFactory
    {
        public override IEnumerable<Type> Select(IEnumerable<Type> installerTypes)
        {
            var tupleList = new List<Tuple<int, Type>>();

            foreach (var type in installerTypes)
            {
                var installerAttribute = type.GetCustomAttributes(typeof(ServiceInstallerAttribute), false).Cast<ServiceInstallerAttribute>().FirstOrDefault();
                var order = installerAttribute == null ? 0 : installerAttribute.Order;
                tupleList.Add(new Tuple<int, Type>(order, type));
            }

            return tupleList.OrderBy(a => a.Item1).Select(a => a.Item2).ToList();
        }
    }
}
