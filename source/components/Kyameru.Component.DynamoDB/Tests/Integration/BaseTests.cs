using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.DynamoDB.IntegrationTests;

public abstract class BaseTests
{
    protected IServiceCollection ServiceCollection { get; private set; }
    
    protected IServiceProvider ServiceProvider { get; private set; }

    protected BaseTests()
    {
        ServiceCollection = ConfigureServices();
    }

    protected void BuildServiceProvider()
    {
        ServiceProvider = ServiceCollection.BuildServiceProvider();
    }
    
    protected async Task<string> CreateDynamoDbTable(string hashKeyName, string rangeKeyName)
    {
        var tableName = Guid.NewGuid().ToString();
        var dbClient = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
        var keySchema = new List<KeySchemaElement>()
        {
            new()
            {
                AttributeName = hashKeyName,
                KeyType = "HASH"
            }
        };

        var attributes = new List<AttributeDefinition>()
        {
            new()
            {
                AttributeName = hashKeyName,
                AttributeType = "S"
            }
        };

        if (!string.IsNullOrEmpty(rangeKeyName))
        {
            keySchema.Add(new KeySchemaElement(rangeKeyName, KeyType.RANGE));
            attributes.Add(new AttributeDefinition() { AttributeName =  rangeKeyName, AttributeType = "S" });
        }

        var createTableRequest = new CreateTableRequest()
        {
            AttributeDefinitions = attributes,
            KeySchema = keySchema,
            TableName = tableName,
            ProvisionedThroughput = new ProvisionedThroughput(1, 1),
            StreamSpecification = new StreamSpecification()
            {
                StreamEnabled = true,
                StreamViewType = StreamViewType.NEW_AND_OLD_IMAGES
            }
        };
        
        await dbClient.CreateTableAsync(createTableRequest);
        
        return tableName;
    }

    protected async Task DeleteDynamoDbTable(string tableName)
    {
        var dbClient = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
        await dbClient.DeleteTableAsync(tableName, CancellationToken.None);
    }

    protected IServiceCollection ConfigureServices()
    {
        ServiceCollection = new ServiceCollection();
        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
        ServiceCollection.AddLocalStack(config);
        ServiceCollection.AddDefaultAWSOptions(config.GetAWSOptions());
        ServiceCollection.AddAwsService<IAmazonDynamoDB>();
        ServiceCollection.AddScoped<IDynamoDBContext>(x =>
        {
            var dynamoClient = x.GetRequiredService<IAmazonDynamoDB>();
            return new DynamoDBContextBuilder().WithDynamoDBClient(() => dynamoClient).Build();
        });
        AddServices(ServiceCollection);
        ServiceCollection.AddLogging();

        return ServiceCollection;
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

    protected List<T> ConvertGetItemResponse<T>(BatchGetItemResponse? response)
    {
        if (response == null || response.Responses == null || response.Responses.Count <= 0)
        {
            throw new ArgumentException("Response does not contain an item", nameof(response));
        }
        var concrete = new List<T>();
        foreach (var item in response.Responses)
        {
            foreach (var row in item.Value)
            {
                var document = Document.FromAttributeMap(row);
                var json = JsonSerializer.Serialize(document);
                concrete.Add(JsonSerializer.Deserialize<T>(json));
            }
            
        }
        
        return concrete;
    }

    protected async Task<T> GetRecord<T>(IAmazonDynamoDB dbClient, string tableName, Dictionary<string, string> keys)
    {
        var attributes = keys.ToDictionary(key => key.Key, key => new AttributeValue(key.Value));

        var response = await dbClient.GetItemAsync(tableName, attributes);
        var concrete = ConvertGetItemResponse<T>(response);
        return concrete;
    }

    protected async Task<List<T>> GetRecords<T>(IAmazonDynamoDB dbClient, string tableName, Dictionary<string, string> keys)
    {
        var attributes = keys.ToDictionary(key => key.Key, key => new AttributeValue(key.Value));
        var batchItemRequest = CreateBatchGetItemRequest(tableName, attributes);
        var response = await dbClient.BatchGetItemAsync(batchItemRequest);
        
        return ConvertGetItemResponse<T>(response);
    }

    protected async Task<List<T>> QueryRecords<T>(IAmazonDynamoDB dbClient, string tableName,
        Dictionary<string, string> keys)
    {
        var keyConditions = keys.ToDictionary(key => key.Key, key => new Condition()
        {
            ComparisonOperator = "EQ",
            AttributeValueList = [new AttributeValue { S = key.Value }]
        });
        
        var query = await dbClient.QueryAsync(new QueryRequest()
        {
            TableName = tableName,
            KeyConditions = keyConditions
        });
        var concrete = new List<T>();
        foreach (var item in query.Items)
        {
            var document = Document.FromAttributeMap(item);
            concrete.Add(JsonSerializer.Deserialize<T>(document.ToJson()));
        }
        
        return concrete;
    }

    protected BatchGetItemRequest CreateBatchGetItemRequest(string tableName, Dictionary<string, AttributeValue> attributes)
    {
        return new BatchGetItemRequest()
        {
            RequestItems = new Dictionary<string, KeysAndAttributes>()
            {
                {
                    tableName, new KeysAndAttributes()
                    {
                        Keys = [attributes]
                    }
                }
            }
        };
    }

    protected abstract IServiceCollection AddServices(IServiceCollection services);
}