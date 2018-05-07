using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans;
using Orleans.Graph.Definition;

namespace ReaService.Orleans.Definition
{
    public interface IIncrementCommitmentGrain : IVertexGrain
    {
        Task Fulfill();

        /// <exception cref="ArgumentNullException"><paramref name="provider"/> is <see langword="null"/></exception>
        /// <exception cref="ArgumentNullException"><paramref name="resource"/> is <see langword="null"/></exception>
        Task Initialize([NotNull] IAgentGrain provider, [NotNull] IResourceGrain resource, double amount);
    }
}