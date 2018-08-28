#region Using Directives

using System.Fabric;
using Microsoft.Extensions.Configuration;

#endregion

namespace ReaService
{
    public static class ServiceFabricConfigurationExtensions
    {
        public static IConfigurationBuilder AddServiceFabricConfiguration(this IConfigurationBuilder builder, ServiceContext serviceContext)
        {
            builder.Add(new ServiceFabricConfigurationSource(serviceContext));
            return builder;
        }
    }
}