#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Orleans.Graph.State;
using Orleans.Graph.Vertex;

#endregion

namespace Orleans.Graph.Query
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IVertexResult
    {
        /// <summary>
        ///     Add an edge to the graph. If you want to add more properties to this edge, add .property() statements after this
        ///     one.
        /// </summary>
        /// <param name="label">optional label</param>
        /// <returns></returns>
        [NotNull]
        IEdgeResult addE([CanBeNull] string label);

        [NotNull]
        IVertexResult @as([NotNull] string name);

        [NotNull]
        IVertexResult @by([NotNull] string label);

        /// <summary>
        ///     De-duplify this resultset (a.k.a. DISTINCT)
        /// </summary>
        /// <returns></returns>
        [NotNull]
        Query dedup();

        [NotNull]
        IGroupResult @group();
        
        [NotNull]
        IVertexResult has([NotNull] string key, [NotNull] Comparer comparer);

        [NotNull]
        IVertexResult @in([CanBeNull] string label);

        [NotNull]
        IEdgeResult inE([CanBeNull] string label);

        [NotNull]
        IVertexResult limit(int limit);

        /// <summary>
        ///     Get the mean value of this resultset (a.k.a. AVERAGE)
        /// </summary>
        /// <returns></returns>
        [NotNull]
        IVertexResult mean();

        [NotNull]
        IVertexResult @out([CanBeNull] string label);

        [NotNull]
        IEdgeResult outE([CanBeNull] string label);
        
        [NotNull]
        IVertexResult property([NotNull] string key, [NotNull] GraphValue value);
        
        [NotNull]
        IVertexResult select([NotNull] params string[] names);

        [NotNull]
        IVertexResult V([CanBeNull] string id);

        [NotNull]
        IVertexResult V(int id);

        [NotNull]
        IVertexResult V(Guid id);

        [NotNull]
        IVertexResult values([NotNull] params string[] keys);
        
        [NotNull]
        IVertexResult has([NotNull] string label, [NotNull] string key, [NotNull] GraphValue value);
        
        [NotNull]
        IVertexResult has([NotNull] string key, GraphValue value);

        [NotNull]
        IVertexResult property([NotNull] IEnumerable<VertexProperty> properties);
    }
}