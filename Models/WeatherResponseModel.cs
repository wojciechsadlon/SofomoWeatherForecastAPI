using Newtonsoft.Json;

namespace SofomoWeatherForecastAPI.Models
{
    public class WeatherResponseModel
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        [JsonProperty("generationtime_ms")]
        public double GenerationTimeMs { get; set; }
        [JsonProperty("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }
        public string Timezone { get; set; }
        [JsonProperty("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; }
        public double Elevation { get; set; }
        [JsonProperty("daily_units")]
        public DailyUnitsModel DailyUnits { get; set; }
        public DailyForecastModel Daily { get; set; }
    }

    public class DailyUnitsModel
    {
        public string Time { get; set; }

        [JsonProperty("temperature_2m_max")]
        public string Temperature2mMax { get; set; }

        [JsonProperty("temperature_2m_min")]
        public string Temperature2mMin { get; set; }

        [JsonProperty("rain_sum")]
        public string RainSum { get; set; }

        [JsonProperty("wind_speed_10m_max")]
        public string WindSpeed10mMax { get; set; }
    }

    public class DailyForecastModel
    {
        public List<string> Time { get; set; }
        [JsonProperty("temperature_2m_max")]
        public List<double> Temperature2mMax { get; set; }
        [JsonProperty("temperature_2m_min")]
        public List<double> Temperature2mMin { get; set; }
        [JsonProperty("rain_sum")]
        public List<double> RainSum { get; set; }
        [JsonProperty("wind_speed_10m_max")]
        public List<double> WindSpeed10mMax { get; set; }
    }
}
