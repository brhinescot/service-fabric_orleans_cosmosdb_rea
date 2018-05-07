using System.Threading.Tasks;
using Orleans;
using Orleans.Graph.Definition;

namespace ReaService.Orleans.Definition
{
    public interface IDecrementCommitmentGrain : IVertexGrain
    {
        Task Fulfill();
    }
}