#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Orleans.Graph.State;
using Orleans.Runtime;

#endregion

namespace Orleans.Graph.Vertex
{
    /// <summary>
    /// </summary>
    [DebuggerDisplay("{Key,nq}: {Value.ToString(),nq}")]
    public class VertexProperty : IEnumerable<Property>, IEquatable<VertexProperty>
    {
        #region Member Fields

        private readonly Dictionary<string, Property> properties = new Dictionary<string, Property>();

        #endregion
        
        #region Properties

        /// <summary>
        /// </summary>
        public Guid Id { get; internal set; }

        /// <summary>
        /// </summary>
        [NotNull]
        public string Key { get; }

        /// <summary>
        /// </summary>
        [CanBeNull]
        public GraphValue Value { get; }

        /// <summary>
        /// </summary>
        public bool Persisted { get; }

        #endregion

        #region .ctor

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="persisted"></param>
        public VertexProperty([NotNull] string key, [CanBeNull] GraphValue value, bool persisted = false)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value;
            Persisted = persisted;
            
            if(!persisted && key != "partition")
            {
                SetMeta("createdOn", DateTime.Now);
                SetMeta("createdBy", $"'{RequestContext.Get("User")}'");
            }
        }

            #endregion

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        [NotNull]
        public VertexProperty SetMeta([NotNull] string key, [CanBeNull] GraphValue value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            Property property = new Property(key, value);
            if (properties.ContainsKey(key))
                properties[key] = property;
            else
                properties.Add(key, property);

            return this;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public IEnumerable<Property> GetProperties()
        {
            return properties.Values;
        }

        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [CanBeNull]
        public Property Get([NotNull] string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            properties.TryGetValue(key, out Property value);
            return value;
        }

        #region IEnumerable<Property> Members

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        IEnumerator<Property> IEnumerable<Property>.GetEnumerator()
        {
            return properties.Values.GetEnumerator();
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Property>) this).GetEnumerator();
        }

        #endregion

        #region Equality Members

        public bool Equals(VertexProperty other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id.Equals(other.Id) && string.Equals(Key, other.Key, StringComparison.InvariantCulture) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((VertexProperty)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ StringComparer.InvariantCulture.GetHashCode(Key);
                hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(VertexProperty left, VertexProperty right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(VertexProperty left, VertexProperty right)
        {
            return !Equals(left, right);
        }

        #endregion
    }
}