#region Using Directives

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

#endregion

namespace ReaService.Orleans.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services, IHostingEnvironment env)
        {
            services.AddMvc();

            string clusterid = env.IsDevelopment() 
                ? "development" 
                : throw new NotImplementedException("Non development deployment. Please configure the Orleans client ClusterId.");

            services.AddOrleansClient("fabric:/ReaService/ReaService.Orleans.Host", clusterid);
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
}
