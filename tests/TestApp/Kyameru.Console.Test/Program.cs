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
                Kyameru.Route.From("file:///home/giles/workspace/tmp?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    .To("s3://kyameru-component-s3?Path=/test&FileName=banana.txt")
                    .BuildAsync(services);


            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
