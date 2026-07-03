using System.Data;

namespace Kyameru.TestUtilities.Localstack;

public class Builder
{
    private readonly IServiceProvider _keyedServiceProvider;
    private List<Resource> _resources = new();
    
    public static Builder Create(IServiceProvider serviceProvider)
    {
        return new Builder(serviceProvider);
    }

    protected Builder(IServiceProvider serviceProvider)
    {
        _keyedServiceProvider = serviceProvider;
    }

    public Builder With(string name, Enums.LocalstackService serviceType, Dictionary<string, string> props)
    {
        if (_resources.Any(x => x.Name == name && x.ServiceType == serviceType))
        {
            throw new DuplicateNameException("Duplicate resource creation.");
        }
        
        _resources.Add(new()
        {
            Name = name,
            ServiceType =  serviceType,
            Props = props
        });

        return this;
    }

    public Session Build()
    {
        return new Session(_resources, _keyedServiceProvider);
    }
}