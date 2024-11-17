namespace SofomoWeatherForecastAPI.Entities
{
    public class WeatherForecast
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public DateTime ForecastDate { get; set; }
        public double? TemperatureMax { get; set; }
        public double? TemperatureMin { get; set; }
        public double? RainSum { get; set; }
        public double? WindSpeedMax { get; set; }

        public int? WeatherForecastUnitId { get; set; }
        public WeatherForecastUnit WeatherForecastUnit { get; set; }

        public Location Location { get; set; }
    }
}
