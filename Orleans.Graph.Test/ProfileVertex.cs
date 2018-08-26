#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph.Test.Definition;

#endregion

namespace Orleans.Graph.Test
{
    public class ProfileVertex : VertexGrain, IProfile
    {
        public Task SetProfileData(ProfileData data)
        {
            State["name"] = data.Name;

            return WriteStateAsync();
        }

        public Task<ProfileData> GetProfileDataAsync()
        {
            var data = new ProfileData
            {
                Name = State["name"]
            };

            return Task.FromResult(data);
        }
    }
}