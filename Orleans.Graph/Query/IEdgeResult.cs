#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Orleans.Graph.State;

#endregion

namespace Orleans.Graph.Query
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IEdgeResult
    {
        [NotNull]
        IEdgeResult E([CanBeNull] string id);

        [NotNull]
        IEdgeResult E(Guid id);

        [NotNull]
        IEdgeResult from([NotNull] IVertexResult source);

        [NotNull]
        IEdgeResult has([NotNull] string key, [NotNull] GraphValue value);

        [NotNull]
        IEdgeResult has([NotNull] string label, [NotNull] string key, [NotNull] GraphValue value);
        
        [NotNull]
        IEdgeResult hasId([NotNull] string id);

        [NotNull]
        IEdgeResult hasId(Guid id);

        [NotNull]
        IEdgeResult hasId(int id);

        [NotNull]
        IEdgeResult hasLabel([NotNull] string label);

        [NotNull]
        IVertexResult inV([CanBeNull] string label);

        [NotNull]
        IEdgeResult limit(int limit);

        [NotNull]
        IVertexResult outV([CanBeNull] string label);

        [NotNull]
        IEdgeResult property([NotNull] string key, [NotNull] GraphValue value);

        [NotNull]
        IEdgeResult property<TProperty>([NotNull] IEnumerable<TProperty> properties, [NotNull] KeyValueAction<TProperty> action);
        
        [NotNull]
        IEdgeResult to([NotNull] IVertexResult goal);
    }
}