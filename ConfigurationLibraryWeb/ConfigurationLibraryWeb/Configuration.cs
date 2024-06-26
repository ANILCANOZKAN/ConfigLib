using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ConfigurationLibraryWeb
{
    public class Configuration
    {
        [BsonId]
        public ObjectId _id {  get; set; }
        public int configurationId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public dynamic value { get; set; }
        public bool isActive { get; set; }
        public string applicationName { get; set; }
    }
}
