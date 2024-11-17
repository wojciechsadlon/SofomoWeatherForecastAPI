using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SofomoWeatherForecastAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<double>(type: "float", nullable: false),
                    Longitude = table.Column<double>(type: "float", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecastUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Temperature2mMax = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Temperature2mMin = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RainSum = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    WindSpeed10mMax = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecastUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherForecasts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    ForecastDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TemperatureMax = table.Column<double>(type: "float", nullable: true),
                    TemperatureMin = table.Column<double>(type: "float", nullable: true),
                    RainSum = table.Column<double>(type: "float", nullable: true),
                    WindSpeedMax = table.Column<double>(type: "float", nullable: true),
                    WeatherForecastUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherForecasts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WeatherForecasts_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WeatherForecasts_WeatherForecastUnits_WeatherForecastUnitId",
                        column: x => x.WeatherForecastUnitId,
                        principalTable: "WeatherForecastUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_LocationId",
                table: "WeatherForecasts",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_WeatherForecastUnitId",
                table: "WeatherForecasts",
                column: "WeatherForecastUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecastUnits_Temperature2mMax_Temperature2mMin_RainSum_WindSpeed10mMax",
                table: "WeatherForecastUnits",
                columns: new[] { "Temperature2mMax", "Temperature2mMin", "RainSum", "WindSpeed10mMax" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WeatherForecasts");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "WeatherForecastUnits");
        }
    }
}
