#region Using Directives

using System;
using System.Diagnostics;
using System.Linq;
using Orleans.Graph.State;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Orleans.Graph
{
    public class UnitTest1
    {
        private readonly ITestOutputHelper output;

        public UnitTest1(ITestOutputHelper output)
        {
            this.output = output;
        }

        private static Guid ToGuid(int value)
        {
            var bytes = new byte[16];
            BitConverter.GetBytes(value).CopyTo(bytes, 0);
            return new Guid(bytes);
        }

        [Fact]
        public void TestBoolean()
        {
            const bool a = true;
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("true", c);

            GraphValue d = c;
            bool e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestDateTime()
        {
            var a = DateTime.Parse("2017-10-06T11:59:51.4023776Z");
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("'2017-10-06T11:59:51.4023776Z'", c);

            GraphValue d = c;
            DateTime e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestDecimal()
        {
            const decimal a = 123.456754567m;
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("123.456754567", c);

            GraphValue d = c;
            decimal e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestDouble()
        {
            const double a = 123.456754567;
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("123.456754567", c);

            GraphValue d = c;
            double e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestFloat()
        {
            const float a = 123.45f;
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("123.45", c);

            GraphValue d = c;
            float e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestGuid()
        {
            var a = Guid.ParseExact("1a189755-8322-443e-8cca-a5c4c83d6207", "D");
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("'1a189755-8322-443e-8cca-a5c4c83d6207'", c);

            GraphValue d = c;
            Guid e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestInt()
        {
            const int a = 123;
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("123", c);

            GraphValue d = c;
            int e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestLong()
        {
            const long a = 123456789012345;
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("123456789012345", c);

            GraphValue d = c;
            long e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void TestString()
        {
            const string a = "Seattle, WA";
            GraphValue b = a;

            var c = b.ToGraphString();
            Assert.Equal("'Seattle, WA'", c);

            GraphValue d = c;
            string e = d;

            Assert.Equal(a, e);
        }

        [Fact]
        public void BranchingCacheTest()
        {
            var matrix = new BranchingCache<int, int, int, int, int>();

            var total = 0;
            for (var first = 10; first <= 50; first += 10)
            for (var second = 10; second <= 50; second += 10)
            for (var third = 10; third <= 50; third += 10)
            for (var fourth = 10; fourth <= 50; fourth += 10)
            for (var fifth = 10; fifth <= 50; fifth += 10)
            {
                var wasSet = matrix.Set(first, second, third, fourth, fifth);
                if(!wasSet)
                    total++;
            }
            
            Assert.Equal(total, matrix.GetAll().Count());
            Assert.Equal(total/5, matrix.GetSubset(40).Count());
            Assert.Equal(total/25, matrix.GetSubset(40, 20).Count());
            Assert.Equal(total/125, matrix.GetSubset(40, 20, 10).Count());
            
            // This looks like a multidimensional directed matrix!
            Assert.Equal(50, matrix.Get(40, 20, 10, 50));
            Assert.Equal(50, matrix.Get(40, 30, 20, 50));
            Assert.Equal(50, matrix.Get(10, 10, 10, 50));
        }

        [Fact]
        public void BranchingCacheTest2()
        {
            var matrix = new BranchingCache<string, string, string, string, string, Restaurant>();

            matrix.Set("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland", new Restaurant("Shadowland"));
            matrix.Set("WA", "Seattle", "West Seattle", "Restaurant", "Luna Park", new Restaurant("Luna Park"));
            matrix.Set("WA", "Seattle", "West Seattle", "Restaurant", "Subway", new Restaurant("Subway"));
            matrix.Set("WA", "Seattle", "West Seattle", "Restaurant", "Big Mikes", new Restaurant("Big Mikes"));
            matrix.Set("WA", "Seattle", "West Seattle", "Restaurant", "Eat at Joe's", new Restaurant("Eat at Joe's"));
            matrix.Set("WA", "Seattle", "West Seattle", "Bar", "Nancy's", new Restaurant("Nancy's"));
            matrix.Set("WA", "Seattle", "West Seattle", "Bar", "The Old Mule", new Restaurant("The Old Mule"));
            matrix.Set("WA", "Seattle", "West Seattle", "Bar", "Frank's", new Restaurant("Frank's"));
            matrix.Set("WA", "Seattle", "West Seattle", "Bar", "Dan's", new Restaurant("Dan's"));
            matrix.Set("WA", "Seattle", "West Seattle", "Bar", "Hole in the Wall", new Restaurant("Hole in the Wall"));

            matrix.Set("WA", "Seattle", "Belltown", "Restaurant", "Shadowland", new Restaurant("Shadowland"));
            matrix.Set("WA", "Seattle", "Belltown", "Restaurant", "Clyde's", new Restaurant("Clyde's"));
            matrix.Set("WA", "Seattle", "Belltown", "Restaurant", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
            matrix.Set("WA", "Seattle", "Belltown", "Restaurant", "Tin Hammer", new Restaurant("Tin Hammer"));
            matrix.Set("WA", "Seattle", "Belltown", "Restaurant", "Bart's", new Restaurant("Bart's"));
            matrix.Set("WA", "Seattle", "Belltown", "Bar", "R Place", new Restaurant("R Place"));
            matrix.Set("WA", "Seattle", "Belltown", "Bar", "Linda's", new Restaurant("Linda's"));
            matrix.Set("WA", "Seattle", "Belltown", "Bar", "The Crocodile", new Restaurant("The Crocodile"));
            matrix.Set("WA", "Seattle", "Belltown", "Bar", "Substation", new Restaurant("Substation"));
            matrix.Set("WA", "Seattle", "Belltown", "Bar", "Blackies", new Restaurant("Blackies"));

            var enumerable = matrix.GetSubset("WA");
            Assert.Equal(20, enumerable.Count()); 
            
            var enumerable2 = matrix.GetSubset("WA", "Seattle", "Belltown");
            Assert.Equal(10, enumerable2.Count());

            var restaurants = matrix.GetAll();
            Assert.Equal(20, restaurants.Count());

            var merge = matrix.Merge("Shadowland");
            Assert.Equal(2, merge.Count());

            var restaurant = matrix.Get("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland");
            Assert.NotNull(restaurant);
            matrix.Remove("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland"); 
            restaurant = matrix.Get("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland");
            Assert.Null(restaurant);

            var subset = matrix.GetSubset("WA", "Seattle", "Belltown", "Restaurant");
            Assert.Equal(5, subset.Count());
            matrix.Prune("WA", "Seattle", "Belltown", "Restaurant");
            subset = matrix.GetSubset("WA", "Seattle", "Belltown", "Restaurant");
            Assert.Empty(subset);

            merge = matrix.Merge("Shadowland");
            Assert.Empty(merge);
        }

        [Fact]
        public void BranchingCacheTest3()
        {
            var matrix = new BranchingCache<string, string, string, string, string, Restaurant>(entity => (entity.State, entity.City, entity.Neighborhood, entity.Type, entity.Name));

            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Restaurant", "Luna Park"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Restaurant", "Subway"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Restaurant", "Big Mikes"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Restaurant", "Eat at Joe's"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Bar", "Nancy's"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Bar", "The Old Mule"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Bar", "Frank's"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Bar", "Dan's"));
            matrix.Set(new Restaurant("WA", "Seattle", "West Seattle", "Bar", "Hole in the Wall"));

            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Restaurant", "The Tavern"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Restaurant", "Clyde's"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Restaurant", "The Metropolitan Grill"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Restaurant", "Tin Hammer"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Restaurant", "Bart's"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Bar", "R Place"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Bar", "Linda's"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Bar", "The Crocodile"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Bar", "Substation"));
            matrix.Set(new Restaurant("WA", "Seattle", "Belltown", "Bar", "Blackies"));

            var enumerable = matrix.GetSubset("WA");
            Assert.Equal(20, enumerable.Count());

            var enumerable2 = matrix.GetSubset("WA", "Seattle", "Belltown");
            Assert.Equal(10, enumerable2.Count());

            var resturant = matrix.Get("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland");
            Assert.NotNull(resturant);
            matrix.Remove("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland");
            resturant = matrix.Get("WA", "Seattle", "West Seattle", "Restaurant", "Shadowland");
            Assert.Null(resturant);

            var subset = matrix.GetSubset("WA", "Seattle", "Belltown", "Restaurant");
            Assert.Equal(5, subset.Count());
            matrix.Prune("WA", "Seattle", "Belltown", "Restaurant");
            subset = matrix.GetSubset("WA", "Seattle", "Belltown", "Restaurant");
            Assert.Empty(subset);
        }

        [Fact]
        public void BranchingCacheMergeTest()
        {
            var matrix = new BranchedCache<string, string, Restaurant>();

            matrix.Set("West Seattle", "Shadowland", new Restaurant("Shadowland"));
            matrix.Set("West Seattle", "Luna Park", new Restaurant("Luna Park"));
            matrix.Set("West Seattle", "Subway", new Restaurant("Subway"));
            matrix.Set("West Seattle", "Big Mikes", new Restaurant("Big Mikes"));
            matrix.Set("West Seattle", "Eat at Joe's", new Restaurant("Eat at Joe's"));
            matrix.Set("Belltown", "Shadowland", new Restaurant("Shadowland"));
            matrix.Set("Belltown", "Clyde's", new Restaurant("Clyde's"));
            matrix.Set("Belltown", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
            matrix.Set("Belltown", "Tin Hammer", new Restaurant("Tin Hammer"));
            matrix.Set("Belltown", "Bart's", new Restaurant("Bart's"));

            var enumerable = matrix.GetSubset("West Seattle");
            Assert.Equal(5, enumerable.Count());
            var enumerable2 = matrix.GetSubset("Belltown");
            Assert.Equal(5, enumerable2.Count());

            var enumerable3 = matrix.Merge("Shadowland");
            Assert.Equal(2, enumerable3.Count());
            
            var enumerable4 = matrix.Merge("Luna Park");
            Assert.Single(enumerable4);
        }

        [Fact]
        public void BranchingCacheMergeTest2()
        {
            var matrix = new BranchingCache<string, string, string, Restaurant>();

            matrix.Set("Seattle", "West Seattle", "Shadowland", new Restaurant("Shadowland"));
            matrix.Set("Seattle", "West Seattle", "Luna Park", new Restaurant("Luna Park"));
            matrix.Set("Seattle", "West Seattle", "Subway", new Restaurant("Subway"));
            matrix.Set("Seattle", "West Seattle", "Big Mikes", new Restaurant("Big Mikes"));
            matrix.Set("Seattle", "West Seattle", "Eat at Joe's", new Restaurant("Eat at Joe's"));
            matrix.Set("Seattle", "Belltown", "Shadowland", new Restaurant("Shadowland"));
            matrix.Set("Seattle", "Belltown", "Clyde's", new Restaurant("Clyde's"));
            matrix.Set("Seattle", "Belltown", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
            matrix.Set("Seattle", "Belltown", "Tin Hammer", new Restaurant("Tin Hammer"));
            matrix.Set("Seattle", "Belltown", "Bart's", new Restaurant("Bart's"));
            
            var enumerable3 = matrix.Merge("Shadowland");
            Assert.Equal(2, enumerable3.Count());
            
            var enumerable4 = matrix.Merge("Luna Park");
            Assert.Single(enumerable4);
        }
        
        [Fact]
        public void BranchingCacheMergePerformanceTest()
        {
            var matrix = new BranchingCache<string, string, string, Restaurant>();
            for (var j = 0; j < 500; j++)
            {

                matrix.Set($"Seattle-{j}", "West Seattle", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Seattle-{j}", "West Seattle", "Luna Park", new Restaurant("Luna Park"));
                matrix.Set($"Seattle-{j}", "West Seattle", "Subway", new Restaurant("Subway"));
                matrix.Set($"Seattle-{j}", "West Seattle", "Big Mikes", new Restaurant("Big Mikes"));
                matrix.Set($"Seattle-{j}", "West Seattle", "Eat at Joe's", new Restaurant("Eat at Joe's"));
                matrix.Set($"Seattle-{j}", "Belltown", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Seattle-{j}", "Belltown", "Clyde's", new Restaurant("Clyde's"));
                matrix.Set($"Seattle-{j}", "Belltown", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
                matrix.Set($"Seattle-{j}", "Belltown", "Tin Hammer", new Restaurant("Tin Hammer"));
                matrix.Set($"Seattle-{j}", "Belltown", "Bart's", new Restaurant("Bart's"));
                matrix.Set($"Olympia-{j}", "West Seattle", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Olympia-{j}", "West Seattle", "Luna Park", new Restaurant("Luna Park"));
                matrix.Set($"Olympia-{j}", "West Seattle", "Subway", new Restaurant("Subway"));
                matrix.Set($"Olympia-{j}", "West Seattle", "Big Mikes", new Restaurant("Big Mikes"));
                matrix.Set($"Olympia-{j}", "West Seattle", "Eat at Joe's", new Restaurant("Eat at Joe's"));
                matrix.Set($"Olympia-{j}", "Belltown", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Olympia-{j}", "Belltown", "Clyde's", new Restaurant("Clyde's"));
                matrix.Set($"Olympia-{j}", "Belltown", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
                matrix.Set($"Olympia-{j}", "Belltown", "Tin Hammer", new Restaurant("Tin Hammer"));
                matrix.Set($"Olympia-{j}", "Belltown", "Bart's", new Restaurant("Bart's"));
                
                matrix.Set($"Tacoma-{j}", "West Seattle", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Tacoma-{j}", "West Seattle", "Luna Park", new Restaurant("Luna Park"));
                matrix.Set($"Tacoma-{j}", "West Seattle", "Subway", new Restaurant("Subway"));
                matrix.Set($"Tacoma-{j}", "West Seattle", "Big Mikes", new Restaurant("Big Mikes"));
                matrix.Set($"Tacoma-{j}", "West Seattle", "Eat at Joe's", new Restaurant("Eat at Joe's"));
                matrix.Set($"Tacoma-{j}", "Belltown", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Tacoma-{j}", "Belltown", "Clyde's", new Restaurant("Clyde's"));
                matrix.Set($"Tacoma-{j}", "Belltown", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
                matrix.Set($"Tacoma-{j}", "Belltown", "Tin Hammer", new Restaurant("Tin Hammer"));
                matrix.Set($"Tacoma-{j}", "Belltown", "Bart's", new Restaurant("Bart's"));
                matrix.Set($"Everet-{j}", "West Seattle", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Everet-{j}", "West Seattle", "Luna Park", new Restaurant("Luna Park"));
                matrix.Set($"Everet-{j}", "West Seattle", "Subway", new Restaurant("Subway"));
                matrix.Set($"Everet-{j}", "West Seattle", "Big Mikes", new Restaurant("Big Mikes"));
                matrix.Set($"Everet-{j}", "West Seattle", "Eat at Joe's", new Restaurant("Eat at Joe's"));
                matrix.Set($"Everet-{j}", "Belltown", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Everet-{j}", "Belltown", "Clyde's", new Restaurant("Clyde's"));
                matrix.Set($"Everet-{j}", "Belltown", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
                matrix.Set($"Everet-{j}", "Belltown", "Tin Hammer", new Restaurant("Tin Hammer"));
                matrix.Set($"Everet-{j}", "Belltown", "Bart's", new Restaurant("Bart's"));
                matrix.Set($"Everet-{j}", "Downtown", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Everet-{j}", "Downtown", "Luna Park", new Restaurant("Luna Park"));
                matrix.Set($"Everet-{j}", "Downtown", "Subway", new Restaurant("Subway"));
                matrix.Set($"Everet-{j}", "Downtown", "Big Mikes", new Restaurant("Big Mikes"));
                matrix.Set($"Everet-{j}", "Downtown", "Eat at Joe's", new Restaurant("Eat at Joe's"));
                matrix.Set($"Everet-{j}", "The Glades", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Everet-{j}", "The Glades", "Clyde's", new Restaurant("Clyde's"));
                matrix.Set($"Everet-{j}", "The Glades", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
                matrix.Set($"Everet-{j}", "The Glades", "Tin Hammer", new Restaurant("Tin Hammer"));
                matrix.Set($"Everet-{j}", "The Glades", "Bart's", new Restaurant("Bart's"));
                matrix.Set($"Everet-{j}", "Mid Mountain", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Everet-{j}", "Mid Mountain", "Luna Park", new Restaurant("Luna Park"));
                matrix.Set($"Everet-{j}", "Mid Mountain", "Subway", new Restaurant("Subway"));
                matrix.Set($"Everet-{j}", "Mid Mountain", "Big Mikes", new Restaurant("Big Mikes"));
                matrix.Set($"Everet-{j}", "Mid Mountain", "Eat at Joe's", new Restaurant("Eat at Joe's"));
                matrix.Set($"Everet-{j}", "Oceanside", "Shadowland", new Restaurant("Shadowland"));
                matrix.Set($"Everet-{j}", "Oceanside", "Clyde's", new Restaurant("Clyde's"));
                matrix.Set($"Everet-{j}", "Oceanside", "The Metropolitan Grill", new Restaurant("The Metropolitan Grill"));
                matrix.Set($"Everet-{j}", "Oceanside", "Tin Hammer", new Restaurant("Tin Hammer"));
                matrix.Set($"Everet-{j}", "Oceanside", "Bart's", new Restaurant("Bart's"));
            }

            matrix.Set($"Everet-{1000}", "Oceanside", "Tim Horton's", new Restaurant("Bart's"));

            // Warm up the code;
            var sw = Stopwatch.StartNew();
            var count1 = matrix.Count();
            output.WriteLine($"Searching through {count1} total items\r\n");
            var durationInTicks = sw.ElapsedTicks;
            var microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L * 1000L));
            var calculatedMilliseconds = ((decimal)microseconds / 1000).ToString("f3");

            // Cold Run
            var enumerable3 = matrix.Merge("Shadowland");

            sw.Reset();
            sw.Start();
            var i = enumerable3.Count();
            sw.Stop();

            durationInTicks = sw.ElapsedTicks;
            microseconds = durationInTicks / (Stopwatch.Frequency / (1000L * 1000L));
            calculatedMilliseconds = ((decimal)microseconds /1000).ToString("f3");
            
            output.WriteLine("Cold Start \r\n*****************************************************");
            output.WriteLine($"ElapsedMilliseconds: \t\t{sw.ElapsedMilliseconds}");
            output.WriteLine($"Calculated Milliseconds: \t{calculatedMilliseconds}");
            output.WriteLine($"Calculated Microseconds: \t{microseconds}");
            output.WriteLine($"ElapsedTicks: \t\t\t\t{durationInTicks}");
            
            output.WriteLine("");

            // Warm Run
            enumerable3 = matrix.Merge("Shadowland");

            sw.Reset();
            sw.Start();
            var count = enumerable3.Count();
            sw.Stop();

            durationInTicks = sw.ElapsedTicks;
            microseconds = durationInTicks / (Stopwatch.Frequency / (1000L * 1000L));
            calculatedMilliseconds = ((decimal)microseconds / 1000).ToString("f3");
            
            output.WriteLine("Warm Start \r\n*****************************************************");
            output.WriteLine($"ElapsedMilliseconds: \t\t{sw.ElapsedMilliseconds}");
            output.WriteLine($"Calculated Milliseconds: \t{calculatedMilliseconds}");
            output.WriteLine($"Calculated Microseconds: \t{microseconds}");
            output.WriteLine($"ElapsedTicks: \t\t\t\t{durationInTicks}");

        }

        [DebuggerDisplay("{" + nameof(Name) + "}")]
        private class Restaurant
        {
            public Restaurant(string name)
            {
                Name = name;
            }

            public Restaurant(string state, string city, string neighborhood, string type, string name)
            {
                State = state;
                City = city;
                Neighborhood = neighborhood;
                Type = type;
                Name = name;
            }

            public string State { get; set; }
            public string City { get; set; }
            public string Neighborhood { get; set; }
            public string Type { get; set; }
            public string Name { get; set; }
        }
    }
}