#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph.Definition;

#endregion

namespace Orleans.Graph.Test.Definition
{
    [GraphElement(DefaultPartition = "users")]
    public interface IHasProfile : IEdge
    {
        Task<IProfile> AddProfile(IPerson person, ProfileData data);
    }
}