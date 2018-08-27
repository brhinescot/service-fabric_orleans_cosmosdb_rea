#region Using Directives
// ReSharper disable ClassNeverInstantiated.Global

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

#endregion

namespace ReaService.Orleans.Api
{
    public class HealthCheckMiddleware
    {
        private readonly RequestDelegate next;
        private readonly string path;

        public HealthCheckMiddleware(RequestDelegate next, string path)
        {
            this.next = next;
            this.path = path;
        }

        [DebuggerStepThrough, UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Path.Equals(path, StringComparison.OrdinalIgnoreCase))
            {
                await next.Invoke(context);
            }
            else
            {
                context.Response.OnStarting(state =>
                {
                    var response = ((HttpContext)state).Response;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.Headers["Content-Type"] = new[] { "application/json" };
                    response.Headers["Cache-Control"] = new[] { "no-cache, no-store, must-revalidate" };
                    response.Headers["Pragma"] = new[] { "no-cache" };
                    response.Headers["Expires"] = new[] { "0" };
                    return Task.CompletedTask;
                }, context);
                
                // Some default values until fully implemented.
                await context.Response.WriteAsync(
                    JsonConvert.SerializeObject(new
                    {
                        Status = "Healthy",
                        CurrentBuild = "1.3.245",
                        AverageResponseTime = "22ms",
                        MaxResponseTime = "130ms",
                        ConnectedServices = new[]
                        {
                            new
                            {
                                Service = "REA",
                                Status = "Healthy",
                                AverageResponseTime = "20ms",
                                MaxResponseTime = "120ms",
                                HealthCheck = "https://rea.devinterop.com/health"
                            },
                            new
                            {
                                Service = "EventRecorder",
                                Status = "Healthy",
                                AverageResponseTime = "19ms",
                                MaxResponseTime = "112ms",
                                HealthCheck = "https://events.devinterop.com/health"
                            }
                        }
                    })
                );
            }
        }
    }
}