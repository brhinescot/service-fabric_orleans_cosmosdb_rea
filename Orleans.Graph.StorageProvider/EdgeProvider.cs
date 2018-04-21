#region Using Directives

using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Graphs;
using Microsoft.Extensions.Logging;
using Orleans.Graph.Definition;
using Orleans.Graph.Edge;
using Orleans.Graph.Query;
using Orleans.Runtime;
using AzureEdge = Microsoft.Azure.Graphs.Elements.Edge;
using AzureProperty = Microsoft.Azure.Graphs.Elements.Property;

#endregion

namespace Orleans.Graph.StorageProvider
{
    internal class EdgeProvider : GraphElementProvider
    {
        #region Member Fields

        private readonly DocumentClient client;
        private readonly DocumentCollection graph;
        private readonly IGrainReferenceConverter grainReferenceConverter;
        private readonly IGrainFactory grainFactory;
        private readonly string serviceId;
        private readonly ILogger log;

        #endregion

        internal EdgeProvider(DocumentClient client, DocumentCollection graph, IGrainReferenceConverter grainReferenceConverter, ILogger log, IGrainFactory grainFactory, string serviceId)
        {
            this.client = client;
            this.graph = graph;
            this.grainReferenceConverter = grainReferenceConverter;
            this.grainFactory = grainFactory;
            this.serviceId = serviceId;
            this.log = log;
        }

        internal async Task ReadEdgeStateAsync(GrainReference grainReference, EdgeState edgeState)
        {
            IGraphElementGrain graphElementGrain = grainReference.AsReference<IGraphElementGrain>();
            
            string readExpression = $"g.E('{grainReference.ToKeyString()}')";

            FeedOptions feedOptions = new FeedOptions
            {
                MaxItemCount = 1,
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            var readQuery = client.CreateGremlinQuery<AzureEdge>(graph, readExpression, feedOptions, GraphSONMode.Normal);
            var response = await readQuery.ExecuteNextAsync<AzureEdge>();
            log.Info($"CosmosDB: Read Edge State: Request Charge: {response.RequestCharge}");

            AzureEdge edge = response.FirstOrDefault();
            if (edge == null)
                return;

            edgeState.Persisted = true;

            GrainReference inV = grainReferenceConverter.GetGrainFromKeyString(edge.InVertexId.ToString());
            GrainReference outV = grainReferenceConverter.GetGrainFromKeyString(edge.OutVertexId.ToString());
            inV.BindGrainReference(grainFactory);
            outV.BindGrainReference(grainFactory);

            edgeState.SetInVertex(inV.AsReference<IVertexGrain>());
            edgeState.SetOutVertex(outV.AsReference<IVertexGrain>());

            foreach (AzureProperty property in edge.GetProperties())
            {
                if (property.Key[0] == '@' || property.Key == "partition")
                    continue;

                edgeState[property.Key] = property.Value.ToString();
            }
        }

        internal async Task WriteEdgeStateAsync(GrainReference grainReference, EdgeState edgeState)
        {
            IGraphElementGrain graphElementGrain = grainReference.AsReference<IGraphElementGrain>();

            string upsertExpression = (edgeState.Persisted ?
                    CreateUpdateExpression(grainReference, edgeState, graphElementGrain) :
                    CreateInsertExpression(grainReference, edgeState, graphElementGrain))
                .ToString();

            FeedOptions feedOptions = new FeedOptions
            {
                MaxItemCount = 1,
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            var writeQuery = client.CreateGremlinQuery<AzureEdge>(graph, upsertExpression, feedOptions, GraphSONMode.Normal);
            
            log.Info($"CosmosDB: Writing EdgeState for grain Id '{graphElementGrain.ToKeyString()}'");
            var response = await writeQuery.ExecuteNextAsync<AzureEdge>();
            log.Info($"CosmosDB: Writing EdgeState complete: Request Charge: {response.RequestCharge}");

            edgeState.Persisted = true;
        }

        //g.addV(T.label, 'person', T.id, '9', 'name', 'Brian', 'age', 42).as('a').properties('name').hasValue('Brian').property('acl','public').property('createdOn','12/01/2016').select('a').properties('age').hasValue(42).property('acl','private').select('a')
        internal async Task ClearEdgeStateAsync(GrainReference grainReference, EdgeState edgeState)
        {
            IGraphElementGrain graphElementGrain = grainReference.AsReference<IGraphElementGrain>();

            FeedOptions feedOptions = new FeedOptions
            {
                PartitionKey = new PartitionKey(graphElementGrain.GetGraphPartition())
            };

            string dropCommand = $"g.E('{graphElementGrain.ToKeyString()}')";
            var writeQuery = client.CreateGremlinQuery<AzureEdge>(graph, dropCommand, feedOptions);
            var response = await writeQuery.ExecuteNextAsync<AzureEdge>();
            edgeState.Persisted = false;

            log.Info($"CosmosDB: Drop Vertex State: Request Charge: {response.RequestCharge}");
        }

        private static IEdgeResult CreateInsertExpression(GrainReference grainReference, EdgeState edgeState, IGraphElementGrain graphElementGrain)
        {
            IVertexGrain inVertex = edgeState.GetInVertex();
            IVertexGrain outVertex = edgeState.GetOutVertex();

            string inVertexKeyString = inVertex.ToKeyString();
            string outVertexKeyString = outVertex.ToKeyString();

            IEdgeResult insertExpression = g.V(inVertexKeyString)
                .addE(graphElementGrain.GetGraphLabel())
                .to(g.V(outVertexKeyString))
                .property("id", grainReference.ToKeyString());

            string partition = graphElementGrain.GetGraphPartition();
            if (!string.IsNullOrEmpty(partition))
                insertExpression = insertExpression.property("partition", partition);
            
            return insertExpression.property(edgeState, p => (p.Key, p.Value));
        }

        private static IEdgeResult CreateUpdateExpression(GrainReference grainReference, EdgeState edgeState, IGraphElementGrain graphElementGrain)
        {
            return g.E()
                .has(graphElementGrain.GetGraphLabel(), "id", grainReference.ToKeyString())
                .property(edgeState, p => (p.Key, p.Value));
        }
    }
}