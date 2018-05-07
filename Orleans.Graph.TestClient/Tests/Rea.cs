using System;
using System.Threading.Tasks;
using Orleans.Graph.TestClient.Runner;
using ReaService.Orleans.Definition;

namespace Orleans.Graph.TestClient.Tests
{
    [TestFixture]
    public class Rea
    {
        [Test(Iterations = 100)]
        public async Task IncrementCommitment(IClusterClient client, int partitionNumber, int iteration)
        {
            IAgentGrain agent = client.GetVertexGrain<IAgentGrain>(Guid.NewGuid(), "agents");
            IResourceGrain resource = client.GetVertexGrain<IResourceGrain>(Guid.NewGuid(), "resources");

            IIncrementCommitmentGrain incrementEvent = client.GetVertexGrain<IIncrementCommitmentGrain>(Guid.NewGuid(), "events");

            await incrementEvent.Initialize(agent, resource, 20);

            Console.WriteLine($"    ==> IIncrementCommitmentGrain.Initialize(agent, resource, 20)");
        }
    }
}