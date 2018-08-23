#region Using Directives
// ReSharper disable ClassNeverInstantiated.Global

using System.Diagnostics;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

#endregion

namespace ReaService.Orleans.Api.Middleware
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessingTimeMiddleware
    {
        private readonly RequestDelegate next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ProcessingTimeMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        [DebuggerStepThrough, UsedImplicitly]
        public Task Invoke(HttpContext context)
        {
            var watch = new Stopwatch();
            watch.Start();

            context.Response.OnStarting(state =>
            {
                var response = ((HttpContext) state).Response;

                watch.Stop();
                response.Headers.Add("X-Processing-Time", watch.ElapsedMilliseconds.ToString());

                return Task.CompletedTask;
            }, context);

            return next.Invoke(context);
        }
    }
}