using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.Reflection;

namespace Jwt.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

            var sessionId = Guid.NewGuid();
            ////GetLocalAppDataPath();
            //var LogPath = LocalAppDataPath + @"agy_wpf.log";
            //LogEventLevel level = LogEventLevel.Debug;
            //var tmpl = "[{Timestamp:HH:mm:ss} {Level:u3}{SourceContext}]  {Message:lj}{NewLine}{Exception}";
            var tmpl = "[{Timestamp:dd-MM-yyyy  HH:mm:ss} {Level:u3}{SourceContext}]  {Message:lj}{NewLine}{Exception}";
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                //.Enrich.WithCaller() // this didnt work & do we need it as exceptions have caller & line numbers anyhow.
                .Enrich.WithProperty("SessionId", sessionId)
                .Enrich.With(new SimpleClassEnricher()) // shortens the SourceContext ie: Agy.Wpf.Services.Duende to Duende
                                                        //.WriteTo.File(
                                                        //    LogPath,
                                                        //    fileSizeLimitBytes: 1_000_000,
                                                        //    outputTemplate: tmpl,
                                                        //    rollOnFileSizeLimit: true,
                                                        //    shared: true,
                                                        //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                                                        //.WriteTo.Seq("http://localhost:5341")
                                                        //.WriteTo.Udp("localhost", 9999, AddressFamily.InterNetwork, new Log4jTextFormatter()) // NLog
                                                        //.WriteTo.Udp("localhost", 9998, AddressFamily.InterNetwork, new Log4netTextFormatter()) // Log4Net
                .WriteTo.Console(outputTemplate: tmpl, theme: AnsiConsoleTheme.Code)
                //.WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                //.WriteTo.Debug(outputTemplate: AppId + " {Level:u3} {SourceContext} {Message:lj} {Exception}{NewLine}")
                .WriteTo.Debug(outputTemplate: "{Level:u3} {SourceContext} {Message:lj} {NewLine}")
               .CreateLogger();


            var host = CreateHostBuilder(args).Build();

            // this section sets up and seeds the database. It would NOT normally
            // be done this way in production. It is here to make the sample easier,
            // i.e. clone, set connection string and run.
            var sp = host.Services.GetService<IServiceScopeFactory>()
                .CreateScope()
                .ServiceProvider;
           
            //var options = sp.GetRequiredService<DbContextOptions<AgyDB>>();

            //await InitDB(options);// Agy.Blazor has its own DB for local data, ie users data saved on their machine.


            // back to your regularly scheduled program
            await host.RunAsync();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config
                        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                        .AddJsonFile("app_settings.json", optional: false, reloadOnChange: true);
                    //.AddJsonFile($"app_settings.local.json", optional: true);
                    //.AddJsonFile($"all.Development.json", optional: true, reloadOnChange: true);
                    //AddJsonFile($"all.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });


    }
}
