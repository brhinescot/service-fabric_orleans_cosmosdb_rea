#region Using Directives

using Orleans.Graph.Definition;
using Orleans.Graph.Edge;
using Orleans.Providers;

#endregion

namespace Orleans.Graph
{
    /// <summary>
    /// </summary>
    /// <typeparam name="TInVertex"></typeparam>
    /// <typeparam name="TOutVertex"></typeparam>
    [StorageProvider(ProviderName = "CosmosDBGraph")]
    public abstract class EdgeGrain<TInVertex, TOutVertex> : Grain<EdgeState>, IEdge where TInVertex : IVertex where TOutVertex : IVertex { }
}