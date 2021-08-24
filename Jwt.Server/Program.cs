using Jwt.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Jwt.Server
{
    public class Program
    {

        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Warning)
                .MinimumLevel.Override("IdentityServer", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                //.WriteTo.File(
                //    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logsJwtServer.log"),
                //    fileSizeLimitBytes: 1_000_000,
                //    rollOnFileSizeLimit: true,
                //    shared: true,
                //    flushToDiskInterval: TimeSpan.FromSeconds(1))
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Code)
                .WriteTo.Debug(outputTemplate: "{Level:u3} {SourceContext} {Message:lj} {NewLine}")
                .CreateLogger();

            try
            {
                var SeedAspUserDB = args.Contains("/seed");
                SeedAspUserDB = true; // uncomment to seed manually
                if (SeedAspUserDB)
                {
                    args = args.Except(new[] { "/seed" }).ToArray();
                }

                Log.Information("Jwt Server is starting up...");
                var host = CreateHostBuilder(args).Build();

                var config = host.Services.GetRequiredService<IConfiguration>();
                var connectionString = config.GetConnectionString("AspUsers");
                var DbPath = Path.Combine(AppContext.BaseDirectory, connectionString);// I now use 'AppContext.BaseDirectory' to create a full Db path, as without it Db Is4 Duende.db & AsUsers.db was created in '/var/www/synchub' dir & not where it shuold be in '/var/www/is4' !! 
                Console.WriteLine($"Jwt Server ===============  DbPath {DbPath}");

                if (SeedAspUserDB)
                {
                    Log.Information("Seeding database...");

                    SeedDb.EnsureSeedData(connectionString);
                    Log.Information("Done seeding database.");
                    // return 0;
                }

                Log.Information("Starting host...");
                host.Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly.");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                //.ConfigureAppConfiguration((hostingContext, config) =>
                //{
                //    config
                //        .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                //        .AddJsonFile("appall.json", optional: true)
                //        .AddJsonFile($"appall.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true);
                //})
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    #region Reverse Proxy
                    // If runninig in VS (Debug):
                    //      VS uses ApplicationUrl in launceSettings.json, eg localhost:port
                    //
                    // If deployed (Release):
                    //      UseUrls instructs Kestral to 'only' use:  AnyIP:11567 
                    //      We need to 
                    //      Then in sites Nginx file set up as a reverse proxy: proxy_pass http://localhost:11567;
                    //      'is4' nginx file is located at '/etc/nginx/sites-available'
                    //
                    // Note: If you deploy to a pi (or any server) and have set UseUrls(https://*:11567)
                    //          dont forget that Kestral is running! You can access the site (without nginx) 
                    //          by visiting  http://Pi_or_server_IP:11567  (only if port 11567 is open in ufw).
                    //          Thats why we DONT 'Allow' 11567 in the servers ufw. And DO setup nginx as reverse proxy,
                    //          ie we block kestral to outside world and instead point the outside world to nginx (server_name visygig.com)
                    //          which then points to kestral on 11567  (proxy_pass http://localhost:11567;)
                    #endregion

                    // If you are Running IS4 in VS then you need to comment out UseUrls (browser wont open otherwise)
                    // Set UseUrls in Release (ie when deployed to a server)
                    // If we set UseUrls then VS Debugging wont work, so only set UseUrls when in release (above).

#if RELEASE
                    webBuilder.UseUrls("http://*:11567"); // Browser wont open on local box when using UseUrls.
#endif

                    webBuilder.UseStartup<Startup>();
                });

    }
}
