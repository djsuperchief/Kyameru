using System.Text.Json;
using Amazon.DynamoDBStreams;
using Kyameru.Component.DynamoDB.Contracts;
using Kyameru.Component.DynamoDB.IntegrationTests.DynamoDbRecords;
using Kyameru.Core.Entities;
using Kyameru.TestUtilities;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Kyameru.Component.DynamoDB.IntegrationTests;

public class FromTests : BaseTests
{
    [Fact]
    public async Task FromCanGetSingleRecord()
    {
        BuildServiceProvider();
        var routable = new Routable(new Dictionary<string, string>(), "Default");
        var table = await CreateDynamoDbTable("Department", "Identity");
        var dbRecords = new List<BasicRecord>()
        {
            new("This is a test", "HR")
        };
        
        var dbUpserter = ServiceProvider.GetRequiredService<IDynamoDbUpserter>();
        await dbUpserter.SaveAsync(dbRecords, table, CancellationToken.None);
        var worker = TestThread.CreateDeferred(60, 60);
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
        worker.StartAndWait();
        await from.StopAsync(worker.CancelToken);
        await worker.CancelAsync();
        
        Assert.Equal(JsonSerializer.Serialize(dbRecords), routable.Body);
    }

    protected override IServiceCollection AddServices(IServiceCollection services)
    {
        services.AddTransient<IDynamoDbUpserter, DynamoDbUpserter>();
        services.TryAddAwsService<IAmazonDynamoDBStreams>();
        services.AddTransient<IFrom, DynamoDbFrom>();
        return services;
    }
}