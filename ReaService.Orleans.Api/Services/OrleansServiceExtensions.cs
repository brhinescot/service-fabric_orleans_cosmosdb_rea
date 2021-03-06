﻿#region Using Directives

using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Clustering.ServiceFabric;
using Orleans.Configuration;
using Orleans.Graph;
using Orleans.Graph.Definition;
using Orleans.Graph.Test.Definition;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans.Api
{
    public static class OrleansServiceExtensions
    {
        public static IServiceCollection AddOrleans(this IServiceCollection services, string serviceId, string clusterId)
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
                    parts.AddApplicationPart(typeof(IPerson).Assembly);
                })
                .Build();

            client.Connect().GetAwaiter().GetResult();

            services.AddSingleton<IGrainFactory>(client);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped(provider =>
            {
                var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                var organizationClaim = context.User.FindFirst(claim => claim.Type == "Organization");

                var organizationId = organizationClaim == null ? "c94ed5bea7a74bffaf0cadcdac615908" : organizationClaim.Value;

                var grainFactory = provider.GetRequiredService<IGrainFactory>();
                return grainFactory.GetVertexGrain<IOrganization>(Guid.ParseExact(organizationId, "N"), "partition0");
            });

            return services;
        }
    }
}