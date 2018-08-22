#region Using Directives

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using Microsoft.Extensions.Logging;
using Orleans.Graph.Definition;
using Orleans.Graph.Query;
using Orleans.Graph.Vertex;
using Orleans.Runtime;
using CosmosDbVertex = Microsoft.Azure.Graphs.Elements.Vertex;

#endregion

namespace Orleans.Graph.StorageProvider
{
    internal class VertexProvider : GraphElementProvider
    {
        #region Member Fields

        private readonly DocumentClient client;
        private readonly DocumentCollection graph;
        private readonly IGrainReferenceConverter grainReferenceConverter;
        private readonly ILogger log;
        private readonly IGrainFactory grainFactory;
        private readonly string serviceId;

        #endregion

        internal VertexProvider(DocumentClient client, DocumentCollection graph, IGrainReferenceConverter grainReferenceConverter, ILogger log, IGrainFactory grainFactory, string serviceId)
        {
            this.client = client;
            this.graph = graph;
            this.grainReferenceConverter = grainReferenceConverter;
            this.log = log;
            this.grainFactory = grainFactory;
            this.serviceId = serviceId;
        }

        internal async Task ReadVertexStateAsync(GrainReference grainReference, VertexState vertexState)
        {
            var readExpression = $"g.V('{grainReference.ToKeyString()}')";
            var graphElementGrain = grainReference.AsReference<IGraphElementGrain>();
                
            var feedOptions = new FeedOptions
            {
                MaxItemCount = 1,
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };
            
            var readQuery = client.CreateGremlinQuery<CosmosDbVertex>(graph, readExpression, feedOptions, GraphSONMode.Normal);
            
            var response = await readQuery.ExecuteNextAsync<CosmosDbVertex>();
            log.Info($"CosmosDB: Read Vertex State: Request Charge: {response.RequestCharge}");
            
            var vertex = response.FirstOrDefault();
            if (vertex == null)
                return;

            vertexState.Persisted = true;
            
            foreach (var edge in vertex.GetInEdges())
            {
                var edgeReference = grainReferenceConverter.GetGrainFromKeyString(edge.Id.ToString());
                edgeReference.BindGrainReference(grainFactory);

                var vertexReference = grainReferenceConverter.GetGrainFromKeyString(edge.InVertexId.ToString());
                vertexReference.BindGrainReference(grainFactory);

                vertexState.AddInEdge(edgeReference.AsReference<IEdge>(), vertexReference.AsReference<IVertex>());
            }

            foreach (var edge in vertex.GetOutEdges())
            {
                var edgeReference = grainReferenceConverter.GetGrainFromKeyString(edge.Id.ToString());
                edgeReference.BindGrainReference(grainFactory);
                
                var vertexReference = grainReferenceConverter.GetGrainFromKeyString(edge.InVertexId.ToString());
                vertexReference.BindGrainReference(grainFactory);

                vertexState.AddOutEdge(edgeReference.AsReference<IEdge>(), vertexReference.AsReference<IVertex>());
            }

            foreach (var property in vertex.GetVertexProperties())
            { 
                if (property.Key[0] == '@' || property.Key == "partition")
                    continue;

                var vertexProperty = vertexState.SetProperty(property.Key, property.Value.ToString());

                try
                {
                    foreach (var subProperty in property.GetProperties())
                    {
                        if (subProperty.Key[0] == '@')
                            continue;

                        vertexProperty.SetMeta(subProperty.Key, subProperty.Value.ToString());
                    }
                }
                // BUG: The Microsoft.Azure.Graphs library throws an exception when enumerating over empty properties. How do we check or work around this?
                catch (NullReferenceException) { }
            }
        }

        internal async Task WriteVertexStateAsync(GrainReference grainReference, VertexState vertexState)
        {
            var graphElementGrain = grainReference.AsReference<IGraphElementGrain>();

            var writeExpression = (vertexState.Persisted ? 
                CreateUpdateExpression(grainReference, vertexState, graphElementGrain) : 
                CreateInsertExpression(grainReference, vertexState, graphElementGrain))
                .ToString();
            
            var feedOptions = new FeedOptions
            {
                MaxItemCount = 1,
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            var writeQuery = client.CreateGremlinQuery<CosmosDbVertex>(graph, writeExpression, feedOptions, GraphSONMode.Normal);
            
            log.Info($"CosmosDB: Writing VertexState for grain Id '{graphElementGrain.ToKeyString()}'");
            var response = await writeQuery.ExecuteNextAsync<CosmosDbVertex>();
            log.Info($"CosmosDB: Writing VertexState complete: Request Charge: {response.RequestCharge}");

            vertexState.Persisted = true;
        }

        internal async Task ClearVertexStateAsync(GrainReference grainReference, VertexState vertexState)
        {
            var graphElementGrain = grainReference.AsReference<IGraphElementGrain>();
            
            var feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            var dropCommand = $"g.V('{graphElementGrain.ToKeyString()}')";
            var writeQuery = client.CreateGremlinQuery<CosmosDbVertex>(graph, dropCommand, feedOptions);
            var response = await writeQuery.ExecuteNextAsync<CosmosDbVertex>();
            vertexState.Persisted = false;
            
            log.Info($"CosmosDB: Drop Vertex State: Request Charge: {response.RequestCharge}");
        }

        private static IVertexResult CreateUpdateExpression(GrainReference grainReference, VertexState vertexState, IGraphElementGrain grainWithGraphElement)
        {
            return g.V().has(grainWithGraphElement.GetGraphLabel(), "id", grainReference.ToKeyString()).property(vertexState);
        }

        private static IVertexResult CreateInsertExpression(GrainReference grainReference, VertexState vertexState, IGraphElementGrain graphElementGrain)
        {
            var insertExpression = g.AddV(graphElementGrain.GetGraphLabel())
                .property("id", grainReference.ToKeyString());

            var partition = graphElementGrain.GetGraphPartition();
            if (!string.IsNullOrEmpty(partition))
                insertExpression= insertExpression.property("partition", partition);
            
            return insertExpression.property(vertexState);
        }
    }
}