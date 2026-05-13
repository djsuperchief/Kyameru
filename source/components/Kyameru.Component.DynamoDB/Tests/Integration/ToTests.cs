using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Core.Entities;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.DynamoDB.IntegrationTests;

public class ToTests
{
    [Fact]
    public async Task ToCanWriteToDynamoDb()
    {
        var services =  ConfigureServices();
        services.AddTransient<IDynamoDbUpserter, DynamoDbUpserter>();
        services.AddTransient<ITo, DynamoDbTo>();
        var dbRecord = new DynamoDbRecords.BasicRecord("This is a test");
        var routable = new Routable(new Dictionary<string, string>() {{ "DynamoDbOverrideTable", "KyameruTestFrom"}}, dbRecord);
        var provider = services.BuildServiceProvider();
        var to = provider.GetRequiredService<ITo>();
        await to.ProcessAsync(routable, CancellationToken.None);
        
        
    }

    private IServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        services.AddLocalStack(config);
        services.AddDefaultAWSOptions(config.GetAWSOptions());
        services.AddAwsService<IAmazonDynamoDB>();
        services.AddScoped<IDynamoDBContext>(x =>
        {
            var dynamoClient = x.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContextBuilder().WithDynamoDBClient(() => dynamoClient).Build();
        });
        services.AddLogging();

        return services;
    }
}