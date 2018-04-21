#region Using Directives

using System.Diagnostics;

#endregion

namespace Orleans.Graph.Test.Definition
{
    [DebuggerDisplay("Name: {Name,nq}")]
    public class ProfileData
    {
        #region Properties

        public string Name { get; set; }

        #endregion
    }
}