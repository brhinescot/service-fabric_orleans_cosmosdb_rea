#region Using Directives

using System;
using System.Threading.Tasks;
using Orleans.Core;
using Orleans.Graph.Definition;
using Orleans.Graph.Edge;
using Orleans.Graph.Vertex;
using Orleans.Providers;
using Orleans.Runtime;

#endregion

namespace Orleans.Graph
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TInVertex"></typeparam>
    /// <typeparam name="TOutVertex"></typeparam>
    [StorageProvider(ProviderName = "CosmosDBGraph")]
    public abstract class EdgeGrain<TInVertex, TOutVertex> : Grain<EdgeState> where TInVertex : IVertexGrain where TOutVertex : IVertexGrain
    {
    }
}