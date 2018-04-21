#region Using Directives

using System;
using System.Collections.Generic;
using Orleans.Graph.State;

#endregion

namespace Orleans.Graph.Query
{
    public delegate (string key, GraphValue value) KeyValueAction<in TProperty>(TProperty property);

    public partial class Query : IEdgeResult
    {
        IEdgeResult IEdgeResult.E(string id)
        {
            queryBuilder.Append(id != null ? $".E('{id}')" : ".E()");
            return this;
        }

        IEdgeResult IEdgeResult.E(Guid id)
        {
            queryBuilder.Append($".E('{id:N}')");
            return this;
        }

        IEdgeResult IEdgeResult.property(string key, GraphValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            queryBuilder.Append($".property('{key}',{value.ToGraphString()})");
            return this;
        }
        
        IEdgeResult IEdgeResult.property<TProperty>(IEnumerable<TProperty> properties, KeyValueAction<TProperty> action)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            foreach (TProperty property in properties)
            {
                (string key, GraphValue value) = action(property);
                
                queryBuilder.Append($".property('{key}',{value.ToGraphString()})");
            }
            return this;
        }

        IEdgeResult IEdgeResult.to(IVertexResult goal)
        {
            if (goal == null)
                throw new ArgumentNullException(nameof(goal));

            queryBuilder.Append($".to({goal})");
            return this;
        }

        IEdgeResult IEdgeResult.from(IVertexResult source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            queryBuilder.Append($".from({source})");
            return this;
        }

        IEdgeResult IEdgeResult.hasLabel(string label)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));

            queryBuilder.Append($".hasLabel('{label}')");
            return this;
        }

        IEdgeResult IEdgeResult.hasId(string id)
        {
            if (id == null)
                throw new ArgumentNullException(nameof(id));

            queryBuilder.Append($".hasId('{id}')");
            return this;
        }

        IEdgeResult IEdgeResult.hasId(Guid id)
        {
            queryBuilder.Append($".hasId('{id:D}')");
            return this;
        }

        IEdgeResult IEdgeResult.hasId(int id)
        {
            queryBuilder.Append($".hasId('{id}')");
            return this;
        }

        IEdgeResult IEdgeResult.has(string key, GraphValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            queryBuilder.Append($".has('{key}',{value.ToGraphString()})");
            return this;
        }
        
        IEdgeResult IEdgeResult.has(string label, string key, GraphValue value)
        {
            if (label == null)
                throw new ArgumentNullException(nameof(label));
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            queryBuilder.Append($".has('{label}','{key}',{value.ToGraphString()})");
            return this;
        }

        IVertexResult IEdgeResult.inV(string label)
        {
            queryBuilder.Append(label != null ? $".inV('{label}')" : ".inV()");
            return this;
        }

        IVertexResult IEdgeResult.outV(string label)
        {
            queryBuilder.Append(label != null ? $".outV('{label}')" : ".outV()");
            return this;
        }

        IEdgeResult IEdgeResult.limit(int limit)
        {
            queryBuilder.Append($".limit({limit})");
            return this;
        }
    }
}