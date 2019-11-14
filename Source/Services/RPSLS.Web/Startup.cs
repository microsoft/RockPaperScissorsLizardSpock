using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPSLS.Web.Clients;
using RPSLS.Web.Config;
using RPSLS.Web.Helpers;
using RPSLS.Web.Services;
using System;

namespace RPSLS.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddHealthChecks();
            services.AddRazorPages();
            services.AddServerSideBlazor(opt =>
            {
                opt.DetailedErrors = true;
            });
            services.AddHttpClient();
            services.AddAuthentication(Configuration);

            services.AddOptions();
            services.Configure<RecognitionSettings>(Configuration);
            services.Configure<GoogleAnalyticsSettings>(Configuration);
            services.Configure<GameManagerSettings>(Configuration.GetSection("GameManager"));
            if (Configuration.GetValue<bool>("GameManager:Grpc:GrpcOverHttp", false))
            {
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
                AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2Support", true);
            }

            services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
            services.AddScoped<IGameManagerClient, GameManagerClient>();
            services.AddScoped<IGameService, GameService>();
            services.AddScoped<SvgHelper>();
            services.AddScoped<BattleHelper>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseHealthChecks("/health");
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
