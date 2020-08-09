using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace DotnetTemplateDemo.Api
{
    public class Program
    {
        private static IConfiguration _configuration;
        public static void Main(string[] args)
        {
            try
            {
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                var platform = env.Equals("Development") ? 
                    (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? 
                        ".Windows" : ".LinuxOsx") 
                    : "";
                _configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile(
                        $"appsettings.{env ?? "Production"}{platform}.json",
                        optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables()
                    .AddCommandLine(args)
                    .Build();

                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(_configuration)
                    .CreateLogger()
                    .ForContext<Program>();

                Log.Information("Starting web host");

                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception exception)
            {
                Log.Fatal(exception, "Site terminated");
            }
            finally
            {
                Log.Information("Ending web host");

                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseConfiguration(_configuration)
                        .UseSerilog()
                        .UseStartup<Startup>();
                });
    }
}
