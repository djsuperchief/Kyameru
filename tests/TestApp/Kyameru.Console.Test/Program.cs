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
                services.AddLogging();

                /*Kyameru.Route.From("file:///c:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
                .Process(new ProcessingComp())
                //.To($"slack:///{slackAddress}?MessageSource=Body&Channel=general&Username=Kyameru")
                .To("ftp://kyameru:Password1.@192.168.1.249/&Source=Body")
                .Build(services);*/

                /*Kyameru.Route.From("ftp://kyameru:Password1.@192.168.1.249/&PollTime=50000&Filter=50000&Delete=false")
                .To("file:///C:/Temp?Action=Write")
                .Id("FtpTest")
                .Build(services);*/

                Kyameru.Route.From("file:///c:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    .To($"slack:///{slackAddress}?MessageSource=Body&Channel=general&Username=Kyameru")
                    .BuildAsync(services);


            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
