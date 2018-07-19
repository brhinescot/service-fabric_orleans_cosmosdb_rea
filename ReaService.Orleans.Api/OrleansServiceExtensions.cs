#region Using Directives

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Clustering.ServiceFabric;
using Orleans.Configuration;
using Orleans.Graph.Definition;
using Orleans.Graph.Test.Definition;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans.Api {
    public static class OrleansServiceExtensions
    {
        public static void AddOrleansClient(this IServiceCollection services, string serviceId, string clusterId)
        {
            Uri serviceUri = new Uri(serviceId);

            IClusterClient client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = serviceUri.ToString();
                    options.ClusterId = clusterId;
                })
                .UseServiceFabricClustering(serviceUri)
                .ConfigureLogging(logging => logging.AddDebug())
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(IAgentGrain).Assembly);
                    parts.AddApplicationPart(typeof(IVertexGrain).Assembly);
                    parts.AddApplicationPart(typeof(IPersonVertex).Assembly);
                })
                .Build();

            client.Connect().GetAwaiter().GetResult();

            services.AddSingleton(client);
        }
    }
}