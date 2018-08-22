using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Graph;
using Orleans.Runtime;

namespace ReaService.Orleans.Host
{
    public class StartupTask : IStartupTask
    {
        private readonly IGrainFactory grainFactory;
        private readonly ILogger<StartupTask> logger;

        public StartupTask(IGrainFactory grainFactory, ILogger<StartupTask> logger)
        {
            this.grainFactory = grainFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Placeholder for now.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Execute(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}