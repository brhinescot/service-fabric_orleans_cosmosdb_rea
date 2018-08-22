#region Using Directives

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Orleans.Graph.Test.Definition;
using Orleans.Graph.TestClient.Runner;

#endregion

namespace Orleans.Graph.TestClient.Tests
{
    [TestFixture]
    public class Stress
    {
        [Test(Iterations = 100)]
        public async Task Profile(IClusterClient client, int partitionNumber, int iteration)
        {
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var primaryKey = Guid.NewGuid();
                IPersonVertex person = client.GetVertexGrain<IPersonVertex>(primaryKey, "partition" + partitionNumber);
            
                stopwatch.Restart();
                await person.SetPersonalDataAsync(new PersonalData($"Person{iteration}FirstName", $"Person{iteration}LastName")
                {
                    Birthdate = DateTime.Parse("10/02/1982")
                });
                Console.WriteLine($"    ==> SetPersonalDataAsync: {stopwatch.ElapsedMilliseconds}ms");
            
                stopwatch.Restart();
                IProfileVertex personalProfileVertex = await person.AddProfileAsync(new ProfileData {Name = "PersonalProfile"});
                Console.WriteLine($"    ==> Add Personal Profile: {stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Restart();
                IProfileVertex businessProfileVertex = await person.AddProfileAsync(new ProfileData {Name = "BusinessProfile"});
                Console.WriteLine($"    ==> Add Business Profile: {stopwatch.ElapsedMilliseconds}ms");

                Console.WriteLine();

                stopwatch.Restart();

                person = client.GetVertexGrain<IPersonVertex>(primaryKey, "partition" + partitionNumber);
                var personalDataAsync = await person.GetPersonalDataAsync();

                ProfileData personalProfileData = await personalProfileVertex.GetProfileDataAsync();
                Console.WriteLine($"        ==> Get Personal Profile: {stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Restart();
                ProfileData businessProfileData = await businessProfileVertex.GetProfileDataAsync();
                Console.WriteLine($"        ==> Get Business Profile: {stopwatch.ElapsedMilliseconds}ms");

                stopwatch.Stop();
                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine($"==> Caught excpetion {ex.Message}");
                Console.WriteLine();
                throw;
            }
        }

//        [Test(Iterations = 100)]
//        public async Task Person(IClusterClient client, int partitionNumber, int iteration)
//        {
//            IPersonVertex person = client.GetVertexGrain<IPersonVertex>(Guid.NewGuid(), "partition" + partitionNumber);
//            await person.SetPersonalDataAsync(new PersonalData
//            {
//                FirstName = $"Person{iteration}FirstName",
//                LastName = $"Person{iteration}LastName",
//                Birthdate = DateTime.Parse("10/02/1982")
//            });
//        }
    }
}