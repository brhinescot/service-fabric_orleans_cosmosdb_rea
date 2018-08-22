#region Using Directives

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReaService.Orleans.Api.Middleware;
using Swashbuckle.AspNetCore.Swagger;

#endregion

namespace ReaService.Orleans.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CORSAllowAllPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .Build();
                });
            });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(options =>
            {
                const string swaggerDocsResourceName = "ReaService.Orleans.Api.Resources.SwaggerDocs.xml";

                options.IncludeEmbeddedXmlComments(swaggerDocsResourceName);
                options.DescribeAllEnumsAsStrings();
                options.DescribeAllParametersInCamelCase();
                options.IgnoreObsoleteActions();
                options.IgnoreObsoleteProperties();
                options.EnableAnnotations();
                options.SwaggerDoc("v1", new Info
                {
                    Title = "REA Service API",
                    Version = "v1",
                    Description = "A REA Service.",
                    TermsOfService = "https://<url>.com/rea/terms",
                    Contact = new Contact
                    {
                        Email = "contact@<url>.com",
                        Name = "REA Service",
                        Url = "https://<url>.com"
                    }
                });
            });

            services.AddOrleansClient("fabric:/ReaService/ReaService.Orleans.Host", "REA_Graph");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseCors("CORSAllowAllPolicy");

            app.UseMvc();

            app.UseSwagger(options =>
            {
                options.RouteTemplate = "docs/{documentName}/openApi.json";
                options.UseLowerCasedPaths();
            });

            app.UseSwaggerUI(options =>
            {
                options.DocumentTitle = "REA Service OpenApi Documentation";
                options.RoutePrefix = "docs";
                options.HeadContent = "<div class=\"swagger-ui\"><h1>DevInterop</h1></div>";
                options.SwaggerEndpoint("/docs/v1/openApi.json", "REA Service API V1");
                options.EnableDeepLinking();
                options.DisplayRequestDuration();
                options.EnableFilter();
            });
        }
    }
}