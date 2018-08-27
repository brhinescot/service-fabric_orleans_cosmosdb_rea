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
        private const string CorrelationIdHeader = "X-Correlation-ID";
        private readonly RequestDelegate next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public RequestTrackingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [DebuggerStepThrough, UsedImplicitly]
        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers[CorrelationIdHeader];
            if(correlationId == string.Empty)
                correlationId = Guid.NewGuid().ToString("N");
            context.Items.Add("CorrelationId", correlationId);

            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext) state;
                httpContext.Response.Headers.Add(CorrelationIdHeader, correlationId);
                return Task.CompletedTask;
            }, context);

            await next.Invoke(context);
        }
    }
}