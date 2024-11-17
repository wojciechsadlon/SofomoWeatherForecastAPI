﻿namespace SofomoWeatherForecastAPI.Models
{
    public class LocationModel
    {
        public int Id { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
