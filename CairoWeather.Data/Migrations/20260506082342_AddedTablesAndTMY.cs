using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CairoWeather.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTablesAndTMY : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "HourlyReadingSequence");

            migrationBuilder.CreateTable(
                name: "HourlyReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [HourlyReadingSequence]"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature2m = table.Column<double>(type: "float", nullable: true),
                    RelativeHumidity2m = table.Column<double>(type: "float", nullable: true),
                    DewPoint2m = table.Column<double>(type: "float", nullable: true),
                    ApparentTemperature = table.Column<double>(type: "float", nullable: true),
                    WeatherCode = table.Column<int>(type: "int", nullable: true),
                    PressureMsl = table.Column<double>(type: "float", nullable: true),
                    SurfacePressure = table.Column<double>(type: "float", nullable: true),
                    CloudCover = table.Column<double>(type: "float", nullable: true),
                    CloudCoverLow = table.Column<double>(type: "float", nullable: true),
                    CloudCoverMid = table.Column<double>(type: "float", nullable: true),
                    CloudCoverHigh = table.Column<double>(type: "float", nullable: true),
                    ShortwaveRadiation = table.Column<double>(type: "float", nullable: true),
                    ClearSkyRadiation = table.Column<double>(type: "float", nullable: true),
                    Et0FaoEvapotranspiration = table.Column<double>(type: "float", nullable: true),
                    VapourPressureDeficit = table.Column<double>(type: "float", nullable: true),
                    WindSpeed10m = table.Column<double>(type: "float", nullable: true),
                    WindSpeed100m = table.Column<double>(type: "float", nullable: true),
                    WindDirection10m = table.Column<double>(type: "float", nullable: true),
                    WindDirection100m = table.Column<double>(type: "float", nullable: true),
                    WindGusts10m = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature0To7cm = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature7To28cm = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature28To100cm = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature100To255cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture0To7cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture7To28cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture28To100cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture100To255cm = table.Column<double>(type: "float", nullable: true),
                    Humidex = table.Column<double>(type: "float", nullable: true),
                    AirDensity = table.Column<double>(type: "float", nullable: true),
                    WindPowerDensity = table.Column<double>(type: "float", nullable: true),
                    PhotoThermalUnit = table.Column<double>(type: "float", nullable: true),
                    CloudRadiativeForcing = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HourlyReadings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TmyReadings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false, defaultValueSql: "NEXT VALUE FOR [HourlyReadingSequence]"),
                    Time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperature2m = table.Column<double>(type: "float", nullable: true),
                    RelativeHumidity2m = table.Column<double>(type: "float", nullable: true),
                    DewPoint2m = table.Column<double>(type: "float", nullable: true),
                    ApparentTemperature = table.Column<double>(type: "float", nullable: true),
                    WeatherCode = table.Column<int>(type: "int", nullable: true),
                    PressureMsl = table.Column<double>(type: "float", nullable: true),
                    SurfacePressure = table.Column<double>(type: "float", nullable: true),
                    CloudCover = table.Column<double>(type: "float", nullable: true),
                    CloudCoverLow = table.Column<double>(type: "float", nullable: true),
                    CloudCoverMid = table.Column<double>(type: "float", nullable: true),
                    CloudCoverHigh = table.Column<double>(type: "float", nullable: true),
                    ShortwaveRadiation = table.Column<double>(type: "float", nullable: true),
                    ClearSkyRadiation = table.Column<double>(type: "float", nullable: true),
                    Et0FaoEvapotranspiration = table.Column<double>(type: "float", nullable: true),
                    VapourPressureDeficit = table.Column<double>(type: "float", nullable: true),
                    WindSpeed10m = table.Column<double>(type: "float", nullable: true),
                    WindSpeed100m = table.Column<double>(type: "float", nullable: true),
                    WindDirection10m = table.Column<double>(type: "float", nullable: true),
                    WindDirection100m = table.Column<double>(type: "float", nullable: true),
                    WindGusts10m = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature0To7cm = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature7To28cm = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature28To100cm = table.Column<double>(type: "float", nullable: true),
                    SoilTemperature100To255cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture0To7cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture7To28cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture28To100cm = table.Column<double>(type: "float", nullable: true),
                    SoilMoisture100To255cm = table.Column<double>(type: "float", nullable: true),
                    Humidex = table.Column<double>(type: "float", nullable: true),
                    AirDensity = table.Column<double>(type: "float", nullable: true),
                    WindPowerDensity = table.Column<double>(type: "float", nullable: true),
                    PhotoThermalUnit = table.Column<double>(type: "float", nullable: true),
                    CloudRadiativeForcing = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TmyReadings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HourlyReadings_Time",
                table: "HourlyReadings",
                column: "Time",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TmyReadings_Time",
                table: "TmyReadings",
                column: "Time",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HourlyReadings");

            migrationBuilder.DropTable(
                name: "TmyReadings");

            migrationBuilder.DropSequence(
                name: "HourlyReadingSequence");
        }
    }
}
