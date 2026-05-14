using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Kyameru.Component.DynamoDB.Contracts;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.DynamoDB.IntegrationTests.ToTests;

public abstract class BaseTests
{
    protected async Task<string> CreateDynamoDbTable(IServiceProvider provider)
    {
        var tableName = Guid.NewGuid().ToString();
        var dbClient = provider.GetRequiredService<IAmazonDynamoDB>();
        await dbClient.CreateTableAsync(tableName, [
            new()
            {
                AttributeName = "Identity",
                KeyType = "HASH"
            },

            new()
            {
                AttributeName = "Title",
                KeyType = "RANGE"
            }
        ], [
            new()
            {
                AttributeName = "Identity",
                AttributeType = "S"
            },

            new()
            {
                AttributeName = "Title",
                AttributeType = "S"
            }
        ], new ProvisionedThroughput(1, 1));
        
        return tableName;
    }

    protected async Task DeleteDynamoDbTable(IServiceProvider provider, string tableName)
    {
        var dbClient = provider.GetRequiredService<IAmazonDynamoDB>();
        await dbClient.DeleteTableAsync(tableName, CancellationToken.None);
    }

    protected IServiceCollection ConfigureServices()
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
        services.AddTransient<IDynamoDbUpserter, DynamoDbUpserter>();
        services.AddTransient<ITo, DynamoDbTo>();
        services.AddLogging();

        return services;
    }
    
    protected T ConvertGetItemResponse<T>(GetItemResponse? response)
    {
        if (response == null || response.Item == null || response.Item.Count <= 0)
        {
            throw new ArgumentException("Response does not contain an item", nameof(response));
        }

        var document = Document.FromAttributeMap(response.Item);
        return JsonSerializer.Deserialize<T>(document.ToJson())!;

    }

    protected async Task<T> GetRecord<T>(IAmazonDynamoDB dbClient, string tableName, Dictionary<string, string> keys)
    {
        var attributes = keys.ToDictionary(key => key.Key, key => new AttributeValue(key.Value));

        var response = await dbClient.GetItemAsync(tableName, attributes);
        var concrete = ConvertGetItemResponse<T>(response);
        return concrete;
    }
}