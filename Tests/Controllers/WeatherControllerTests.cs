using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SofomoWeatherForecastAPI.Controllers;
using SofomoWeatherForecastAPI.Data;
using SofomoWeatherForecastAPI.Entities;
using Microsoft.Extensions.Caching.Memory;
using SofomoWeatherForecastAPI.Models;
using SofomoWeatherForecastAPI.Services;

namespace WeatherForecastAPI.Tests
{
    public class WeatherControllerTests
    {
        private readonly WeatherForecastController _controller;
        private readonly WeatherDbContext _context;
        private readonly WeatherForecastService _service;
        private readonly MemoryCache _cache;
        private readonly Mock<ILogger<WeatherForecastService>> _loggerMock;
        private readonly Location _location;

        public WeatherControllerTests()
        {
            var options = new DbContextOptionsBuilder<WeatherDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new WeatherDbContext(options);
            _cache = new MemoryCache(new MemoryCacheOptions());
            _loggerMock = new Mock<ILogger<WeatherForecastService>>();
            _service = new WeatherForecastService(_context, _cache, _loggerMock.Object);
            _controller = new WeatherForecastController(_context, _service, _loggerMock.Object);
            _location = new Location { Latitude = 52.2297, Longitude = 21.0122 };

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

        }

        [Fact]
        public async Task AddOrUpdateWeatherDataAsync_ShouldAddLocationAndNewWeatherData()
        {
            // Act
            var result = await _controller.AddLocationAndGetWeather(_location.Latitude, _location.Longitude) as OkObjectResult;
            var weatherForecast = _context.WeatherForecasts.ToList();
            var location = _context.Locations.FirstOrDefault();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(weatherForecast);
            Assert.NotNull(location);
            Assert.Equal(weatherForecast[0].LocationId, location.Id);
            Assert.Equal(weatherForecast.Count, 7);
        }
        [Fact]
        public async Task AddOrUpdateWeatherDataAsync_ShouldGetMyWeatherData()
        {
            // Arrange
            string ipAddress = "193.28.84.1";
            
            // Act
            var ipStackLocation = await IpStackService.GetLocationFromIpAsync(ipAddress);
            double latitude = ipStackLocation.Latitude.Value;
            double longitude = ipStackLocation.Longitude.Value;

            var weatherData = await _controller.AddLocationAndGetWeather(latitude, longitude);

            var weatherForecast = _context.WeatherForecasts.ToList();
            var location = _context.Locations.FirstOrDefault();

            // Assert
            Assert.NotNull(ipStackLocation);
            Assert.NotNull(location);
            Assert.NotNull(weatherData);
            Assert.Equal(weatherForecast[0].LocationId, location.Id);
            Assert.Equal(weatherForecast.Count, 7);
        }


        [Fact]
        public void GetLocations_ShouldReturnAllLocations()
        {
            // Arrange
            _context.Locations.Add(new Location { Latitude = 52.2297, Longitude = 21.0122 });
            _context.Locations.Add(new Location { Latitude = 50.0647, Longitude = 19.9450 });
            _context.SaveChanges();

            // Act
            var result = _service.GetLocationsData();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void GetLocations_ShouldDeleteLocation()
        {
            // Arrange
            var location = new Location { Latitude = 52.2297, Longitude = 21.0122 };
            _context.Locations.Add(location);
            _context.SaveChanges();

            // Act
            _controller.DeleteLocation(location.Id);
            var locations = _service.GetLocationsData();

            // Assert
            Assert.NotNull(locations);
            Assert.Empty(locations);
        }
    }
}