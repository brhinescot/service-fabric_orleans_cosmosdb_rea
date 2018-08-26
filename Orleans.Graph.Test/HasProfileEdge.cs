#region Using Directives

using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph.Test.Definition;

#endregion

namespace Orleans.Graph.Test
{
    [UsedImplicitly]
    public class HasProfileEdge : EdgeGrain<IPerson, IProfile>, IHasProfile
    {
        public async Task<IProfile> AddProfile(IPerson person, ProfileData data)
        {
            var profile = GrainFactory.GetVertexGrain<IProfile>(this.GetGraphRuntimeId(), this.GetGraphPartition());
            await profile.SetProfileData(data);
            
            State.SetInVertex(person);
            State.SetOutVertex(profile);

            await WriteStateAsync();
            return profile;
        }
    }
}