namespace SofomoWeatherForecastAPI.Models
{
    public class WeatherForecastModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public DateTime ForecastDate { get; set; }
        public string? TemperatureMax { get; set; }
        public string? TemperatureMin { get; set; }
        public string? RainSum { get; set; }
        public string? WindSpeedMax { get; set; }
    }
}
