using Amazon.S3;
using Kyameru.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.S3;

public class Inflator : IOasis
{
    public IAtomicComponent CreateAtomicComponent(Dictionary<string, string> headers)
    {
        throw new NotImplementedException();
    }

    public IFromComponent CreateFromComponent(Dictionary<string, string> headers, bool isAtomic, IServiceProvider serviceProvider)
    {
        throw new NotImplementedException();
    }

    public IToComponent CreateToComponent(Dictionary<string, string> headers, IServiceProvider serviceProvider)
    {
        var component = serviceProvider.GetRequiredService<ITo>();
        component.SetHeaders(headers);
        return component;
    }

    public IServiceCollection RegisterFrom(IServiceCollection serviceCollection)
    {
        throw new NotImplementedException();
    }

    public IServiceCollection RegisterTo(IServiceCollection serviceCollection)
    {
        serviceCollection.AddAWSService<IAmazonS3>();
        serviceCollection.AddTransient<ITo, S3To>();
        return serviceCollection;
    }
}
