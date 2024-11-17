using Microsoft.EntityFrameworkCore;
using SofomoWeatherForecastAPI.Entities;

namespace SofomoWeatherForecastAPI.Data
{
    public class WeatherDbContext : DbContext
    {
        public WeatherDbContext(DbContextOptions<WeatherDbContext> options) : base(options) { }
        public DbSet<Location> Locations { get; set; }
        public DbSet<WeatherForecast> WeatherForecasts { get; set; }
        public DbSet<WeatherForecastUnit> WeatherForecastUnits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherForecast>()
                .HasOne(wf => wf.Location)
                .WithMany(l => l.WeatherForecasts)
                .HasForeignKey(wf => wf.LocationId);

            modelBuilder.Entity<WeatherForecast>()
                 .HasOne(wf => wf.WeatherForecastUnit)
                 .WithMany()
                 .HasForeignKey(wf => wf.WeatherForecastUnitId)
                 .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<WeatherForecastUnit>()
                .HasIndex(u => new { u.Temperature2mMax, u.Temperature2mMin, u.RainSum, u.WindSpeed10mMax })
                .IsUnique();

        }
    }
}
