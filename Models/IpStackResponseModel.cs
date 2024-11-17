namespace SofomoWeatherForecastAPI.Models
{
    public class IpStackResponseModel
    {
        public string Ip { get; set; }
        public string CountryName { get; set; }
        public string RegionName { get; set; }
        public string City { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
    }
}
