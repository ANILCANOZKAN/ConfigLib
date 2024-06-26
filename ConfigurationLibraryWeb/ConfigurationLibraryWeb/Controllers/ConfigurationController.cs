using ConfigurationLibrary;
using Microsoft.AspNetCore.Mvc;

namespace ConfigurationLibraryWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigurationController : ControllerBase
    {
        public ConfigurationController(IConfigurationRepository repository)
        {
            _repository = repository;
        }

        [Route("/library")]
        [HttpGet]
        public IActionResult Get()
        {
            string connectionString = "mongodb+srv://anilcan:Password_123.@task.9wpfwp3.mongodb.net/?retryWrites=true&w=majority&appName=Task";
            string applicationName = "SERVICE-B";
            int refreshTimerIntervalInMs = 30000;

            var configReader = new ConfigurationReader(applicationName, connectionString, refreshTimerIntervalInMs);
            try
            {
                string value = configReader.GetValue<string>("IsBasketEnabled");

                return Ok($"Value: {value}");
            }
            catch (Exception ex)
            {
                return NotFound($"Error: {ex.Message}");
            }
        }
        
        private readonly IConfigurationRepository _repository;
       

        [HttpGet]
        public async Task<IActionResult> GetConfigurations(string applicationName)
        {
            var configurations = await _repository.GetActiveConfigurationsAsync(applicationName);
            return Ok(configurations);
        }

        [HttpGet]
        [Route("/all")]
        public async Task<IActionResult> GetAllConfigurations()
        {
            var configurations = await _repository.GetAllConfigurationsAsync();
            return Ok(configurations);
        }

        [HttpPost]
        public async Task<IActionResult> AddConfiguration([FromBody] addConfigurationDto record)
        {
            await _repository.AddConfigurationAsync(record);

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateConfiguration([FromBody] updateConfigurationDto record)
        {
            await _repository.UpdateConfigurationAsync(record);
            return Ok();
        }
    }
}
