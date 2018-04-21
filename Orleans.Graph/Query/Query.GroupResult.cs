#region Using Directives

using System;
using System.ComponentModel;

#endregion

namespace Orleans.Graph.Query
{
    public partial class Query : IGroupResult
    {
        IGroupResult IGroupResult.by(Field field)
        {
            if (!Enum.IsDefined(typeof(Field), field))
                throw new InvalidEnumArgumentException(nameof(field), (int) field, typeof(Field));

            queryBuilder.Append($".by({field})");
            return this;
        }

        IGroupResult IGroupResult.by(string field)
        {
            if (field == null)
                throw new ArgumentNullException(nameof(field));

            queryBuilder.Append($".by('{field}')");
            return this;
        }
    }
}