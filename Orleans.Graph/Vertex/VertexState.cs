#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Orleans.Graph.Definition;
using Orleans.Graph.State;

#endregion

namespace Orleans.Graph.Vertex
{
    /// <summary>
    /// </summary>
    public class VertexState : IEnumerable<VertexProperty>
    {
        private enum Direction
        {
            In,
            Out
        }
        
        private readonly Dictionary<string, VertexProperty> vertexProperties = new Dictionary<string, VertexProperty>();
        private readonly BranchingCache<Direction, string, string, Guid, IEdge> edges = new BranchingCache<Direction, string, string, Guid, IEdge>();
        
        // TODO: Support IEnumerable properties
        public GraphValue this[[NotNull] string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                key = key.LowercaseFirst();

                if (vertexProperties.TryGetValue(key, out var property))
                    return property.Value;

                var newProperty = new VertexProperty(key, null);
                vertexProperties.Add(key, newProperty);
                return newProperty.Value;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                key = key.LowercaseFirst();
                
                var vertexProperty = new VertexProperty(key, value);

                if (vertexProperties.ContainsKey(key))
                    vertexProperties[key] = vertexProperty;
                else
                    vertexProperties.Add(key, vertexProperty);
            }
        }

        public bool Persisted { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [NotNull]
        public VertexProperty SetProperty([NotNull] string key, GraphValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                return new VertexProperty(key, null);

            var property = new VertexProperty(key, value);
            if (vertexProperties.ContainsKey(key))
                vertexProperties[key] = property;
            else
                vertexProperties.Add(key, property);

            return property;
        }
        
        #region IEnumerable<VertexProperty> Members

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<VertexProperty> IEnumerable<VertexProperty>.GetEnumerator()
        {
            return vertexProperties.Values.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<VertexProperty>) this).GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Adds an in edge to this vertex.
        /// </summary>
        /// <remarks>
        /// It is not necessary to call the <c>WriteStateAsync()</c> method when adding an edge. Edge state is not stored or written 
        /// with the vertex. On activation, if configured, edges will be read into the state for runtime usage.
        /// </remarks>
        /// <param name="edge"></param>
        /// <param name="outVertex"></param>
        public void AddInEdge([NotNull] IEdge edge, IVertex outVertex)
        {
            if (edge == null) 
                throw new ArgumentNullException(nameof(edge));
            if (outVertex == null) 
                throw new ArgumentNullException(nameof(outVertex));

            edges.Set(Direction.In, edge.GetGraphLabel(), outVertex.GetGraphLabel(), outVertex.GetGraphRuntimeId(), edge);
        }

        /// <summary>
        /// Adds an out edge to this vertex.
        /// </summary>
        /// <remarks>
        /// It is not necessary to call the <c>WriteStateAsync()</c> method when adding an edge. Edge state is not stored or written 
        /// with the vertex. On activation, if configured, edges will be read into the state for runtime usage.
        /// </remarks>
        /// <param name="edge"></param>
        /// <param name="inVertex"></param>
        public void AddOutEdge([NotNull] IEdge edge, IVertex inVertex)
        {
            if (edge == null)
                throw new ArgumentNullException(nameof(edge));
            if (inVertex == null)
                throw new ArgumentNullException(nameof(inVertex));
            
            edges.Set(Direction.Out, edge.GetGraphLabel(), inVertex.GetGraphLabel(), inVertex.GetGraphRuntimeId(), edge);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<IEdge> GetInEdges([CanBeNull] string edgeLabel = null)
        {
            return edgeLabel == null ? edges.GetSubset(Direction.In) : edges.GetSubset(Direction.In, edgeLabel);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<IEdge> GetOutEdges([CanBeNull] string edgeLabel = null)
        {
            return edgeLabel == null ? edges.GetSubset(Direction.Out) : edges.GetSubset(Direction.Out, edgeLabel);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<IEdge> GetInEdges([NotNull] string edgeLabel, [NotNull] string vertexLabel)
        {
            if (edgeLabel == null) 
                throw new ArgumentNullException(nameof(edgeLabel));
            if (vertexLabel == null) 
                throw new ArgumentNullException(nameof(vertexLabel));
            
            return edges.GetSubset(Direction.In, edgeLabel, vertexLabel);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<IEdge> GetOutEdges([NotNull] string edgeLabel, [NotNull] string vertexLabel)
        {
            if (edgeLabel == null) 
                throw new ArgumentNullException(nameof(edgeLabel));
            if (vertexLabel == null) 
                throw new ArgumentNullException(nameof(vertexLabel));
            
            return edges.GetSubset(Direction.Out, edgeLabel, vertexLabel);
        }

        [ItemNotNull, NotNull]
        public IEnumerable<IEdge> GetBothEdges([CanBeNull] string label = null)
        {
            foreach (IEdge edgeGrain in GetInEdges(label))
                yield return edgeGrain;

            foreach (IEdge edgeGrain in GetOutEdges(label))
                yield return edgeGrain;
        }

        [ItemNotNull, NotNull]
        public IEnumerable<IEdge> GetBothEdges([NotNull] string edgeLabel, [NotNull] string vertexLabel)
        {
            if (edgeLabel == null) 
                throw new ArgumentNullException(nameof(edgeLabel));
            if (vertexLabel == null) 
                throw new ArgumentNullException(nameof(vertexLabel));
            
            foreach (IEdge edgeGrain in GetInEdges(edgeLabel, vertexLabel))
                yield return edgeGrain;

            foreach (IEdge edgeGrain in GetOutEdges(edgeLabel, vertexLabel))
                yield return edgeGrain;
        }
    }
}