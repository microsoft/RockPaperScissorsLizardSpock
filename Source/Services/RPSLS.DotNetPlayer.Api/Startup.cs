using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RPSLS.DotNetPlayer.Api.Services;
using RPSLS.DotNetPlayer.Api.Settings;
using RPSLS.DotNetPlayer.API.Services;
using RPSLS.DotNetPlayer.API.Settings;
using RPSLS.DotNetPlayer.API.Strategies;
using System;
using System.Net.Http;

namespace RPSLS.DotNetPlayer.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<StrategySettings>(Configuration);
            services.Configure<PredictorSettings>(Configuration);
            services.AddControllers();
            services.AddHealthChecks();
            services.AddHttpClient("Predictor");

            services.AddSingleton<IPickStrategyFactory, PickStrategyFactory>();
            services.AddTransient<IPredictorProxyService, PredictorProxyService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/health");
        }
    }
}
