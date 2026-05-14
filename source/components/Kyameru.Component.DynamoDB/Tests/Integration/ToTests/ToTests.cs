using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.IntegrationTests.DynamoDbRecords;
using Kyameru.Core.Entities;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.DynamoDB.IntegrationTests.ToTests;

public class ToTests : BaseTests
{
    [Fact]
    public async Task ToCanWriteToDynamoDb()
    {
        var provider =  ConfigureServices().BuildServiceProvider();
        var table = await CreateDynamoDbTable(provider);
        var dbRecord = new DynamoDbRecords.BasicRecord("This is a test");
        var routable = new Routable(new Dictionary<string, string>() {{ "DynamoDbOverrideTable", table}}, dbRecord);
        var to = provider.GetRequiredService<ITo>();
        await to.ProcessAsync(routable, CancellationToken.None);
        
        var dbContext = provider.GetRequiredService<IAmazonDynamoDB>();
        var record = await GetRecord<BasicRecord>(dbContext, table, new Dictionary<string, string>()
        {
            { "Identity", dbRecord.HashKey},
            { "Title", dbRecord.RangeKey }
        });
        
        Assert.Equal(JsonSerializer.Serialize(dbRecord), JsonSerializer.Serialize(record));
        
        await DeleteDynamoDbTable(provider, table);
    }
}