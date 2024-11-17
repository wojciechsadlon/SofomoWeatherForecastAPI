using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SofomoWeatherForecastAPI.Migrations
{
    /// <inheritdoc />
    public partial class addweatherforecastunit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WeatherForecastUnitId",
                table: "WeatherForecasts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeatherForecastUnitId1",
                table: "WeatherForecasts",
                type: "int",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_WeatherForecastUnitId",
                table: "WeatherForecasts",
                column: "WeatherForecastUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecasts_WeatherForecastUnitId1",
                table: "WeatherForecasts",
                column: "WeatherForecastUnitId1");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherForecastUnits_Temperature2mMax_Temperature2mMin_RainSum_WindSpeed10mMax",
                table: "WeatherForecastUnits",
                columns: new[] { "Temperature2mMax", "Temperature2mMin", "RainSum", "WindSpeed10mMax" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WeatherForecasts_WeatherForecastUnits_WeatherForecastUnitId",
                table: "WeatherForecasts",
                column: "WeatherForecastUnitId",
                principalTable: "WeatherForecastUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WeatherForecasts_WeatherForecastUnits_WeatherForecastUnitId1",
                table: "WeatherForecasts",
                column: "WeatherForecastUnitId1",
                principalTable: "WeatherForecastUnits",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WeatherForecasts_WeatherForecastUnits_WeatherForecastUnitId",
                table: "WeatherForecasts");

            migrationBuilder.DropForeignKey(
                name: "FK_WeatherForecasts_WeatherForecastUnits_WeatherForecastUnitId1",
                table: "WeatherForecasts");

            migrationBuilder.DropTable(
                name: "WeatherForecastUnits");

            migrationBuilder.DropIndex(
                name: "IX_WeatherForecasts_WeatherForecastUnitId",
                table: "WeatherForecasts");

            migrationBuilder.DropIndex(
                name: "IX_WeatherForecasts_WeatherForecastUnitId1",
                table: "WeatherForecasts");

            migrationBuilder.DropColumn(
                name: "WeatherForecastUnitId",
                table: "WeatherForecasts");

            migrationBuilder.DropColumn(
                name: "WeatherForecastUnitId1",
                table: "WeatherForecasts");
        }
    }
}
