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

namespace ReaService.Orleans.Api
{
    public static class OrleansServiceExtensions
    {
        public static void AddOrleansClient(this IServiceCollection services, string serviceId, string clusterId)
        {
            var serviceUri = new Uri(serviceId);

            var client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = serviceUri.ToString();
                    options.ClusterId = clusterId;
                })
                .UseServiceFabricClustering(serviceUri)
                .ConfigureLogging(logging => logging.AddDebug().AddEventSourceLogger())
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(IAgent).Assembly);
                    parts.AddApplicationPart(typeof(IVertex).Assembly);
                    parts.AddApplicationPart(typeof(IPersonVertex).Assembly);
                })
                .Build();

            client.Connect().Wait();
            services.AddSingleton(client);
        }
    }
}