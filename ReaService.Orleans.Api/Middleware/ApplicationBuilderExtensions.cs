#region Using Directives

using Microsoft.AspNetCore.Builder;

#endregion

namespace ReaService.Orleans.Api.Middleware
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder app, string path = "/health")
        {
            return app.UseMiddleware<HealthCheckMiddleware>(path);
        }

        public static IApplicationBuilder UseRequestTimer(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ProcessingTimeMiddleware>();
        }

        public static IApplicationBuilder UseRequestTracking(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestTrackingMiddleware>();
        }
    }
}