#region Using Directives

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

#endregion

namespace Orleans.Graph.Query
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IGroupResult
    {
        [NotNull]
        IGroupResult by(Field field);

        [NotNull]
        IGroupResult by([NotNull] string field);
    }
}