

using ConfigurationLibraryWeb;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;
using System.Text.Json.Nodes;

public class MongoConfigurationRepository : IConfigurationRepository
{
    private readonly IMongoCollection<Configuration> _collection;

    public MongoConfigurationRepository(string connectionString)
    {
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase("ConfigurationLibrary");
        _collection = database.GetCollection<Configuration>("Configurations");
    }

    public async Task<IEnumerable<Configuration>> GetActiveConfigurationsAsync(string applicationName)
    {
        var filter = Builders<Configuration>.Filter.Eq(c => c.applicationName, applicationName) &
                     Builders<Configuration>.Filter.Eq(c => c.isActive, true);
        return await _collection.Find(filter).ToListAsync();
    }
    public async Task<IEnumerable<Configuration>> GetAllConfigurationsAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task AddConfigurationAsync(addConfigurationDto record)
    {
        string valueAsString = record.value.ToString();
        var value = ConvertToBsonValue(record.type, valueAsString);


        var configuration = new Configuration
        {
            applicationName = record.applicationName,
            isActive = record.isActive,
            name = record.name,
            type = record.type,
            value = value,
        };

        await _collection.InsertOneAsync(configuration);
    }
    public dynamic ConvertToBsonValue(string type, string value)
    {
        switch (type.ToLower())
        {
            case "string":
                return value;
            case "bool" or "boolean":
                if (bool.TryParse(value, out bool boolValue))
                    return boolValue;
                else
                    throw new ArgumentException($"Cannot convert '{value}' to boolean");
            case "int" or "ınt":
                if (int.TryParse(value, out int intValue))
                    return intValue;
                else
                    throw new ArgumentException($"Cannot convert '{value}' to integer");
            case "double":
                if (double.TryParse(value, out double doubleValue))
                    return doubleValue;
                else
                    throw new ArgumentException($"Cannot convert '{value}' to double");
            default:
                throw new ArgumentException($"Unsupported type: {type}");
        }
    }


    public async Task UpdateConfigurationAsync(updateConfigurationDto record)
    {
        var filter = Builders<Configuration>.Filter.Eq(c => c.configurationId, record.configurationId);
        var oldConfiguration = _collection.Find(filter).First();
        string valueAsString = record.value.ToString();
        var value = ConvertToBsonValue(record.type, valueAsString);
        var configuration = new Configuration
        {
            _id = oldConfiguration._id,
            applicationName = record.applicationName,
            isActive = record.isActive,
            name = record.name,
            type = record.type,
            value = value,
            configurationId = record.configurationId
        };
        
        await _collection.ReplaceOneAsync(filter, configuration);
    }
}
