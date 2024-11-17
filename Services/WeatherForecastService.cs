using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SofomoWeatherForecastAPI.Data;
using SofomoWeatherForecastAPI.Entities;
using SofomoWeatherForecastAPI.Models;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly WeatherDbContext _dbContext;
    private readonly IMemoryCache _cache;
    private readonly ILogger<IWeatherForecastService> _logger;
    private readonly string _cacheWeather = "weather_data";
    private readonly string _cacheLocations = "locations";

    public WeatherForecastService(WeatherDbContext dbContext, IMemoryCache cache, ILogger<IWeatherForecastService> logger)
    {
        _dbContext = dbContext;
        _cache = cache;
        _logger = logger;
    }

    public void InitializeCache()
    {
        var allWeatherData = GetWeatherData();
        _cache.Set(_cacheWeather, allWeatherData, TimeSpan.FromMinutes(30));

        var allLocations = GetLocationsData();
        _cache.Set(_cacheLocations, allLocations, TimeSpan.FromMinutes(30));
    }

    public List<WeatherForecastModel> AddOrUpdateWeatherDataAsync(WeatherResponseModel weatherData)
    {
        if (weatherData.Daily == null || !weatherData.Daily.Time.Any()) return new List<WeatherForecastModel>();

        var location = _dbContext.Locations
            .FirstOrDefault(l => l.Latitude == weatherData.Latitude && l.Longitude == weatherData.Longitude);

        if (location == null)
        {
            location = new Location
            {
                Latitude = weatherData.Latitude,
                Longitude = weatherData.Longitude,
                CreatedAt = DateTime.UtcNow
            };
            _dbContext.Locations.Add(location);
            _dbContext.SaveChanges();
        }

        var existingForecasts = _dbContext.WeatherForecasts
            .Where(wf => wf.LocationId == location.Id)
            .AsEnumerable()
            .Where(wf => weatherData.Daily.Time.Contains(wf.ForecastDate.ToString("yyyy-MM-dd")))
            .ToList();

        var forecastDates = weatherData.Daily.Time.Select(d => DateTime.Parse(d)).ToList();

        for (int i = 0; i < forecastDates.Count; i++)
        {
            var date = forecastDates[i];
            var temperatureMax = weatherData.Daily?.Temperature2mMax[i];
            var temperatureMin = weatherData.Daily?.Temperature2mMin[i];
            var rainSum = weatherData.Daily?.RainSum[i];
            var windSpeedMax = weatherData.Daily?.WindSpeed10mMax[i];

            var forecast = existingForecasts.FirstOrDefault(wf => wf.ForecastDate.Date == date.Date);

            if (forecast == null)
            {
                forecast = new WeatherForecast
                {
                    LocationId = location.Id,
                    ForecastDate = date,
                    TemperatureMax = temperatureMax,
                    TemperatureMin = temperatureMin,
                    RainSum = rainSum,
                    WindSpeedMax = windSpeedMax
                };
                _dbContext.WeatherForecasts.Add(forecast);
            }
            else
            {
                forecast.TemperatureMax = temperatureMax;
                forecast.TemperatureMin = temperatureMin;
                forecast.RainSum = rainSum;
                forecast.WindSpeedMax = windSpeedMax;
            }
            AddOrMatchUnits(forecast, weatherData.DailyUnits);
        }

        _dbContext.SaveChanges();
        var savedForecastsForLocation = _dbContext.WeatherForecasts.Where(x => x.LocationId == location.Id).ToList();

        return GetWeatherForecastsModel(savedForecastsForLocation);
    }

    private void AddOrMatchUnits(WeatherForecast forecast, DailyUnitsModel dailyUnits)
    {
        var existingUnit = _dbContext.WeatherForecastUnits
        .FirstOrDefault(u => u.Temperature2mMax == dailyUnits.Temperature2mMax
                                && u.Temperature2mMin == dailyUnits.Temperature2mMin
                                && u.RainSum == dailyUnits.RainSum
                                && u.WindSpeed10mMax == dailyUnits.WindSpeed10mMax);

        WeatherForecastUnit unitToUse;

        if (existingUnit != null)
        {
            unitToUse = existingUnit;
        }
        else
        {
            unitToUse = new WeatherForecastUnit
            {
                Temperature2mMax = dailyUnits.Temperature2mMax,
                Temperature2mMin = dailyUnits.Temperature2mMin,
                RainSum = dailyUnits.RainSum,
                WindSpeed10mMax = dailyUnits.WindSpeed10mMax
            };

            _dbContext.WeatherForecastUnits.Add(unitToUse);
            _dbContext.SaveChanges();
        }

        forecast.WeatherForecastUnit = unitToUse;
    }

    private List<WeatherForecastModel> GetWeatherForecastsModel(List<WeatherForecast> weatherForecasts)
    {
        return weatherForecasts.Select(f => new WeatherForecastModel
        {
            Id = f.Id,
            LocationId = f.LocationId,
            ForecastDate = f.ForecastDate,
            TemperatureMax = f.TemperatureMax.ToString() + ' ' + f.WeatherForecastUnit?.Temperature2mMax,
            TemperatureMin = f.TemperatureMin.ToString() + ' ' + f.WeatherForecastUnit?.Temperature2mMin,
            RainSum = f.RainSum.ToString() + ' ' + f.WeatherForecastUnit?.RainSum,
            WindSpeedMax = f.WindSpeedMax.ToString() + ' ' + f.WeatherForecastUnit?.WindSpeed10mMax
        }).ToList();
    }

    private List<LocationModel> GetLocationsModel(List<Location> locations)
    {
        return locations.Select(location => new LocationModel
        {
            Id = location.Id,
            Latitude = location.Latitude,
            Longitude = location.Longitude,
            CreatedAt = location.CreatedAt
        }).ToList();
    }

    public List<WeatherForecastModel> GetWeatherData()
    {
        try
        {
            var forecasts = _dbContext.WeatherForecasts.ToList();

            var models = GetWeatherForecastsModel(forecasts);

            _cache.Set(_cacheWeather, models, TimeSpan.FromMinutes(30));

            return models;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data from database.");

            if (_cache.TryGetValue(_cacheWeather, out List<WeatherForecastModel> fallbackData))
            {
                _logger.LogWarning("Database is out of reach, returning cached data.");
                return fallbackData;
            }

            throw new Exception("Unable to return data from database or cache.");
        }
    }

    public List<LocationModel> GetLocationsData()
    {
        try
        {
            var locations = _dbContext.Locations.ToList();

            var models = GetLocationsModel(locations);

            _cache.Set(_cacheLocations, models, TimeSpan.FromMinutes(30));

            return models;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting data from database.");

            if (_cache.TryGetValue(_cacheLocations, out List<LocationModel> fallbackData))
            {
                _logger.LogWarning("Database is out of reach, returning cached data.");
                return fallbackData;
            }

            throw new Exception("Unable to return data from database or cache.");
        }
    }

    public List<WeatherForecastModel> GetWeatherByLocationId(int locationId)
    {
        var location = _dbContext.Locations
            .Include(l => l.WeatherForecasts)
            .FirstOrDefault(l => l.Id == locationId);

        if (location == null || location.WeatherForecasts == null)
        {
            _logger.LogWarning($"No weather forecasts found for LocationId {locationId}");
            return new List<WeatherForecastModel>();
        }

        return GetWeatherForecastsModel(location.WeatherForecasts.ToList());
    }

    public bool DeleteLocation(int id)
    {
        var location = _dbContext.Locations.Find(id);
        if (location == null) return false;

        _dbContext.Locations.Remove(location);
        _dbContext.SaveChanges();

        return true;
    }
}
