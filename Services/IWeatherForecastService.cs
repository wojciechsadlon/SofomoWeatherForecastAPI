using SofomoWeatherForecastAPI.Models;

public interface IWeatherForecastService
{
    List<WeatherForecastModel> AddOrUpdateWeatherDataAsync(WeatherResponseModel weatherData);
    bool DeleteLocation(int id);
    List<LocationModel> GetLocationsData();
    List<WeatherForecastModel> GetWeatherByLocationId(int locationId);
    List<WeatherForecastModel> GetWeatherData();
    void InitializeCache();
}