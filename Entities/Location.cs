using Microsoft.EntityFrameworkCore.Metadata;

namespace SofomoWeatherForecastAPI.Entities
{
    public class Location
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<WeatherForecast> WeatherForecasts { get; set; } = new List<WeatherForecast>();
    }
}
