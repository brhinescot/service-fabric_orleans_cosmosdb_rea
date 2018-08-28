#region Using Directives

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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
                options.AddPolicy("AllowAllPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .Build();
                });
            });

            services.AddLogging(builder =>
            {
                builder.AddEventSourceLogger()
                    .AddDebug()
                    .AddConsole();
            });
            
            var jwtOptions = Configuration.GetSection(nameof(JwtOptions));
            var jwtSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                Configuration.GetValue<string>("REA_JWT_SECRET_KEY")));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = jwtOptions[nameof(JwtOptions.Issuer)],
                        ValidAudience = jwtOptions[nameof(JwtOptions.Audience)],
                        IssuerSigningKey = jwtSigningKey
                    };
                    options.Validate();
                });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddOrleans("fabric:/ReaService/ReaService.Orleans.Host", "REA_Graph");

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
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRequestTracking();
            app.UseRequestTimer();
            app.UseHealthCheck();

            app.UseCors("AllowAllPolicy");
            app.UseAuthentication();
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
                options.SwaggerEndpoint("/docs/v1/openApi.json", "REA Service API V1");
                options.EnableDeepLinking();
                options.DisplayRequestDuration();
                options.EnableFilter();
            });
        }
    }
}