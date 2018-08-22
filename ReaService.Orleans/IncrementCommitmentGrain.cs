#region Using Directives

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Orleans.Graph;
using Orleans.Graph.Definition;
using ReaService.Orleans.Definition;

#endregion

namespace ReaService.Orleans
{
    public class IncrementCommitmentGrain : VertexGrain, IIncrementCommitment
    {
        /// <exception cref="ArgumentNullException"><paramref name="provider" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentNullException"><paramref name="resource" /> is <see langword="null" /></exception>
        public async Task Initialize([NotNull] IAgent provider, [NotNull] IResource resource, double amount)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));
            if (resource == null)
                throw new ArgumentNullException(nameof(resource));

            State[StateKeys.Amount] = amount;

            var hasProviderEdge = GrainFactory.GetEdgeGrain<IHasProviderEdge>(Guid.NewGuid(), this);
            State.AddOutEdge(hasProviderEdge, provider);

            var hasResourceEdge = GrainFactory.GetEdgeGrain<IHasResourceEdge>(Guid.NewGuid(), this);
            State.AddOutEdge(hasResourceEdge, resource);

            await WriteStateAsync();
        }

        public async Task Fulfill()
        {
            State[StateKeys.Fulfilled] = true;

            await WriteStateAsync();
        }

        private struct StateKeys
        {
            [NotNull]
            public static string Fulfilled => "Fulfilled";

            public static string Amount => "Amount";
        }
    }

    public class HasProviderEdge : EdgeGrain<IncrementCommitmentGrain, AgentGrain>, IHasProviderEdge { }

    public interface IHasProviderEdge : IEdge { }

    public class HasResourceEdge : EdgeGrain<IncrementCommitmentGrain, ResourceGrain>, IHasResourceEdge { }

    public interface IHasResourceEdge : IEdge { }
}