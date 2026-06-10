using System.Text.Json;
using Amazon.DynamoDBStreams;
using Amazon.DynamoDBv2;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.IntegrationTests.DynamoDbRecords;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.DynamoDB.IntegrationTests;

public class FromTests : BaseTests
{
    [Theory]
    [MemberData(nameof(DynamoDbData))]   
    public async Task FromCanGetSingleRecord(List<BasicRecord> dbRecords)
    {
        BuildServiceProvider();
        var table = await CreateDynamoDbTable("Department", "Identity");
        try
        {
            var routable = new Routable(new Dictionary<string, string>(), "Default");
            var worker = TestThread.CreateDeferred(30, 30);
            var from = ServiceProvider.GetRequiredService<IFrom>();
            from.SetHeaders(new Dictionary<string, string>()
            {
                { "Host", table },
                { "PollTime", "5" }
            });
            from.Setup();
            from.OnActionAsync += async (sender, args) =>
            {
                routable = args.Data;
                worker.Continue();
                await Task.CompletedTask;
            };
            worker.SetThread(from.StartAsync);
            worker.Start();
            
            await Task.Delay(5000, worker.CancelToken);
            var dbUpserter = ServiceProvider.GetRequiredService<IDynamoDbUpserter>();
            await dbUpserter.SaveAsync(dbRecords, table, CancellationToken.None);
            worker.WaitForExecution();
            await from.StopAsync(worker.CancelToken);
            await worker.CancelAsync();
            
            var received = new List<BasicRecord>();
            foreach (var record in routable.Body as List<string>)
            {
                received.Add(JsonSerializer.Deserialize<BasicRecord>(record));
            }
            
            Assert.Equal(dbRecords, received);
        }
        finally
        {
            await DeleteDynamoDbTable(table);
        }
    }

    public static IEnumerable<object[]> DynamoDbData()
    {
        yield return new object[] { new List<BasicRecord>() {new ("This is a test", "HR")} };
        yield return new object[] {new List<BasicRecord>() {new ("This is a test", "HR"),  new ("This is another test", "HR")} };
    }

    protected override IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<IDynamoDbUpserter, DynamoDbUpserter>();
        services.TryAddAwsService<IAmazonDynamoDBStreams>();
        services.TryAddAwsService<IAmazonDynamoDB>();
        services.AddTransient<IFrom, DynamoDbFrom>();
        return services;
    }
}