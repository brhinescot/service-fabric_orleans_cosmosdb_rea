#region Using Directives

using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans.Configuration;
using Orleans.Graph.Edge;
using Orleans.Graph.Vertex;
using Orleans.Runtime;
using Orleans.Storage;

#endregion

namespace Orleans.Graph.StorageProvider
{
    [UsedImplicitly]
    public class CosmosDbGraphStorage : IGrainStorage, ILifecycleParticipant<ISiloLifecycle>
    {
        #region Member Fields

        private readonly CosmosDbGraphStorageOptions options;
        private readonly IOptions<ClusterOptions> clusterOptions;
        private readonly IGrainFactory grainFactory;
        private readonly IGrainReferenceConverter grainReferenceConverter;
        private readonly ILogger log;
        private readonly string name;
        private readonly string serviceId;

        private EdgeProvider edgeProvider;
        private VertexProvider vertexProvider;
        private DocumentClient client;
        private DocumentCollection graph;

        #endregion

        #region .ctor

        /// <summary>
        ///     Initializes a new instance of the <see cref="CosmosDbGraphStorage"/> class.
        /// </summary>
        /// <param name="name">Name assigned for this provider</param>
        /// <param name="options"></param>
        /// <param name="clusterOptions"></param>
        /// <param name="grainFactory"></param>
        /// <param name="grainReferenceConverter"></param>
        /// <param name="loggerFactory"></param>
        public CosmosDbGraphStorage(string name, 
            CosmosDbGraphStorageOptions options, 
            IOptions<ClusterOptions> clusterOptions,
            IGrainFactory grainFactory,
            IGrainReferenceConverter grainReferenceConverter,
            ILoggerFactory loggerFactory)
        {
            this.name = name;
            this.options = options;
            this.clusterOptions = clusterOptions;
            this.grainFactory = grainFactory;
            this.grainReferenceConverter = grainReferenceConverter;

            var loggerName = $"{typeof(CosmosDbGraphStorage).FullName}.{name}";
            log = loggerFactory.CreateLogger(loggerName);
            serviceId = clusterOptions.Value.ServiceId;
        }

        #endregion

        #region IGrainStorage Members
        
        /// <summary>Read data function for this storage provider instance.</summary>
        /// <param name="grainType">Type of this grain [fully qualified class name]</param>
        /// <param name="grainReference">Grain reference object for this grain.</param>
        /// <param name="grainState">State data object to be populated for this grain.</param>
        /// <returns>Completion promise for the Read operation on the specified grain.</returns>
        public async Task ReadStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            switch (grainState.State)
            {
                case VertexState vertexState:
                    await vertexProvider.ReadVertexStateAsync(grainReference, vertexState);
                    log.Info($"Completed read of vertex grain state for grain {grainReference.ToKeyString()}.");
                    break;
                case EdgeState edgeState:
                    await edgeProvider.ReadEdgeStateAsync(grainReference, edgeState);
                    log.Info($"Completed read of edge grain state for grain {grainReference.ToKeyString()}.");
                    break;
            }
        }

        /// <summary>Write data function for this storage provider instance.</summary>
        /// <param name="grainType">Type of this grain [fully qualified class name]</param>
        /// <param name="grainReference">Grain reference object for this grain.</param>
        /// <param name="grainState">State data object to be written for this grain.</param>
        /// <returns>Completion promise for the Write operation on the specified grain.</returns>
        public async Task WriteStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            switch (grainState.State)
            {
                case VertexState vertexState:
                    log.Info($"CosmosDb: Writing vertex grain state for grain type {grainType}.");
                    await vertexProvider.WriteVertexStateAsync(grainReference, vertexState);
                    break;
                case EdgeState edgeState:
                    log.Info($"CosmosDb: Writing edge grain state for grain type {grainType}.");
                    await edgeProvider.WriteEdgeStateAsync(grainReference, edgeState);
                    break;
            }
        }

        /// <summary>Delete / Clear data function for this storage provider instance.</summary>
        /// <param name="grainType">Type of this grain [fully qualified class name]</param>
        /// <param name="grainReference">Grain reference object for this grain.</param>
        /// <param name="grainState">Copy of last-known state data object for this grain.</param>
        /// <returns>Completion promise for the Delete operation on the specified grain.</returns>
        public async Task ClearStateAsync(string grainType, GrainReference grainReference, IGrainState grainState)
        {
            switch (grainState.State)
            {
                case VertexState vertexState:
                    log.Info($"Clearing vertex grain state for grain type {grainType}.");
                    await vertexProvider.ClearVertexStateAsync(grainReference, vertexState);
                    break;
                case EdgeState edgeState:
                    log.Info($"Clearing edge grain state for grain type {grainType}.");
                    await edgeProvider.ClearEdgeStateAsync(grainReference, edgeState);
                    break;
            }
        }

        #endregion

        public void Participate(ISiloLifecycle lifecycle)
        {
            lifecycle.Subscribe(OptionFormattingUtilities.Name<CosmosDbGraphStorage>(name), options.InitStage, Init, Close);
        }

        private async Task Init(CancellationToken ct)
        {
            try
            {
                client = new DocumentClient(new Uri(options.Endpoint), options.AuthKey, new ConnectionPolicy
                {
                    EnableEndpointDiscovery = false
                });

                graph = await client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(options.Database, options.Collection));
                graph.PartitionKey.Paths.Add("/partition");

                vertexProvider = new VertexProvider(client, graph, grainReferenceConverter, log, grainFactory, serviceId);
                edgeProvider = new EdgeProvider(client, graph, grainReferenceConverter, log, grainFactory, serviceId);

                log.Info($"{nameof(CosmosDbGraphStorage)} named '{name}' initialized for service id {clusterOptions.Value.ServiceId}.");
            }
            catch (UriFormatException)
            {
                throw;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (DocumentClientException)
            {
                throw;
            }
        }

        /// <summary>Close function for this provider instance.</summary>
        /// <returns>Completion promise for the Close operation on this provider.</returns>
        public Task Close(CancellationToken ct)
        {
            client?.Dispose();
            return Task.CompletedTask;
        }
    }
}