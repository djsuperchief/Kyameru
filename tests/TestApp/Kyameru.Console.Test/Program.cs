using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon.S3;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

// need to update this
namespace Kyameru.Console.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string slackAddress = Environment.GetEnvironmentVariable("SlackAddress");
            await new HostBuilder().ConfigureServices((hostContext, services) =>
            {
                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();
                services.AddLogging();
                services.AddLocalStack(Configuration);
                services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
                services.AddAwsService<IAmazonS3>();

                Kyameru.Route.From("file:///home/giles/workspace/tmp?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    //.Process()
                    .To("s3://kyameru-component-s3/test&FileName=banana.txt")
                    .Id("AWS-S3-Test")
                    .BuildAsync(services);


            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
