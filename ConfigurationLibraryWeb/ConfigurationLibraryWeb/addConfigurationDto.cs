using System.Text.Json;

namespace ConfigurationLibraryWeb
{
    public class addConfigurationDto
    {
        public string name { get; set; }
        public string type { get; set; }
        public dynamic value { get; set; }
        public bool isActive { get; set; }
        public string applicationName { get; set; }
    }
}
