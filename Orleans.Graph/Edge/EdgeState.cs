#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Orleans.Graph.Definition;
using Orleans.Graph.State;
using Orleans.Graph.Vertex;

#endregion

namespace Orleans.Graph.Edge
{
    /// <summary>
    /// 
    /// </summary>
    public class EdgeState : IEnumerable<Property>
    {
        private readonly Dictionary<string, Property> properties = new Dictionary<string, Property>();
        private IVertexGrain inVertex;
        private IVertexGrain outVertex;
        
        public GraphValue this[[NotNull] string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                key = key.LowercaseFirst();

                if (properties.TryGetValue(key, out Property property))
                    return property.Value;

                Property newProperty = new Property(key, null);
                properties.Add(key, newProperty);
                return newProperty.Value;
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException(nameof(key));

                key = key.LowercaseFirst();

                Property vertexProperty = new Property(key, value);

                if (properties.ContainsKey(key))
                    properties[key] = vertexProperty;
                else
                    properties.Add(key, vertexProperty);
            }
        }

        public bool Persisted { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void SetInVertex([NotNull] IVertexGrain vertex)
        {
            inVertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertex"></param>
        public void SetOutVertex([NotNull] IVertexGrain vertex)
        {
            outVertex = vertex ?? throw new ArgumentNullException(nameof(vertex));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public IVertexGrain GetInVertex() => inVertex;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [CanBeNull]
        public IVertexGrain GetOutVertex() => outVertex;

        #region IEnumerable<Property> Members

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<Property> IEnumerable<Property>.GetEnumerator() => properties.Values.GetEnumerator();

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<Property>) this).GetEnumerator();

        #endregion
    }
}