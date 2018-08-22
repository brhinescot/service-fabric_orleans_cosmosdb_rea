#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans
{
    public class DecrementCommitmentGrain : VertexGrain, IDecrementCommitment
    {
        public async Task Fulfill()
        {
            State[StateKeys.Fulfilled] = true;

            await WriteStateAsync();
        }

        private struct StateKeys
        {
            public static string Fulfilled => "Fulfilled";
        }
    }
}