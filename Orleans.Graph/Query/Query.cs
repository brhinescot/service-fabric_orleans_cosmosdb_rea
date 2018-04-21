#region Using Directives

using System.Text;

#endregion

namespace Orleans.Graph.Query
{
    public partial class Query
    {
        #region Member Fields

        private readonly StringBuilder queryBuilder = new StringBuilder("g");

        #endregion

        /// <summary>
        ///     Returns the full Gremlin Query composed from this statement
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return queryBuilder.ToString();
        }

        /// <summary>
        ///     Add a vertex to the graph. If you want to add more properties to this vertex, add .property() statements after this
        ///     one.
        /// </summary>
        /// <param name="label">optional label</param>
        /// <returns></returns>
        public Query AddV(string label = null)
        {
            queryBuilder.Append(label == null ? ".addV()" : $".addV('{label}')");
            return this;
        }

        /// <summary>
        ///     Count the number of vertices/edges in this resultset.
        /// </summary>
        /// <returns></returns>
        public Query Count()
        {
            queryBuilder.Append(".count()");
            return this;
        }

        /// <summary>
        ///     Drop the vertices/edges matching this resultset
        /// </summary>
        /// <returns></returns>
        public Query Drop()
        {
            queryBuilder.Append(".drop()");
            return this;
        }
    }
}