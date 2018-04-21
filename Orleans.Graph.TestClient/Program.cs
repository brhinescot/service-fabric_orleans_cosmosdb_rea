#region Using Directives

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Clustering.ServiceFabric;
using Orleans.Configuration;
using Orleans.Graph.Definition;
using Orleans.Graph.Test;
using Orleans.Graph.Test.Definition;
using Orleans.Graph.TestClient.Runner;
using Orleans.Graph.Vertex;
using Orleans.Runtime;
using ReaService.Orleans;
using ReaService.Orleans.Definition;

#endregion

namespace Orleans.Graph.TestClient
{
    internal static class Program
    {
        private static readonly TestRunner Runner = new TestRunner();
        
        private static void Main(string[] args)
        {
            Runner.Initialize();
            
            Run(args).Wait();
        }

        private static async Task Run(string[] args)
        {
            SetRequestContextData();
            WriteWelcome();
            
            IClusterClient client = await Connect();

            WriteReadyMessage();
            
            await StartProcessingLoop(client);
        }

        private static async Task StartProcessingLoop(IClusterClient client)
        {
            while (true)
            {
                WriteTestPrompt();

                string input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                    continue;

                input = input.ToLower();
                
                if (input == "cls")
                {
                    Console.Clear();
                    continue;
                }
                if (input == "?")
                {
                    Console.WriteLine("==> Available Test Classes");
                    foreach (string className in Runner.GetTestClassNames())
                        Console.WriteLine("  ==> " + className);

                    continue;
                }
                if (input == "exit")
                    break;

                ITestCommand command = Runner.Parse(input);
                await RunTest(client, command);
            }
        }

        private static void WriteTestPrompt()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("test> ");
            Console.ResetColor();
        }

        private static async Task RunTest(IClusterClient client, ITestCommand command)
        {
            int iteration = 0;
            int batch = 1;
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (true)
            {
                #region Handle Stop

                if (Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    Console.CursorLeft = 0;
                    Console.WriteLine("==> Exiting stress test.");
                    break;
                }

                #endregion

                int partitionNumber = iteration % 100;

                await command.Execute(client, partitionNumber, iteration);

                if (partitionNumber == 99)
                {
                    long milliseconds = stopwatch.ElapsedMilliseconds;
                    Console.WriteLine($"==> Batch {batch++}: Average completion time of last 100 tasks: {milliseconds / 100}ms.");
                    stopwatch.Restart();
                }

                iteration++;
            }

            stopwatch.Stop();
        }

        private static void SetRequestContextData()
        {
            RequestContext.ActivityId = Guid.NewGuid();
            RequestContext.PropagateActivityId = true;
            RequestContext.Set("User", "brian@fakeemail.fake");
        }

        private static void WriteReadyMessage()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Connection to local cluster was successful. Ready to run tests.");
            Console.WriteLine();
            Console.ResetColor();
        }

        private static async Task<IClusterClient> Connect()
        {
            Uri serviceName = new Uri("fabric:/ReaService/ReaService.Orleans.Host");

            var builder = new ClientBuilder();

            builder.Configure<ClusterOptions>(options =>
            {
                options.ServiceId = serviceName.ToString();
                options.ClusterId = "development";
            });

            // TODO: Pick a clustering provider and configure it here.
            builder.UseServiceFabricClustering(serviceName);

            // Add the application assemblies.
            builder.ConfigureApplicationParts(parts =>
            {
                parts.AddApplicationPart(typeof(IAgentGrain).Assembly);
                parts.AddApplicationPart(typeof(IVertexGrain).Assembly);
                parts.AddApplicationPart(typeof(IPersonVertex).Assembly);
            });

            // Optional: configure logging.
            builder.ConfigureLogging(logging => logging.AddDebug());

            // Create the client and connect to the cluster.
            var client = builder.Build();
            await client.Connect();
            return client;
        }

        private static void WriteWelcome()
        {
            Console.WriteLine("Ready to connect to local Service Fabric Orleans Silo. Press [Enter] when ready.");
            Console.ReadKey();

            Console.Clear();
            Console.WriteLine("Connecting to cluster, Please wait....");
        }
    }
}