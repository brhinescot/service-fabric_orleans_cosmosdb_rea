using System.Threading.Tasks;
using Orleans;
using Orleans.Graph;
using ReaService.Orleans.Definition;

namespace ReaService.Orleans
{
    public class DecrementCommitmentGrain : VertexGrain, IDecrementCommitmentGrain
    {
        private struct StateKeys
        {
            public static string Fulfilled => "Fulfilled";
        }

        public async Task Fulfill()
        {
            State[StateKeys.Fulfilled] = true;

            await WriteStateAsync();
        }
    }
}