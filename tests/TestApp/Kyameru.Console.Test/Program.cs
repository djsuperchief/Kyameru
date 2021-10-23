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
                .To($"slack:///{slackAddress}?MessageSource=Body&Channel=general&Username=Kyameru")
                .Build(services);

            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
