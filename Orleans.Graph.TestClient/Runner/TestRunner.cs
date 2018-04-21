#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

#endregion

namespace Orleans.Graph.TestClient.Runner
{
    internal class TestRunner
    {
        #region Member Fields

        private readonly Dictionary<string, TestClassCommand> testClassCommands = new Dictionary<string, TestClassCommand>();

        #endregion

        public void Initialize()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                TestFixtureAttribute fixtureAttribute = type.GetCustomAttribute<TestFixtureAttribute>();
                if (fixtureAttribute == null)
                    continue;
                
                object fixtureInstance = Activator.CreateInstance(type);
                TestClassCommand classCommand = new TestClassCommand(fixtureInstance);
                
                foreach (MethodInfo method in type.GetMethods())
                {
                    TestAttribute testAttribute = method.GetCustomAttribute<TestAttribute>();
                    if (testAttribute == null)
                        continue;

                    TestMethodCommand methodCommand = new TestMethodCommand(method)
                    {
                        Iterations = testAttribute.Iterations,
                        PartitionCount = testAttribute.PartitionCount
                    };

                    string methodName = method.Name.Replace("Tests", string.Empty).Replace("Test", string.Empty).ToLower();
                    classCommand.AddTestMethodCommand(methodName, methodCommand);
                }

                testClassCommands.Add(type.Name.ToLower(), classCommand);
            }
        }

        public IEnumerable<string> GetTestClassNames()
        {
            return testClassCommands.Keys;
        }

        public ITestCommand Parse(string input)
        {
            var parsed = input.Split(' ');

            switch (parsed.Length)
            {
                case 1:
                    testClassCommands.TryGetValue(parsed[0], out TestClassCommand testClassCommand);
                    return testClassCommand;
                case 2 when testClassCommands.TryGetValue(parsed[0], out TestClassCommand testMethodCommand):
                    return testMethodCommand.Parse(parsed[1]);
                default:
                    return null;
            }
        }

        private class TestClassCommand : ITestCommand
        {
            private readonly object testClassInstance;

            #region Member Fields

            private readonly Dictionary<string, TestMethodCommand> testMethodCommands = new Dictionary<string, TestMethodCommand>();

            #endregion

            public TestClassCommand(object testClassInstance)
            {
                this.testClassInstance = testClassInstance;
            }

            public void AddTestMethodCommand(string name, TestMethodCommand command)
            {
                command.TestClassInstance = testClassInstance;
                testMethodCommands.Add(name, command);
            }

            public async Task Execute(IClusterClient client, int partitionNumber, int iteration)
            {
                await Task.WhenAll(testMethodCommands.Select(test => test.Value.Execute(client, partitionNumber, iteration)));
            }

            public ITestCommand Parse(string testMethodName)
            {
                testMethodCommands.TryGetValue(testMethodName, out TestMethodCommand command);
                return command;
            }
        }

        private class TestMethodCommand : ITestCommand
        {
            public int Iterations { get; set; }
            public int PartitionCount { get; set; }
            public object TestClassInstance { get; set; }
                
            #region Member Fields

            private readonly MethodInfo test;

            #endregion

            public TestMethodCommand(MethodInfo test)
            {
                this.test = test;
            }

            public async Task Execute(IClusterClient client, int partitionNumber, int iteration)
            {
                await (Task) test.Invoke(TestClassInstance, new object[] {client, partitionNumber, iteration});
            }
        }
    }
}