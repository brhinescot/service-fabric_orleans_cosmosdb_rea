#region Using Directives

using System;
using System.Diagnostics;
using JetBrains.Annotations;

#endregion

namespace Orleans.Graph.State
{
    /// <summary>
    /// </summary>
    [DebuggerDisplay("{Key,nq}: {Value.ToString(),nq}")]
    public class Property : IEquatable<Property>
    {
        /// <summary>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="persisted"></param>
        public Property([NotNull] string key, [CanBeNull] GraphValue value, bool persisted = false)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value;
            Persisted = persisted;
        }

        public bool Equals(Property other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(Key, other.Key, StringComparison.InvariantCulture) && Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Property) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (StringComparer.InvariantCulture.GetHashCode(Key) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }

        public static bool operator ==(Property left, Property right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Property left, Property right)
        {
            return !Equals(left, right);
        }

        #region Properties

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
    }
}