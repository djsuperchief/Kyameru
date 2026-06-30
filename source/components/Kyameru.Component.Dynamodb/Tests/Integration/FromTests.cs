using System.Text.Json;
using Amazon.DynamoDBStreams;
using Amazon.DynamoDBv2;
using Kyameru.Component.Dynamodb.Contracts;
using Kyameru.Component.Dynamodb.IntegrationTests.DynamoDbRecords;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using Kyameru.TestUtilities.Enums;
using Kyameru.TestUtilities.Localstack;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Kyameru.Component.Dynamodb.IntegrationTests;

public class FromTests : BaseTests
{
    [Theory]
    [MemberData(nameof(DynamoDbData))]   
    public async Task FromCanGetSingleRecord(List<BasicRecord> dbRecords)
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
    }

    [Fact]
    public async Task FullFromRouteWorksAsIntended()
    {
        var thread = TestThread.CreateDeferred(30, 30);
        var tableName = Guid.NewGuid().ToString();
        var dbRecord = new BasicRecord("This is a test", "HR");
        var recordString = new List<string>();
        Component.Generic.Builder.Create().WithTo(x =>
        {
            recordString = x.Body as List<string>;
        }).Build(ServiceCollection);

        Kyameru.Route.From($"dynamodb://{tableName}?PollTime=5")
            .To("generic://test", x =>
            {
                thread.Continue();
            })
            .Build(ServiceCollection);
        BuildServiceProvider();
        using (var localstackSession = Builder.Create(ServiceProvider)
                   .With(tableName, LocalstackService.DynamoDb, new Dictionary<string, string>()
                   {
                       { "HashKey", "Department" },
                       { "RangeKey", "Identity" }
                   }).Build())
        {
            await localstackSession.Init();
            var service = ServiceProvider.GetRequiredService<IHostedService>();
            var dbUpserter = ServiceProvider.GetRequiredService<IDynamoDbUpserter>();
            thread.SetThread(service.StartAsync);
            thread.Start();
            await Task.Delay(TimeSpan.FromSeconds(10));
            await dbUpserter.SaveAsync(dbRecord, tableName, CancellationToken.None);

            thread.WaitForExecution();
            await thread.CancelAsync();
            Assert.Equal(dbRecord, JsonSerializer.Deserialize<BasicRecord>(recordString[0]));
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