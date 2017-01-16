using Castle.Core.Configuration;
using Castle.MicroKernel;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetOpen.Common.IoC
{
    public class WindsorArrayFacility: IFacility
    {
        #region Implement IFacility
        public void Init(IKernel kernel, IConfiguration facilityConfig)
        {
            kernel.Resolver.AddSubResolver(new ArrayResolver(kernel));
        }

        public void Terminate()
        {
        }
        #endregion
    }
}
