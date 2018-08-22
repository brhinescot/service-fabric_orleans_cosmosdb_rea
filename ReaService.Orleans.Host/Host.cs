#region Using Directives

using System.Collections.Generic;
using System.Fabric;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Orleans;
using Orleans.Clustering.ServiceFabric;
using Orleans.Configuration;
using Orleans.Graph;
using Orleans.Graph.Definition;
using Orleans.Graph.StorageProvider;
using Orleans.Graph.Test;
using Orleans.Graph.Test.Definition;
using Orleans.Hosting;
using Orleans.Hosting.ServiceFabric;
using Orleans.Runtime;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans.Host
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Host : StatelessService
    {
        public Host(StatelessServiceContext context) : base(context){ }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            // Listeners can be opened and closed multiple times over the lifetime of a service instance.
            // A new Orleans silo will be both created and initialized each time the listener is opened and will be shutdown 
            // when the listener is closed.
            var orleansListener = OrleansServiceListener.CreateStateless(
                (fabricServiceContext, siloBuilder) =>
                {
                    siloBuilder.Configure<ClusterOptions>(options =>
                    {
                        // The service id is unique for the entire service over its lifetime. This is used to identify persistent state
                        // such as reminders and grain state.
                        options.ServiceId = fabricServiceContext.ServiceName.ToString();

                        // The cluster id identifies a deployed cluster. Since Service Fabric uses rolling upgrades, the cluster id
                        // can be kept constant. This is used to identify which silos belong to a particular cluster.
                        options.ClusterId = "development";
                    });

                    // Configure clustering to use Service Fabric membership.
                    siloBuilder.UseServiceFabricClustering(fabricServiceContext);
                    siloBuilder.ConfigureLogging(logging => logging.AddDebug());
                    siloBuilder.AddStartupTask<StartupTask>();
                    
                    // Service Fabric manages port allocations, so update the configuration using those ports.
                    // Gather configuration from Service Fabric.
                    var activation = fabricServiceContext.CodePackageActivationContext;

                    var endpoints = activation.GetEndpoints();
                    var siloEndpoint = endpoints["OrleansSiloEndpoint"];
                    var gatewayEndpoint = endpoints["OrleansProxyEndpoint"];
                    var hostname = fabricServiceContext.NodeContext.IPAddressOrFQDN;
                    siloBuilder.ConfigureEndpoints(hostname, siloEndpoint.Port, gatewayEndpoint.Port);

                    var configurationSection = activation.GetConfigurationPackageObject("Config").Settings.Sections["CosmosDbConfig"];
                    var configParameters = configurationSection.Parameters;

                    siloBuilder.AddCosmosDbGraphGrainStorage("CosmosDBGraph", options =>
                    {
                        options.Endpoint = configParameters["Endpoint"].Value;
                        options.AuthKey = configParameters["AuthKey"].Value;
                        options.Database = configParameters["Database"].Value;
                        options.Collection = configParameters["Collection"].Value;
                    });

                    // Add the application assemblies.
                    siloBuilder.ConfigureApplicationParts(parts =>
                    {
                        parts.AddApplicationPart(typeof(IAgent).Assembly);
                        parts.AddApplicationPart(typeof(AgentGrain).Assembly);
                        parts.AddApplicationPart(typeof(IVertex).Assembly);
                        parts.AddApplicationPart(typeof(VertexGrain).Assembly);
                        parts.AddApplicationPart(typeof(IPersonVertex).Assembly);
                        parts.AddApplicationPart(typeof(PersonVertex).Assembly);
                    });

                    siloBuilder.UseSiloUnobservedExceptionsHandler();
                });

            return new[] { orleansListener };
        }

//        /// <summary>
//        /// This is the main entry point for your service instance.
//        /// </summary>
//        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
//        protected override async Task RunAsync(CancellationToken cancellationToken)
//        {
//            while (true)
//            {
//                cancellationToken.ThrowIfCancellationRequested();
//
//                ServiceEventSource.Current.ServiceMessage(this.Context, "Working");
//
//                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
//            }
//        }
    }
}
