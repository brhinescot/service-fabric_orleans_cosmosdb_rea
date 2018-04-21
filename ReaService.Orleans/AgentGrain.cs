using System.Threading.Tasks;
using Orleans.Graph;
using ReaService.Orleans.Definition;

namespace ReaService.Orleans
{
    public class AgentGrain : VertexGrain, IAgentGrain
    {
        public Task AddInfo(string name)
        {
            State["name"] = "Test";

            return Task.CompletedTask;
        }
    }
}
