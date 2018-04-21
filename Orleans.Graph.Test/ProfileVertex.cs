#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph.Test.Definition;

#endregion

namespace Orleans.Graph.Test
{
    public class ProfileVertex : VertexGrain, IProfileVertex
    {
        public async Task SetProfileData(ProfileData data)
        {
            State["name"] = data.Name;

            await WriteStateAsync();
        }

        public Task<ProfileData> GetProfileDataAsync()
        {
            ProfileData data = new ProfileData
            {
                Name = State["name"]
            };

            return Task.FromResult(data);
        }
    }
}