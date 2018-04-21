#region Using Directives

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Orleans.Graph.Query
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Comparers
    {
        /// <summary>
        ///     Greater-than comparer (>)
        /// </summary>
        /// <param name="value">String value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer gt([NotNull] string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return new Comparer($"gt('{value}')");
        }

        /// <summary>
        ///     Less-than comparer (&lt;)
        /// </summary>
        /// <param name="value">String value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer lt([NotNull] string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return new Comparer($"lt('{value}')");
        }

        /// <summary>
        ///     Equals comparer (=)
        /// </summary>
        /// <param name="value">String value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer eq([NotNull] string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            return new Comparer($"eq('{value}')");
        }

        /// <summary>
        ///     Greater-than comparer (>)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer gt(int value)
        {
            return new Comparer($"gt({value})");
        }

        /// <summary>
        ///     Less-than comparer (&lt;)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer lt(int value)
        {
            return new Comparer($"lt({value})");
        }

        /// <summary>
        ///     Equality comparer (=)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer eq(int value)
        {
            return new Comparer($"eq({value})");
        }

        /// <summary>
        ///     Greater-than comparer (>)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer gt(long value)
        {
            return new Comparer($"gt({value})");
        }

        /// <summary>
        ///     Less-than comparer (&lt;)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer lt(long value)
        {
            return new Comparer($"lt({value})");
        }

        /// <summary>
        ///     Equality comparer (=)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer eq(long value)
        {
            return new Comparer($"eq({value})");
        }

        /// <summary>
        ///     Greater-than comparer (>)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer gt(float value)
        {
            return new Comparer($"gt({value})");
        }

        /// <summary>
        ///     Less-than comparer (&lt;)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer lt(float value)
        {
            return new Comparer($"lt({value})");
        }

        /// <summary>
        ///     Equality comparer (=)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer eq(float value)
        {
            return new Comparer($"eq({value})");
        }

        /// <summary>
        ///     Greater-than comparer (>)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer gt(double value)
        {
            return new Comparer($"gt({value})");
        }

        /// <summary>
        ///     Less-than comparer (&lt;)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer lt(double value)
        {
            return new Comparer($"lt({value})");
        }

        /// <summary>
        ///     Equality comparer (=)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer eq(double value)
        {
            return new Comparer($"eq({value})");
        }

        /// <summary>
        ///     Greater-than comparer (>)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer gt(decimal value)
        {
            return new Comparer($"gt({value})");
        }

        /// <summary>
        ///     Less-than comparer (&lt;)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer lt(decimal value)
        {
            return new Comparer($"lt({value})");
        }

        /// <summary>
        ///     Equality comparer (=)
        /// </summary>
        /// <param name="value">Integer value to compare with</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer eq(decimal value)
        {
            return new Comparer($"eq({value})");
        }

        /// <summary>
        ///     Within comparer
        /// </summary>
        /// <param name="values">A list of string values to search</param>
        /// <returns>Comparer (implicitly converted to string)</returns>
        [NotNull]
        public static Comparer within([NotNull] params string[] values)
        {
            if (values == null)
                throw new ArgumentNullException(nameof(values));
            if (values.Length == 0)
                throw new ArgumentException("Value cannot be an empty collection.", nameof(values));
            
            return new Comparer($"within({values.Select(x => $"'{x}'").Aggregate((a, b) => a + "," + b)})");
        }
    }
}