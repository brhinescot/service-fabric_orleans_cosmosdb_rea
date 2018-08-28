#region Using Directives

using System.Fabric;
using Microsoft.Extensions.Configuration;

#endregion

namespace ReaService
{
    internal class ServiceFabricConfigurationSource : IConfigurationSource
    {
        #region Member Fields

        private readonly ServiceContext serviceContext;

        #endregion

        public ServiceFabricConfigurationSource(ServiceContext serviceContext)
        {
            this.serviceContext = serviceContext;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new ServiceFabricConfigurationProvider(serviceContext);
        }
    }
}