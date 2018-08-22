#region Using Directives

using System;
using System.Threading.Tasks;
using Orleans.Graph.TestClient.Runner;
using ReaService.Orleans.Definition;

#endregion

namespace Orleans.Graph.TestClient.Tests
{
    [TestFixture]
    public class Rea
    {
        [Test(Iterations = 100)]
        public async Task IncrementCommitment(IClusterClient client, int partitionNumber, int iteration)
        {
            var agent = client.GetVertexGrain<IAgent>(Guid.NewGuid(), "agents");
            var resource = client.GetVertexGrain<IResource>(Guid.NewGuid(), "resources");

            var incrementEvent = client.GetVertexGrain<IIncrementCommitment>(Guid.NewGuid(), "events");

            await incrementEvent.Initialize(agent, resource, 20);

            Console.WriteLine("    ==> IIncrementCommitment.Initialize(agent, resource, 20)");
        }
    }
}