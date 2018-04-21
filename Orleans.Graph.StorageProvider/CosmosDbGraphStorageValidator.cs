namespace Orleans.Graph.StorageProvider
{
    /// <summary>
    /// Configuration validator for AzureTableStorageOptions
    /// </summary>
    public class CosmosDbGraphStorageValidator : IConfigurationValidator
    {
        private readonly CosmosDbGraphStorageOptions options;
        private readonly string name;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">The option to be validated.</param>
        /// <param name="name">The option name to be validated.</param>
        public CosmosDbGraphStorageValidator(CosmosDbGraphStorageOptions options, string name)
        {
            this.options = options;
            this.name = name;
        }

        public void ValidateConfiguration()
        {
            
        }
    }
}