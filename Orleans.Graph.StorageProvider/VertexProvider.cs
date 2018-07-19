﻿#region Using Directives

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
using AzureVertex = Microsoft.Azure.Graphs.Elements.Vertex;

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
            string readExpression = $"g.V('{grainReference.ToKeyString()}')";
            IGraphElementGrain graphElementGrain = grainReference.AsReference<IGraphElementGrain>();
                
            FeedOptions feedOptions = new FeedOptions
            {
                MaxItemCount = 1,
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };
            
            var readQuery = client.CreateGremlinQuery<AzureVertex>(graph, readExpression, feedOptions, GraphSONMode.Normal);
            
            var response = await readQuery.ExecuteNextAsync<AzureVertex>();
            log.Info($"CosmosDB: Read Vertex State: Request Charge: {response.RequestCharge}");
            
            AzureVertex vertex = response.FirstOrDefault();
            if (vertex == null)
                return;

            vertexState.Persisted = true;
            
            foreach (var edge in vertex.GetInEdges())
            {
                GrainReference edgeReference = grainReferenceConverter.GetGrainFromKeyString(edge.Id.ToString());
                edgeReference.BindGrainReference(grainFactory);

                GrainReference vertexReference = grainReferenceConverter.GetGrainFromKeyString(edge.InVertexId.ToString());
                vertexReference.BindGrainReference(grainFactory);

                vertexState.AddInEdge(edgeReference.AsReference<IEdgeGrain>(), vertexReference.AsReference<IVertexGrain>());
            }

            foreach (var edge in vertex.GetOutEdges())
            {
                GrainReference edgeReference = grainReferenceConverter.GetGrainFromKeyString(edge.Id.ToString());
                edgeReference.BindGrainReference(grainFactory);
                
                GrainReference vertexReference = grainReferenceConverter.GetGrainFromKeyString(edge.InVertexId.ToString());
                vertexReference.BindGrainReference(grainFactory);

                vertexState.AddOutEdge(edgeReference.AsReference<IEdgeGrain>(), vertexReference.AsReference<IVertexGrain>());
            }

            foreach (var property in vertex.GetVertexProperties())
            { 
                if (property.Key[0] == '@' || property.Key == "partition")
                    continue;

                VertexProperty vertexProperty = vertexState.SetProperty(property.Key, property.Value.ToString());

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
            IGraphElementGrain graphElementGrain = grainReference.AsReference<IGraphElementGrain>();

            string writeExpression = (vertexState.Persisted ? 
                CreateUpdateExpression(grainReference, vertexState, graphElementGrain) : 
                CreateInsertExpression(grainReference, vertexState, graphElementGrain))
                .ToString();
            
            FeedOptions feedOptions = new FeedOptions
            {
                MaxItemCount = 1,
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            var writeQuery = client.CreateGremlinQuery<AzureVertex>(graph, writeExpression, feedOptions, GraphSONMode.Normal);
            
            log.Info($"CosmosDB: Writing VertexState for grain Id '{graphElementGrain.ToKeyString()}'");
            var response = await writeQuery.ExecuteNextAsync<AzureVertex>();
            log.Info($"CosmosDB: Writing VertexState complete: Request Charge: {response.RequestCharge}");

            vertexState.Persisted = true;
        }

        internal async Task ClearVertexStateAsync(GrainReference grainReference, VertexState vertexState)
        {
            IGraphElementGrain graphElementGrain = grainReference.AsReference<IGraphElementGrain>();
            
            FeedOptions feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            string dropCommand = $"g.V('{graphElementGrain.ToKeyString()}')";
            var writeQuery = client.CreateGremlinQuery<AzureVertex>(graph, dropCommand, feedOptions);
            var response = await writeQuery.ExecuteNextAsync<AzureVertex>();
            vertexState.Persisted = false;
            
            log.Info($"CosmosDB: Drop Vertex State: Request Charge: {response.RequestCharge}");
        }

        private static IVertexResult CreateUpdateExpression(GrainReference grainReference, VertexState vertexState, IGraphElementGrain grainWithGraphElement)
        {
            return g.V().has(grainWithGraphElement.GetGraphLabel(), "id", grainReference.ToKeyString()).property(vertexState);
        }

        private static IVertexResult CreateInsertExpression(GrainReference grainReference, VertexState vertexState, IGraphElementGrain graphElementGrain)
        {
            IVertexResult insertExpression = g.AddV(graphElementGrain.GetGraphLabel())
                .property("id", grainReference.ToKeyString());

            string partition = graphElementGrain.GetGraphPartition();
            if (!string.IsNullOrEmpty(partition))
                insertExpression= insertExpression.property("partition", partition);
            
            return insertExpression.property(vertexState);
        }
    }
}