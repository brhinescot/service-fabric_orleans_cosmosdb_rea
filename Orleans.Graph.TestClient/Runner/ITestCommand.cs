#region Using Directives

using System.Threading.Tasks;

#endregion

namespace Orleans.Graph.TestClient.Runner
{
    internal interface ITestCommand
    {
        Task Execute(IClusterClient client, int partitionNumber, int iteration);
    }
}