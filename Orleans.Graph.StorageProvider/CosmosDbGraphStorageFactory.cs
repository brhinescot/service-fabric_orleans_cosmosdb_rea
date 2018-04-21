using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Orleans.Configuration.Overrides;
using Orleans.Storage;

namespace Orleans.Graph.StorageProvider
{
    public static class CosmosDbGraphStorageFactory
    {
        public static IGrainStorage Create(IServiceProvider services, string name)
        {
            var optionsSnapshot = services.GetRequiredService<IOptionsSnapshot<CosmosDbGraphStorageOptions>>();
            var clusterOptions = services.GetProviderClusterOptions(name);
            return ActivatorUtilities.CreateInstance<CosmosDbGraphStorage>(services, name, optionsSnapshot.Get(name), clusterOptions);
        }
    }
}