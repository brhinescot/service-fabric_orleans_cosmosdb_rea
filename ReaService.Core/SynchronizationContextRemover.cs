#region Using Directives

using System;
using System.Fabric;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Configuration;

#endregion

namespace ReaService
{
    public struct SynchronizationContextRemover : INotifyCompletion
    {
        public bool IsCompleted => SynchronizationContext.Current == null;

        public static SynchronizationContextRemover Instance { get; } = new SynchronizationContextRemover();
        
        public void OnCompleted(Action continuation)
        {
            var prevContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);
                continuation();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevContext);
            }
        }

        public SynchronizationContextRemover GetAwaiter()
        {
            return this;
        }

        public void GetResult() { }
    }

    internal class ServiceFabricConfigurationProvider : ConfigurationProvider
    {
        private readonly ServiceContext serviceContext;

        public ServiceFabricConfigurationProvider(ServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
        }

        public override void Load()
        {
            var config = serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            foreach (var section in config.Settings.Sections)
            foreach (var parameter in section.Parameters)
                Data[$"{section.Name}{ConfigurationPath.KeyDelimiter}{parameter.Name}"] = parameter.Value;
        }
    }

    internal class ServiceFabricConfigurationSource : IConfigurationSource
    {
        private readonly ServiceContext serviceContext;

        public ServiceFabricConfigurationSource(ServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServiceFabricConfigurationProvider(serviceContext);
        }
    }

    public static class ServiceFabricConfigurationExtensions
    {
        public static IConfigurationBuilder AddServiceFabricConfiguration(this IConfigurationBuilder builder, ServiceContext serviceContext)
        {
            builder.Add(new ServiceFabricConfigurationSource(serviceContext));
            return builder;
        }
    }
}