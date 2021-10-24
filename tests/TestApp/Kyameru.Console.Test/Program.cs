using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

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

                Kyameru.Route.From("file:///c:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
                .Process(new ProcessingComp())
                //.To($"slack:///{slackAddress}?MessageSource=Body&Channel=general&Username=Kyameru")
                .To("ftp://kyameru:Password1.@192.168.1.249/&Source=Body")
                .Build(services);

            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
