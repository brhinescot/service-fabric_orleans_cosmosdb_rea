#region Using Directives

using System;
using System.Threading.Tasks;
using Orleans.Graph.Definition;

#endregion

namespace ReaService.Orleans.Definition
{
    public interface IIncrementCommitment : IVertex
    {
        Task Fulfill();

        /// <exception cref="ArgumentNullException"><paramref name="provider" /> is <see langword="null" /></exception>
        /// <exception cref="ArgumentNullException"><paramref name="resource" /> is <see langword="null" /></exception>
        Task Initialize(IAgent provider, IResource resource, double amount);
    }
}