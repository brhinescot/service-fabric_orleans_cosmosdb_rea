using System;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Orleans.Hosting;
using Orleans.Storage;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans.Providers;
using Microsoft.Extensions.Options;
using Orleans.Configuration;

namespace Orleans.Graph.StorageProvider
{
    public static class CosmosDbGraphSiloBuilderExtensions
    {
        /// <summary>
        /// Configure silo to use azure blob storage as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddCosmosDbGraphGrainStorageAsDefault(this ISiloHostBuilder builder, Action<CosmosDbGraphStorageOptions> configureOptions)
        {
            return builder.AddCosmosDbGraphGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use azure blob storage for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddCosmosDbGraphGrainStorage(this ISiloHostBuilder builder, string name, Action<CosmosDbGraphStorageOptions> configureOptions)
        {
            return builder.ConfigureServices(services => services.AddCosmosDbGraphGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Configure silo to use azure blob storage as the default grain storage.
        /// </summary>
        public static ISiloHostBuilder AddCosmosDbGraphGrainStorageAsDefault(this ISiloHostBuilder builder, Action<OptionsBuilder<CosmosDbGraphStorageOptions>> configureOptions = null)
        {
            return builder.AddCosmosDbGraphGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use azure blob storage for grain storage.
        /// </summary>
        public static ISiloHostBuilder AddCosmosDbGraphGrainStorage(this ISiloHostBuilder builder, string name, Action<OptionsBuilder<CosmosDbGraphStorageOptions>> configureOptions = null)
        {
            return builder.ConfigureServices(services => services.AddCosmosDbGraphGrainStorage(name, configureOptions));
        }

        /// <summary>
        /// Configure silo to use azure blob storage as the default grain storage.
        /// </summary>
        public static IServiceCollection AddCosmosDbGraphGrainStorageAsDefault(this IServiceCollection services, Action<CosmosDbGraphStorageOptions> configureOptions)
        {
            return services.AddCosmosDbGraphGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, ob => ob.Configure(configureOptions));
        }

        /// <summary>
        /// Configure silo to use azure blob storage for grain storage.
        /// </summary>
        public static IServiceCollection AddCosmosDbGraphGrainStorage(this IServiceCollection services, string name, Action<CosmosDbGraphStorageOptions> configureOptions)
        {
            return services.AddCosmosDbGraphGrainStorage(name, ob => ob.Configure(configureOptions));
        }

        /// <summary>
        /// Configure silo to use azure blob storage as the default grain storage.
        /// </summary>
        public static IServiceCollection AddCosmosDbGraphGrainStorageAsDefault(this IServiceCollection services, Action<OptionsBuilder<CosmosDbGraphStorageOptions>> configureOptions = null)
        {
            return services.AddCosmosDbGraphGrainStorage(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME, configureOptions);
        }

        /// <summary>
        /// Configure silo to use azure blob storage for grain storage.
        /// </summary>
        public static IServiceCollection AddCosmosDbGraphGrainStorage(this IServiceCollection services, string name,
            Action<OptionsBuilder<CosmosDbGraphStorageOptions>> configureOptions = null)
        {
            configureOptions?.Invoke(services.AddOptions<CosmosDbGraphStorageOptions>(name));
            services.AddTransient<IConfigurationValidator>(sp => new CosmosDbGraphStorageValidator(sp.GetService<IOptionsSnapshot<CosmosDbGraphStorageOptions>>().Get(name), name));
            services.ConfigureNamedOptionForLogging<CosmosDbGraphStorageOptions>(name);
            services.TryAddSingleton(sp => sp.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
            return services.AddSingletonNamedService(name, CosmosDbGraphStorageFactory.Create)
                .AddSingletonNamedService(name, (s, n) => (ILifecycleParticipant<ISiloLifecycle>) s.GetRequiredServiceByName<IGrainStorage>(n));
        }
    }
}