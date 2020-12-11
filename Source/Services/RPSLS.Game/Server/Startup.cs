using GameApi.Proto;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RPSLS.Game.Server.Config;
using RPSLS.Game.Server.Grpc.Interceptors;
using RPSLS.Game.Server.Grpc.Services;
using System;

namespace RPSLS.Game.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddAuthentication(Configuration);
            services.AddApplicationInsightsTelemetry();
            services.AddHealthChecks();
            services.AddHttpContextAccessor();

            services.AddOptions()
                .Configure<GoogleAnalyticsSettings>(Configuration)
                .Configure<RecognitionSettings>(Configuration)
                .Configure<TwitterSettings>(x => x.IsLoginEnabled = !string.IsNullOrWhiteSpace(Configuration["Authentication:Twitter:ConsumerKey"]) && !string.IsNullOrWhiteSpace(Configuration["Authentication:Twitter:ConsumerSecret"]))
                .Configure<GameManagerSettings>(Configuration.GetSection("GameManager"))
                .ConfigureOptions<MultiplayerSettingsOptions>();

            if (Configuration.GetValue<bool>("GameManager:Grpc:GrpcOverHttp", false))
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            }

            services.AddSingleton<AzdsMetadataInterceptor>();

            services.AddGrpcClient<BotGameManager.BotGameManagerClient>((services, options) =>
            {
                var gameManagerUrl = services.GetService<IOptions<GameManagerSettings>>().Value.Url;
                options.Address = new Uri(gameManagerUrl);
            }).AddInterceptor<AzdsMetadataInterceptor>();

            services.AddGrpcClient<ConfigurationManager.ConfigurationManagerClient>((services, options) =>
            {
                var gameManagerUrl = services.GetService<IOptions<GameManagerSettings>>().Value.Url;
                options.Address = new Uri(gameManagerUrl);
            });
            
            services.AddGrpcClient<MultiplayerGameManager.MultiplayerGameManagerClient>((services, options) =>
            {
                var gameManagerUrl = services.GetService<IOptions<GameManagerSettings>>().Value.Url;
                options.Address = new Uri(gameManagerUrl);
            }).AddInterceptor<AzdsMetadataInterceptor>();

            services.AddGrpc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseHealthChecks("/health");
            app.UseGrpcWeb();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGrpcService<BotGameManagerService>().EnableGrpcWeb();
                endpoints.MapGrpcService<GameSettingsManagerService>().EnableGrpcWeb();
                endpoints.MapGrpcService<MultiplayerGameManagerService>().EnableGrpcWeb();
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
