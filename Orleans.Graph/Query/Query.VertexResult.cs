#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Orleans.Graph.State;
using Orleans.Graph.Vertex;

#endregion

namespace Orleans.Graph.Query
{
    public partial class Query : IVertexResult
    {
        /// <summary>
        ///     De-duplify this resultset (a.k.a. DISTINCT)
        /// </summary>
        /// <returns></returns>
        Query IVertexResult.dedup()
        {
            queryBuilder.Append(".dedup()");
            return this;
        }

        IVertexResult IVertexResult.limit(int limit)
        {
            if (limit <= 0)
                throw new ArgumentException("Value must be greater than zero.", nameof(limit));

            queryBuilder.Append($".limit({limit})");
            return this;
        }

        /// <summary>
        ///     Get the mean value of this resultset (a.k.a. AVERAGE)
        /// </summary>
        /// <returns></returns>
        IVertexResult IVertexResult.mean()
        {
            queryBuilder.Append(".mean()");
            return this;
        }

        /// <summary>
        ///     Add an edge to the graph. If you want to add more properties to this edge, add .property() statements after this
        ///     one.
        /// </summary>
        /// <param name="label">optional label</param>
        /// <returns></returns>
        IEdgeResult IVertexResult.addE(string label)
        {
            queryBuilder.Append(label == null ? ".addE()" : $".addE('{label}')");
            return this;
        }

        IVertexResult IVertexResult.V(string id)
        {
            queryBuilder.Append(id != null ? $".V('{id}')" : ".V()");
            return this;
        }

        IVertexResult IVertexResult.V(int id)
        {
            queryBuilder.Append($".V({id})");
            return this;
        }

        IVertexResult IVertexResult.V(Guid id)
        {
            queryBuilder.Append($".V({id:D})");
            return this;
        }

        IVertexResult IVertexResult.has(string key, GraphValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            queryBuilder.Append($".has('{key}',{value.ToGraphString()})");
            return this;
        }

        IVertexResult IVertexResult.has(string label, string key, GraphValue value)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            queryBuilder.Append($".has('{label}','{key}',{value.ToGraphString()})");

            return this;
        }

        IVertexResult IVertexResult.property(string key, GraphValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            queryBuilder.Append($".property('{key}',{value.ToGraphString()})");
            return this;
        }
        
        IVertexResult IVertexResult.property(IEnumerable<VertexProperty> properties)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            
            foreach (VertexProperty property in properties)
            {
                if (property.Value?.Type == null) 
                    continue;
                
                queryBuilder.Append($".property('{property.Key}',{property.Value.ToGraphString()}");
                foreach (Property meta in property.GetProperties())
                    queryBuilder.Append($",'{meta.Key}',{meta.Value?.ToGraphString()}");
                queryBuilder.Append(")");
            }
            return this;
        }

        IVertexResult IVertexResult.has(string key, Comparer comparer)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (comparer == null)
                throw new ArgumentNullException(nameof(comparer));

            queryBuilder.Append($".has('{key}',{comparer})");
            return this;
        }

        IVertexResult IVertexResult.@in(string label)
        {
            queryBuilder.Append(label != null ? $".in('{label}')" : ".in()");
            return this;
        }

        IEdgeResult IVertexResult.inE(string label)
        {
            queryBuilder.Append(label != null ? $".inE('{label}')" : ".inE()");
            return this;
        }

        IVertexResult IVertexResult.@out(string label)
        {
            queryBuilder.Append(label != null ? $".out('{label}')" : ".out()");
            return this;
        }

        IEdgeResult IVertexResult.outE(string label)
        {
            queryBuilder.Append(label != null ? $".outE('{label}')" : ".outE()");
            return this;
        }

        IVertexResult IVertexResult.select(params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            if (names.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(names));

            queryBuilder.Append($".select({names.Select(x => $"'{x}'").Aggregate((a, b) => a + "," + b)})");
            return this;
        }

        IVertexResult IVertexResult.values(params string[] keys)
        {
            if (keys == null)
                throw new ArgumentNullException(nameof(keys));
            if (keys.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(keys));

            queryBuilder.Append($".values({keys.Select(x => $"'{x}'").Aggregate((a, b) => a + "," + b)})");
            return this;
        }

        IVertexResult IVertexResult.@as(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            queryBuilder.Append($".as('{name}')");
            return this;
        }

        IVertexResult IVertexResult.by(string label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            queryBuilder.Append($".by('{label}')");
            return this;
        }

        IGroupResult IVertexResult.group()
        {
            queryBuilder.Append(".group()");
            return this;
        }
    }
}