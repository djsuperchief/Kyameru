using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.TestUtilities.Contracts;
using Kyameru.TestUtilities.Localstack.Resources;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.TestUtilities.Localstack;

public static class Installer
{
    public static IServiceCollection InstallLocalstack(this IServiceCollection services, IConfiguration config)
    {
        services.AddLocalStack(config);
        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAwsService<IAmazonDynamoDB>();
        services.AddScoped<IDynamoDBContext>(x =>
        {
            var dynamoClient = x.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContextBuilder().WithDynamoDBClient(() => dynamoClient).Build();
        });
        
        InstallResources(services);
        
        return services;
    }

    private static void InstallResources(IServiceCollection services)
    {
        services.AddKeyedTransient<ILocalstackResource, DynamoDb>(Enums.LocalstackService.DynamoDb);
    }
}