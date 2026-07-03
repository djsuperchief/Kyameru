using Kyameru.TestUtilities.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.TestUtilities.Localstack;

public class Session : IDisposable
{
    private readonly List<Resource> _resources;
    private readonly IServiceProvider _keyedServiceProvider;
    private bool _disposed = false;

    public Session(List<Resource> resources, IServiceProvider serviceProvider)
    {
        _resources = resources;
        _keyedServiceProvider = serviceProvider;
    }

    public async Task Init()
    {
        foreach (Resource resource in _resources)
        {
            var service = _keyedServiceProvider.GetRequiredKeyedService<ILocalstackResource>(resource.ServiceType);
            await service.Create(resource.Name, resource.Props);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }
        
        if (disposing)
        {
            foreach (Resource resource in _resources)
            {
                var service = _keyedServiceProvider.GetRequiredKeyedService<ILocalstackResource>(resource.ServiceType);
                Task.Run(() => service.Delete(resource.Name, resource.Props)).GetAwaiter().GetResult();
            }
        }
        
        _disposed = true;
    }
}