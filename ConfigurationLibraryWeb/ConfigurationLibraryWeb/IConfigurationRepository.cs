namespace ConfigurationLibraryWeb
{
    public interface IConfigurationRepository
    {
        Task<IEnumerable<Configuration>> GetActiveConfigurationsAsync(string applicationName);
        Task<IEnumerable<Configuration>>GetAllConfigurationsAsync();
        Task AddConfigurationAsync(addConfigurationDto record);
        Task UpdateConfigurationAsync(updateConfigurationDto record);
    }

}
