#region Using Directives

using Orleans.Graph.Query;

#endregion

namespace Orleans.Graph.StorageProvider
{
    internal abstract class GraphElementProvider
    {
        protected static string CreateUpsertExpression(IEdgeResult updateExpression, IEdgeResult insertExpression)
        {
            return $"{updateExpression}.fold().coalesce(unfold(), {insertExpression})";
        }
    }
} 