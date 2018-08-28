#region Using Directives

using System.Fabric;
using Microsoft.Extensions.Configuration;

#endregion

namespace ReaService
{
    internal class ServiceFabricConfigurationProvider : ConfigurationProvider
    {
        #region Member Fields

        private readonly ServiceContext serviceContext;

        #endregion

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
}