using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static DeviceManager.Core.Utils.LogExtensions;

namespace DeviceManager.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var logLocation = config.GetSection("LogLocation").Value ?? "C:\\Logs\\DeviceManager.API\\log-.json";

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentUserName()
                .Enrich.WithClientIp()
                .Enrich.WithClientAgent()
                .Enrich.With(new ThreadIdEnricher())
                .Enrich.With(new CallerEnricher())
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
                .WriteTo.File(logLocation,
                       outputTemplate: "{NewLine} TIME: {Timestamp:HH:mm:ss} {NewLine} TYPE:{Level} {NewLine} MachineName- {MachineName} | Enviornment- {EnvironmentUserName} | Client- {ClientIp} - {ClientAgent} | SourceContext- {SourceContext} {NewLine} (Thread: {ThreadId}) MESSAGE:{Message} (at {Caller}) {NewLine} EXCEPTION: {Exception}",
                       rollingInterval: RollingInterval.Day,
                       shared: true)
                .WriteTo.Sentry(s =>
                {
                    s.SendDefaultPii = true;
                    s.TracesSampleRate = 1.0;
                    s.MinimumBreadcrumbLevel = LogEventLevel.Debug;
                    s.MinimumEventLevel = LogEventLevel.Error;
                })
                .CreateLogger();
            
            try
            {
                Log.Information("Application is starting");

                var host = CreateHostBuilder(args).Build();

                host.Run();

                Log.Information("Application Started");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args).UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseSentry();
                    webBuilder.UseIISIntegration();
                    webBuilder.UseContentRoot(Directory.GetCurrentDirectory());
                    webBuilder.UseStartup<Startup>();
                });
    }
}
