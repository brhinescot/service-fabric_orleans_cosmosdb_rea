#region Using Directives

using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Test.Definition;

#endregion

namespace Orleans.Graph.Test
{
    [UsedImplicitly]
    public class HasProfileEdge : EdgeGrain<IPersonVertex, IProfileVertex>, IHasProfileEdge
    {
        public async Task<IProfileVertex> AddProfile(IPersonVertex personVertex, ProfileData data)
        {
            IProfileVertex profileVertex = GrainFactory.GetVertexGrain<IProfileVertex>(this.GetGraphRuntimeId(), this.GetGraphPartition());
            await profileVertex.SetProfileData(data);
            
            State.SetInVertex(personVertex);
            State.SetOutVertex(profileVertex);

            await WriteStateAsync();
            return profileVertex;
        }
    }
}