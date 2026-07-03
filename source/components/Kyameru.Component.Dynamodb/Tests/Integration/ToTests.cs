using System.Text.Json;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Kyameru.Component.Dynamodb.Contracts;
using Kyameru.Component.Dynamodb.IntegrationTests.DynamoDbRecords;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Kyameru.TestUtilities.Enums;
using Kyameru.TestUtilities.Localstack;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kyameru.Component.Dynamodb.IntegrationTests.ToTests;

public class ToTests : BaseTests
{
    [Fact]
    public async Task ToCanWriteToDynamoDb()
    {
        var table = Guid.NewGuid().ToString();
        BuildServiceProvider();
        using (var localstackSession = Builder.Create(ServiceProvider)
                   .With(table, LocalstackService.DynamoDb, new Dictionary<string, string>()
                   {
                       { "HashKey", "Department" },
                       { "RangeKey", "Identity" }
                   }).Build())
        {
            await localstackSession.Init();
            var dbRecord = new BasicRecord("This is a test", "HR");
            var routable = new Routable(new Dictionary<string, string>() { { "DynamoDbOverrideTable", table } },
                dbRecord);
            var to = ServiceProvider.GetRequiredService<ITo>();
            await to.ProcessAsync(routable, CancellationToken.None);

            var dbContext = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
            var record = await GetRecord<BasicRecord>(dbContext, table, new Dictionary<string, string>()
            {
                { "Department", dbRecord.HashKey },
                { "Identity", dbRecord.Identity }
            });

            Assert.Equal(JsonSerializer.Serialize(dbRecord), JsonSerializer.Serialize(record));
        }
    }

     [Fact]
     public async Task ToCanWiteMultipleDynamoDbRecords()
     {
         BuildServiceProvider();
         var table = Guid.NewGuid().ToString();
         using (var localstackSession = Builder.Create(ServiceProvider)
                    .With(table, LocalstackService.DynamoDb, new Dictionary<string, string>()
                    {
                        { "HashKey", "Department" },
                        { "RangeKey", "Identity" }
                    }).Build())
         {
             await localstackSession.Init();
             var dbRecords = new List<BasicRecord>()
             {
                 new("This is a test", "HR"),
                 new("This is another test", "HR"),
             };
             var routable = new Routable(new Dictionary<string, string>() { { "DynamoDbOverrideTable", table } },
                 dbRecords);
             var to = ServiceProvider.GetRequiredService<ITo>();
             await to.ProcessAsync(routable, CancellationToken.None);
             var dbContext = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
             var records = await QueryRecords<BasicRecord>(dbContext, table, new Dictionary<string, string>()
             {
                 { "Department", "HR" }
             });
             foreach (var record in records)
             {
                 var compare = dbRecords.Single(x => x.Identity == record.Identity);
                 Assert.Equal(compare, record);
             }
         }
     }
    
     [Fact]
     public async Task FullImplementationWorksAsExpected()
     {
         var thread = TestThread.CreateDeferred(60);
         var table = Guid.NewGuid().ToString();
         var dbRecord = new BasicRecord("This is a test", "HR");
         Component.Generic.Builder.Create()
             .WithFrom()
             .Build(ServiceCollection);
         Kyameru.Route.From("generic://test")
             .Process(x =>
             {
                 x.SetBody(dbRecord);
             })
             .To($"dynamoDB://{table}", x =>
             {
                 thread.Continue();
             })
             .Build(ServiceCollection);
         BuildServiceProvider();
         using (var localstackSession = Builder.Create(ServiceProvider)
                    .With(table, LocalstackService.DynamoDb, new Dictionary<string, string>()
                    {
                        { "HashKey", "Department" },
                        { "RangeKey", "Identity" }
                    }).Build())
         {
             await localstackSession.Init();
             var service = ServiceProvider.GetRequiredService<IHostedService>();
             thread.SetThread(service.StartAsync);
             thread.StartAndWait();
             await thread.CancelAsync();
             var dbContext = ServiceProvider.GetRequiredService<IAmazonDynamoDB>();
             var record = await GetRecord<BasicRecord>(dbContext, table, new Dictionary<string, string>()
             {
                 { "Department", dbRecord.HashKey },
                 { "Identity", dbRecord.Identity }
             });

             Assert.Equal(JsonSerializer.Serialize(dbRecord), JsonSerializer.Serialize(record));
         }
     }

    protected override IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<IDynamoDbUpserter, DynamoDbUpserter>();
        services.AddTransient<ITo, DynamoDbTo>();
        
        return services;
    }
}