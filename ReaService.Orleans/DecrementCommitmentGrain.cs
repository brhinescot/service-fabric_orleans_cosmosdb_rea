#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans
{
    public class DecrementCommitmentGrain : VertexGrain, IDecrementCommitment
    {
        public Task Fulfill()
        {
            State[StateKeys.Fulfilled] = true;

            return WriteStateAsync();
        }

        private struct StateKeys
        {
            public static string Fulfilled => "Fulfilled";
        }
    }
}