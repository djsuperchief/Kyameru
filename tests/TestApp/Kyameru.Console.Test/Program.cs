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
                // // services.AddAwsService<IAmazonS3>(new AWSOptions
                // // {
                // //     DefaultClientConfig =
                // //     {
                // //         ServiceURL = "http://localhost:4566",
                // //         UseHttp = true
                // //     },
                // //     Region = RegionEndpoint.EUWest2,
                // //     Credentials = new BasicAWSCredentials("nope", "nope")
                // // });

                // services.AddTransient<IAmazonS3>(x =>
                // {
                //     var creds = new BasicAWSCredentials("ignore", "ignore");
                //     var config = new AmazonS3Config()
                //     {
                //         ServiceURL = $"http://localhost:4566",
                //         RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName("eu-west-2"),
                //         UseHttp = true,
                //         AuthenticationRegion = "eu-west-2"
                //     };

                //     config.ServiceURL = $"http://localhost:4566";

                //     return new AmazonS3Client(creds, config);

                // });
                #region "other test routes

                /*Kyameru.Route.From("file:///c:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
                .Process(new ProcessingComp())
                //.To($"slack:///{slackAddress}?MessageSource=Body&Channel=general&Username=Kyameru")
                .To("ftp://kyameru:Password1.@192.168.1.249/&Source=Body")
                .Build(services);*/

                /*Kyameru.Route.From("ftp://kyameru:Password1.@192.168.1.249/&PollTime=50000&Filter=50000&Delete=false")
                .To("file:///C:/Temp?Action=Write")
                .Id("FtpTest")
                .Build(services);*/

                /*Kyameru.Route.From("file:///c:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    .To($"slack:///{slackAddress}?MessageSource=Body&Channel=general&Username=Kyameru")
                    .BuildAsync(services);*/

                #endregion

                Kyameru.Route.From("file:///home/giles/workspace/tmp?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    .To("s3://kyameru-component-s3/test&FileName=banana.txt")
                    .BuildAsync(services);


            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
