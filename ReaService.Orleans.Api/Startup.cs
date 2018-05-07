using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Clustering.ServiceFabric;
using Orleans.Configuration;
using Orleans.Graph.Definition;
using Orleans.Graph.Test.Definition;
using ReaService.Orleans.Definition;

namespace ReaService.Orleans.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddOrleansClient("fabric:/ReaService/ReaService.Orleans.Host", "development");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }

    public static class OrleansServiceExtensions
    {
        public static void AddOrleansClient(this IServiceCollection services, string serviceName, string clusterId)
        {
            Uri serviceUri = new Uri(serviceName);

            IClusterClient client = new ClientBuilder()
                .Configure<ClusterOptions>(options =>
                {
                    options.ServiceId = serviceUri.ToString();
                    options.ClusterId = clusterId;
                })
                .UseServiceFabricClustering(serviceUri)
                .ConfigureLogging(logging => logging.AddDebug())
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(IAgentGrain).Assembly);
                    parts.AddApplicationPart(typeof(IVertexGrain).Assembly);
                    parts.AddApplicationPart(typeof(IPersonVertex).Assembly);
                })
                .Build();

            client.Connect().GetAwaiter().GetResult();

            services.AddSingleton(client);
        }
    }
}
