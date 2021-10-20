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
            await new HostBuilder().ConfigureServices((hostContext, services) =>
            {
                services.AddLogging();

                Kyameru.Route.From("file:///c:/Temp?Notifications=Created&SubDirectories=true&Filter=*.*")
                .Process(new ProcessingComp())
                .To("slack:///TH7DAGRQE/B01DWN0088P/f23IiCaHrx8NnThl7xUDiDSn?MessageSource=Body&Channel=general&Username=Kyameru")
                .Build(services);

            }).ConfigureLogging((hostContext, services) =>
            {
                services.AddConsole();
            }).RunConsoleAsync();


        }
    }
}
