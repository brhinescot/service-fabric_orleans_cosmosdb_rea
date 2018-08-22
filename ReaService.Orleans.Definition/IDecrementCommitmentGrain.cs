#region Using Directives

using System.Threading.Tasks;
using Orleans.Graph.Definition;

#endregion

namespace ReaService.Orleans.Definition
{
    public interface IDecrementCommitment : IVertex
    {
        Task Fulfill();
    }
}