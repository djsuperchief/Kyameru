﻿using Amazon.S3;
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

                Kyameru.Route.From($"file://{fileLocation}?Notifications=Created&SubDirectories=true&Filter=*.*")
                    .Process(new ProcessingComp())
                    .Process((Routable x) =>
                    {
                        var byteString = Encoding.UTF8.GetBytes("Hello World");

                        x.SetHeader("S3FileName", x.Headers["SourceFile"]);
                        x.SetHeader("S3DataType", "Byte");
                        x.SetBody<byte[]>(byteString);
                    })
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
