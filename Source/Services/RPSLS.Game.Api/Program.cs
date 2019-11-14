using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RPSLS.Game.Api.Extensions;
using System;
using System.IO;
using System.Net;

namespace RPSLS.Game.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetStartupConfiguration();
            CreateHostBuilder(configuration,args).Build().Run();
        }

        private static IConfiguration GetStartupConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables();

            return builder.Build();
        }

        public static IHostBuilder CreateHostBuilder(IConfiguration configuration, string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        var (httpPort, grpcPort) = GetDefinedPorts(configuration);
                        options.Listen(IPAddress.Any, httpPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http1AndHttp2;
                        });
                        options.Listen(IPAddress.Any, grpcPort, listenOptions =>
                        {
                            listenOptions.Protocols = HttpProtocols.Http2;
                        });
                    })
                    .ConfigureAppConfiguration(cb => cb.AddFolder(configuration["KvMountFolder"] ?? "/kvmnt"))
                    .UseStartup<Startup>();
                });


        private static (int httpPort, int grpcPort) GetDefinedPorts(IConfiguration config)
        {
            var port = config.GetValue("PORT", -1);
            if (port == -1)
            {
                var aspnetcoreUris = config.GetValue("ASPNETCORE_URLS", "");
                if (!string.IsNullOrEmpty(aspnetcoreUris))
                {
                    if (Uri.TryCreate(aspnetcoreUris, UriKind.Absolute, out Uri uri))
                    {
                        port = uri.Port;
                    }
                }
            }

            if (port == -1) { port = 80; }

            var grpcPort = config.GetValue("GRPC_PORT", port+1);
            return (port, grpcPort);
        }
    }
}
