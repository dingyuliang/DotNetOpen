using Castle.Windsor;

namespace DotNetOpen.Common
{
    public interface IServiceInstaller
    {
        IWindsorContainer Install(IWindsorContainer container);
    }
}
