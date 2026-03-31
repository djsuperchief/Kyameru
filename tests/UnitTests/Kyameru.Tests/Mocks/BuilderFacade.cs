using Kyameru.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Tests.Mocks;

public class BuilderFacade : AbstractBuilder
{
    public void RegisterScheduled(IServiceCollection services)
    {
        base.RegisterScheduledServices(services, "notexist");
    }
    
    public void RegisterTo(IServiceCollection services)
    {
        base.RegisterToServices(services, "notexist");
    }
    
    public void RegisterFrom(IServiceCollection services)
    {
        base.RegisterFromServices(services, "notexist");
    }
}