using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoWeather.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedCairoWeatherAppTablesForHoulryandYearlyTabless : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WeatherCode = table.Column<int>(type: "int", nullable: false),
                    Temperature2mMean = table.Column<double>(type: "float", nullable: false),
                    Temperature2mMax = table.Column<double>(type: "float", nullable: false),
                    Temperature2mMin = table.Column<double>(type: "float", nullable: false),
                    ApparentTemperatureMean = table.Column<double>(type: "float", nullable: false),
                    ApparentTemperatureMax = table.Column<double>(type: "float", nullable: false),
                    ApparentTemperatureMin = table.Column<double>(type: "float", nullable: false),
                    Sunrise = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sunset = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaylightDuration = table.Column<double>(type: "float", nullable: false),
                    SunshineDuration = table.Column<double>(type: "float", nullable: false),
                    WindSpeed10mMax = table.Column<double>(type: "float", nullable: false),
                    WindGusts10mMax = table.Column<double>(type: "float", nullable: false),
                    WindDirection10mDominant = table.Column<double>(type: "float", nullable: false),
                    ShortwaveRadiationSum = table.Column<double>(type: "float", nullable: false),
                    Et0FaoEvapotranspirationSum = table.Column<double>(type: "float", nullable: false),
                    CloudCoverMean = table.Column<double>(type: "float", nullable: false),
                    CloudCoverMax = table.Column<double>(type: "float", nullable: false),
                    CloudCoverMin = table.Column<double>(type: "float", nullable: false),
                    DewPoint2mMean = table.Column<double>(type: "float", nullable: false),
                    DewPoint2mMax = table.Column<double>(type: "float", nullable: false),
                    DewPoint2mMin = table.Column<double>(type: "float", nullable: false),
                    RelativeHumidity2mMean = table.Column<double>(type: "float", nullable: false),
                    RelativeHumidity2mMax = table.Column<double>(type: "float", nullable: false),
                    RelativeHumidity2mMin = table.Column<double>(type: "float", nullable: false),
                    PressureMslMean = table.Column<double>(type: "float", nullable: false),
                    PressureMslMax = table.Column<double>(type: "float", nullable: false),
                    PressureMslMin = table.Column<double>(type: "float", nullable: false),
                    SurfacePressureMean = table.Column<double>(type: "float", nullable: false),
                    SurfacePressureMax = table.Column<double>(type: "float", nullable: false),
                    SurfacePressureMin = table.Column<double>(type: "float", nullable: false),
                    WindSpeed10mMean = table.Column<double>(type: "float", nullable: false),
                    WindSpeed10mMin = table.Column<double>(type: "float", nullable: false),
                    WindGusts10mMean = table.Column<double>(type: "float", nullable: false),
                    WindGusts10mMin = table.Column<double>(type: "float", nullable: false),
                    WetBulbTemperature2mMean = table.Column<double>(type: "float", nullable: false),
                    WetBulbTemperature2mMax = table.Column<double>(type: "float", nullable: false),
                    WetBulbTemperature2mMin = table.Column<double>(type: "float", nullable: false),
                    VapourPressureDeficitMax = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture0To100cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture0To7cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture7To28cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture28To100cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature0To100cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature0To7cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature7To28cmMean = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature28To100cmMean = table.Column<double>(type: "float", nullable: false),
                    SnowfallWaterEquivalentSum = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HourlyReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature2m = table.Column<double>(type: "float", nullable: false),
                    RelativeHumidity2m = table.Column<double>(type: "float", nullable: false),
                    DewPoint2m = table.Column<double>(type: "float", nullable: false),
                    ApparentTemperature = table.Column<double>(type: "float", nullable: false),
                    WeatherCode = table.Column<int>(type: "int", nullable: false),
                    PressureMsl = table.Column<double>(type: "float", nullable: false),
                    SurfacePressure = table.Column<double>(type: "float", nullable: false),
                    CloudCover = table.Column<double>(type: "float", nullable: false),
                    CloudCoverLow = table.Column<double>(type: "float", nullable: false),
                    CloudCoverMid = table.Column<double>(type: "float", nullable: false),
                    CloudCoverHigh = table.Column<double>(type: "float", nullable: false),
                    Et0FaoEvapotranspiration = table.Column<double>(type: "float", nullable: false),
                    VapourPressureDeficit = table.Column<double>(type: "float", nullable: false),
                    WindSpeed10m = table.Column<double>(type: "float", nullable: false),
                    WindSpeed100m = table.Column<double>(type: "float", nullable: false),
                    WindDirection10m = table.Column<double>(type: "float", nullable: false),
                    WindDirection100m = table.Column<double>(type: "float", nullable: false),
                    WindGusts10m = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature0To7cm = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature7To28cm = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature28To100cm = table.Column<double>(type: "float", nullable: false),
                    SoilTemperature100To255cm = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture0To7cm = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture7To28cm = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture28To100cm = table.Column<double>(type: "float", nullable: false),
                    SoilMoisture100To255cm = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourlyReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DailyReadings_Date",
                table: "DailyReadings",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_HourlyReadings_Time",
                table: "HourlyReadings",
                column: "Time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyReadings");

            migrationBuilder.DropTable(
                name: "HourlyReadings");
        }
    }
}
