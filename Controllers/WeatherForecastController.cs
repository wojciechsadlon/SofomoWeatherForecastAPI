using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SofomoWeatherForecastAPI.Data;
using SofomoWeatherForecastAPI.Models;
using SofomoWeatherForecastAPI.Services;

namespace SofomoWeatherForecastAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly IWeatherForecastService _service;
        private readonly ILogger<IWeatherForecastService> _logger;

        public WeatherForecastController(WeatherDbContext context, IWeatherForecastService service, ILogger<IWeatherForecastService> logger)
        {
            _context = context;
            _service = service;
            _logger = logger;
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> AddLocationAndGetWeather(double latitude, double longitude)
        {
            var client = new RestClient("https://api.open-meteo.com");
            var lati = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
            var longi = longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

            var request = new RestRequest("/v1/forecast", Method.Get)
                .AddQueryParameter("latitude", latitude.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .AddQueryParameter("longitude", longitude.ToString(System.Globalization.CultureInfo.InvariantCulture))
                .AddQueryParameter("daily", "temperature_2m_max,temperature_2m_min,rain_sum,wind_speed_10m_max");

            try
            {
                var response = await client.ExecuteAsync(request);
                if (!response.IsSuccessful)
                {
                    _logger.LogError("Error fetching weather data.");
                    return StatusCode(500, "Error fetching weather data");
                }

                var responseModel = JsonConvert.DeserializeObject<WeatherResponseModel>(response.Content);
                var models = _service.AddOrUpdateWeatherDataAsync(responseModel);

                return Ok(models);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving weather data to database.");
                throw new Exception(ex.Message);
            }
        }

        [HttpPost("sync-weather")]
        public async Task<IActionResult> SyncWeatherForExistingLocations()
        {
            try
            {
                var locations = _service.GetLocationsData();
                if (locations == null || locations.Count == 0)
                {
                    return NotFound("No locations found to sync weather data.");
                }

                foreach (var location in locations)
                {
                    if (location.Latitude != null && location.Longitude != null)
                    {
                        var latitude = location.Latitude;
                        var longitude = location.Longitude;

                        var weatherDataResult = await AddLocationAndGetWeather(latitude, longitude);
                        if (weatherDataResult == null)
                        {
                            _logger.LogError($"Failed to sync weather for location ID: {location.Id}");
                        }
                    }
                    else
                    {
                        _logger.LogWarning($"Location ID: {location.Id} does not have valid coordinates.");
                    }
                }

                return Ok("Weather data synced for all locations.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing weather data for existing locations.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("my-weather")]
        public async Task<IActionResult> GetMyWeather()
        {
            try
            {
                string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

                if (string.IsNullOrEmpty(ipAddress))
                {
                    return BadRequest("Unable to retrieve IP address.");
                }

                var location = await IpStackService.GetLocationFromIpAsync(ipAddress);
                if (location == null || location.Latitude == null || location.Longitude == null)
                {
                    return StatusCode(500, "Error fetching location data.");
                }

                double latitude = location.Latitude.Value;
                double longitude = location.Longitude.Value;

                var weatherData = await AddLocationAndGetWeather(latitude, longitude);
                if (weatherData == null)
                {
                    return StatusCode(500, "Error fetching weather data.");
                }

                return Ok(weatherData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching weather for your location.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("locations")]
        public IActionResult GetLocations()
        {
            try
            {
                var locations = _service.GetLocationsData();
                if (locations == null || !locations.Any())
                {
                    return NotFound("No locations found.");
                }
                return Ok(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving locations.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("weather-data")]
        public IActionResult GetWeatherData()
        {
            try
            {
                var weatherForecasts = _service.GetWeatherData();
                if (weatherForecasts == null || !weatherForecasts.Any())
                {
                    return NotFound("No weather data found.");
                }
                return Ok(weatherForecasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving weather data.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("locations/{id}/weather")]
        public IActionResult GetWeatherByLocationId(int id)
        {
            try
            {
                var forecasts = _service.GetWeatherByLocationId(id);
                if (forecasts == null || forecasts.Count == 0)
                {
                    return NotFound($"No weather data found for location ID: {id}");
                }
                return Ok(forecasts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving weather data for location ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("locations/{id}")]
        public  IActionResult DeleteLocation(int id)
        {
            try
            {
                var success = _service.DeleteLocation(id);
                if (!success)
                {
                    return NotFound($"Location with ID {id} not found.");
                }
                return Ok("Location deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting location with ID: {id}");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
