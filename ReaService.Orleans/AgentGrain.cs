#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans
{
    public class AgentGrain : VertexGrain, IAgent
    {
        public Task AddInfo(string name)
        {
            State["name"] = "Test";

            return WriteStateAsync();
        }
    }
}