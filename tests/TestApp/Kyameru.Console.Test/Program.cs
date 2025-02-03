using Amazon.S3;
using Amazon.SimpleEmail;
using Amazon.SimpleEmailV2;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Kyameru.Component.Ses;
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
                SetupSnsTest(services);
                SetupSesTest(services);
                SetupS3Route(services, fileLocation);


            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }

        static void SetupS3Route(IServiceCollection services, string fileLocation)
        {
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
        }

        static void SetupSnsTest(IServiceCollection services)
        {
            services.AddAwsService<IAmazonSQS>();
            services.AddAwsService<IAmazonSimpleNotificationService>();
            Kyameru.Route.From("sqs://kyameru-from")
            .To("sns://arn:aws:sns:eu-west-2:000000000000:kyameru_to")
            .Id("SNS_TEST")
            .Build(services);
        }

        static void SetupSesTest(IServiceCollection services)
        {
            services.TryAddAwsService<IAmazonSimpleEmailServiceV2>();
            services.TryAddAwsService<IAmazonSimpleEmailService>();
            services.TryAddAwsService<IAmazonSQS>();
            services.TryAddAwsService<IAmazonS3>();
            Kyameru.Route.From("sqs://kyameru-from")
            .Process((Routable x) =>
            {
                if (x.Body.ToString().Contains("dowhen"))
                {
                    x.SetHeader("export", "true");
                    x.SetHeader("S3DataType", "String");
                }

                x.SetBody<SesMessage>(new SesMessage()
                {
                    BodyText = "This is the body",
                    Subject = "Test"
                });
                x.SetHeader("SESTo", "test@test.com");
            })
            .When(x => x.Headers.TryGetValue("export", "false") == "true", "s3://kyameru-component-s3/test&FileName=conditional.txt")
            .To("ses:///?from=kyameru@kyameru.com")
            .Id("sestest")
            .Build(services);
        }
    }
}
