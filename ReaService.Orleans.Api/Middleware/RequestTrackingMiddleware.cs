#region Using Directives
// ReSharper disable ClassNeverInstantiated.Global

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;

#endregion

namespace ReaService.Orleans.Api.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestTrackingMiddleware
    {
        private readonly RequestDelegate next;

        public RequestTrackingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        [DebuggerStepThrough, UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            var tracker = Guid.NewGuid();
            context.Items.Add("RequestIdentifier", tracker);

            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext) state;
                httpContext.Response.Headers.Add("X-Request-Identifier", tracker.ToString());
                return Task.CompletedTask;
            }, context);

            await next.Invoke(context);
        }
    }
}