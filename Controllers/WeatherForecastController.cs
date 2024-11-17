using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using SofomoWeatherForecastAPI.Data;
using SofomoWeatherForecastAPI.Entities;
using SofomoWeatherForecastAPI.Models;
using SofomoWeatherForecastAPI.Services;

namespace SofomoWeatherForecastAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly WeatherDbContext _context;
        private readonly WeatherForecastService _service;
        private readonly ILogger<WeatherForecastService> _logger;

        public WeatherForecastController(WeatherDbContext context, WeatherForecastService service, ILogger<WeatherForecastService> logger)
        {
            _context = context;
            _service = service;
            _logger = logger;
        }

        [HttpGet("forecast")]
        public async Task<IActionResult> AddLocationAndGetWeather(double latitude, double longitude)
        {
            var client = new RestClient("https://api.open-meteo.com");

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
            var locations = _service.GetLocationsData();
            return Ok(locations);
        }

        [HttpGet("weather-data")]
        public IActionResult GetWeatherData()
        {
            var weatherForecasts = _service.GetWeatherData();
            return Ok(weatherForecasts);
        }

        [HttpGet("locations/{id}/weather")]
        public IActionResult GetWeatherByLocationId(int id)
        {
            var forecasts = _service.GetWeatherByLocationId(id);
            if (forecasts == null || forecasts.Count == 0) return NotFound();

            return Ok(forecasts);
        }

        [HttpDelete("locations/{id}")]
        public  IActionResult DeleteLocation(int id)
        {
            var success = _service.DeleteLocation(id);
            if (!success) return NotFound();
            return Ok();
        }
    }
}
