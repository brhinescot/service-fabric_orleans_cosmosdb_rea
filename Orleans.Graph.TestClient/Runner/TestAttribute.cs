using System;

namespace Orleans.Graph.TestClient.Runner
{
    public class TestAttribute : Attribute
    {
        public int Iterations { get; set; } = 100;

        public int PartitionCount { get; set; } = 100;
    }
}