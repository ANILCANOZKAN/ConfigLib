using MongoDB.Bson;
using MongoDB.Driver;

namespace ConfigurationLibrary
{
    public class ConfigurationReader
    {
        private readonly string _applicationName;
        private readonly string _connectionString;
        private readonly int _refreshTimerIntervalInMs;
        private Dictionary<string, object> _configurations;
        private Timer _refreshTimer;
        private IMongoCollection<BsonDocument> _configCollection;

        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            _applicationName = applicationName ?? throw new ArgumentNullException(nameof(applicationName));
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            _refreshTimerIntervalInMs = refreshTimerIntervalInMs;

            Initialize();
        }

        private void Initialize()
        {
            var client = new MongoClient(_connectionString);
            var database = client.GetDatabase("ConfigurationLibrary");
            _configCollection = database.GetCollection<BsonDocument>("Configurations");

            _configurations = new Dictionary<string, object>();
            LoadConfigurationsAsync().Wait();


            _refreshTimer = new Timer(async _ => await RefreshConfigurationsAsync(), null, _refreshTimerIntervalInMs, _refreshTimerIntervalInMs);
        }

        private async Task LoadConfigurationsAsync()
        {
            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("applicationName", _applicationName),
                Builders<BsonDocument>.Filter.Eq("isActive", true)
            );

            var configurations = await _configCollection.Find(filter).ToListAsync();
            foreach (var config in configurations)
            {
                var key = config["name"].AsString;
                var value = config["value"].AsBsonValue;
                _configurations[key] = BsonTypeMapper.MapToDotNetValue(value);
            }
        }

        private async Task RefreshConfigurationsAsync()
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("applicationName", _applicationName),
                    Builders<BsonDocument>.Filter.Eq("isActive", true)
                );

                var configurations = await _configCollection.Find(filter).ToListAsync();
                foreach (var config in configurations)
                {
                    var key = config["name"].AsString;
                    var value = config["value"].AsBsonValue; 
                    _configurations[key] = BsonTypeMapper.MapToDotNetValue(value);
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"Error refreshing configurations: {ex.Message}");
            }
        }

        public T GetValue<T>(string key)
        {
            if (_configurations.ContainsKey(key))
            {
                var value = _configurations[key];
                return (T)Convert.ChangeType(value, typeof(T));
            }
            throw new KeyNotFoundException($"Key '{key}' not found in the configuration.");
        }
    }
}
