using Amazon.S3;
using Amazon.SQS;
using Kyameru.Core.Entities;
using LocalStack.Client.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

// need to update this
namespace Kyameru.Console.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var slackAddress = Environment.GetEnvironmentVariable("SlackAddress");
            string fileLocation;
            await new HostBuilder().ConfigureServices((hostContext, services) =>
            {


                IConfiguration Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();



                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    fileLocation = Configuration["FileAddressWin"];
                }
                else
                {
                    fileLocation = Configuration["FileAddressLinux"];
                }
                services.AddLogging();
                services.AddLocalStack(Configuration);
                services.AddDefaultAWSOptions(Configuration.GetAWSOptions());
                services.AddAwsService<IAmazonS3>();
                services.AddAwsService<IAmazonSQS>();

                Kyameru.Route.From($"file://{fileLocation}?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    .Process((Routable x) =>
                    {
                        x.SetHeader("S3DataType", "String");
                    })
                    .To("s3://kyameru-component-s3/test&FileName=banana.txt", (Routable x) =>
                    {
                        x.SetBody<string>($"File uploaded to S3 bucket '{x.Headers["S3Bucket"]}' with key: {x.Headers["S3Key"]}");
                    })
                    .To("sqs://kyameru-to")
                    .Id("AWS-S3-Test")
                    .Build(services);

                // Kyameru.Route.From("sqs://localhost:4566/000000000000/kyameru-from?PollTime=10")
                // .To("sqs://kyameru-to")
                // .Id("sqs-full-test")
                // .Build(services);


            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
