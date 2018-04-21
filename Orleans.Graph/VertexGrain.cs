#region Using Directives

using Orleans.Graph.Definition;
using Orleans.Graph.Vertex;
using Orleans.Providers;

#endregion

namespace Orleans.Graph
{
    /// <summary>
    /// 
    /// </summary>
    [StorageProvider(ProviderName = "CosmosDBGraph")]
    public abstract class VertexGrain : Grain<VertexState>{}
}