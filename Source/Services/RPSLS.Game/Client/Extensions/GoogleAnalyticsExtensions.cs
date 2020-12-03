using Microsoft.Extensions.DependencyInjection;
using Blazor.Analytics;
using Blazor.Analytics.GoogleAnalytics;

namespace RPSLS.Game.Client.Extensions
{
    public static class GoogleAnalyticsExtensions
    {
        public static IServiceCollection AddGoogleAnalyticsAndInitialize(this IServiceCollection services, string trackingId)
        {
            return services.AddScoped<IAnalytics>(p =>
            {
                var googleAnalytics = ActivatorUtilities.CreateInstance<GoogleAnalyticsStrategy>(p);

                if (trackingId != null)
                {
                    googleAnalytics.Initialize(trackingId);
                }

                return googleAnalytics;
            });
        }
    }
}
