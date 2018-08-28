#region Using Directives

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Orleans;
using Orleans.Graph;
using Orleans.Graph.Test.Definition;
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

            services.AddLogging(builder =>
            {
                builder.AddEventSourceLogger()
                    .AddDebug()
                    .AddConsole();
            });
            
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var jwtSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                Configuration.GetValue<string>("JWT_SECRET_KEY")));

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],
                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = jwtSigningKey,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.IncludeErrorDetails = true;
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddMvc().AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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

            services.AddScoped(provider =>
            {
                var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
                var organizationClaim = context.User.FindFirst(claim => claim.Type == "Organization");

                var clusterClient = provider.GetRequiredService<IClusterClient>();
                return clusterClient.GetVertexGrain<IOrganization>(Guid.ParseExact("c94ed5be-a7a7-4bff-af0c-adcdac615908", "D"), "partition0");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRequestTracking();
            app.UseRequestTimer();
            app.UseHealthCheck();

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

    
    public class JwtIssuerOptions
    {
        /// <summary>
        ///     "iss" (Issuer) Claim
        /// </summary>
        /// <remarks>
        ///     The "iss" (issuer) claim identifies the principal that issued the
        ///     JWT.  The processing of this claim is generally application specific.
        ///     The "iss" value is a case-sensitive string containing a StringOrURI
        ///     value.  Use of this claim is OPTIONAL.
        /// </remarks>
        public string Issuer { get; set; }

        /// <summary>
        ///     "sub" (Subject) Claim
        /// </summary>
        /// <remarks>
        ///     The "sub" (subject) claim identifies the principal that is the
        ///     subject of the JWT.  The claims in a JWT are normally statements
        ///     about the subject.  The subject value MUST either be scoped to be
        ///     locally unique in the context of the issuer or be globally unique.
        ///     The processing of this claim is generally application specific.  The
        ///     "sub" value is a case-sensitive string containing a StringOrURI
        ///     value.  Use of this claim is OPTIONAL.
        /// </remarks>
        public string Subject { get; set; }

        /// <summary>
        ///     "aud" (Audience) Claim
        /// </summary>
        /// <remarks>
        ///     The "aud" (audience) claim identifies the recipients that the JWT is
        ///     intended for.  Each principal intended to process the JWT MUST
        ///     identify itself with a value in the audience claim.  If the principal
        ///     processing the claim does not identify itself with a value in the
        ///     "aud" claim when this claim is present, then the JWT MUST be
        ///     rejected.  In the general case, the "aud" value is an array of case-
        ///     sensitive strings, each containing a StringOrURI value.  In the
        ///     special case when the JWT has one audience, the "aud" value MAY be a
        ///     single case-sensitive string containing a StringOrURI value.  The
        ///     interpretation of audience values is generally application specific.
        ///     Use of this claim is OPTIONAL.
        /// </remarks>
        public string Audience { get; set; }

        /// <summary>
        ///     "nbf" (Not Before) Claim (default is UTC NOW)
        /// </summary>
        /// <remarks>
        ///     The "nbf" (not before) claim identifies the time before which the JWT
        ///     MUST NOT be accepted for processing.  The processing of the "nbf"
        ///     claim requires that the current date/time MUST be after or equal to
        ///     the not-before date/time listed in the "nbf" claim.  Implementers MAY
        ///     provide for some small leeway, usually no more than a few minutes, to
        ///     account for clock skew.  Its value MUST be a number containing a
        ///     NumericDate value.  Use of this claim is OPTIONAL.
        /// </remarks>
        public DateTime NotBefore => DateTime.UtcNow;

        /// <summary>
        ///     "iat" (Issued At) Claim (default is UTC NOW)
        /// </summary>
        /// <remarks>
        ///     The "iat" (issued at) claim identifies the time at which the JWT was
        ///     issued.  This claim can be used to determine the age of the JWT.  Its
        ///     value MUST be a number containing a NumericDate value.  Use of this
        ///     claim is OPTIONAL.
        /// </remarks>
        public DateTime IssuedAt => DateTime.UtcNow;

        /// <summary>
        ///     Set the timespan the token will be valid for (default is 5 min/300 seconds)
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        ///     "exp" (Expiration Time) Claim (returns IssuedAt + ValidFor)
        /// </summary>
        /// <remarks>
        ///     The "exp" (expiration time) claim identifies the expiration time on
        ///     or after which the JWT MUST NOT be accepted for processing.  The
        ///     processing of the "exp" claim requires that the current date/time
        ///     MUST be before the expiration date/time listed in the "exp" claim.
        ///     Implementers MAY provide for some small leeway, usually no more than
        ///     a few minutes, to account for clock skew.  Its value MUST be a number
        ///     containing a NumericDate value.  Use of this claim is OPTIONAL.
        /// </remarks>
        public DateTime Expiration => IssuedAt.Add(ValidFor);

        /// <summary>
        ///     "jti" (JWT ID) Claim (default ID is a GUID)
        /// </summary>
        /// <remarks>
        ///     The "jti" (JWT ID) claim provides a unique identifier for the JWT.
        ///     The identifier value MUST be assigned in a manner that ensures that
        ///     there is a negligible probability that the same value will be
        ///     accidentally assigned to a different data object; if the application
        ///     uses multiple issuers, collisions MUST be prevented among values
        ///     produced by different issuers as well.  The "jti" claim can be used
        ///     to prevent the JWT from being replayed.  The "jti" value is a case-
        ///     sensitive string.  Use of this claim is OPTIONAL.
        /// </remarks>
        public Func<Task<string>> JtiGenerator =>
            () => Task.FromResult(Guid.NewGuid().ToString());

        /// <summary>
        ///     The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}