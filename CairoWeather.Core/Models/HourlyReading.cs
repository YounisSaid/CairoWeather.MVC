using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CairoWeather.Core.Models
{
    public class HourlyReading
    {
        [Key]
        [Ignore] // Ignore Id during CSV mapping
        public int Id { get; set; }

        public DateTime Time { get; set; }

        // Thermal Dynamics
        public double? Temperature2m { get; set; }
        public double? RelativeHumidity2m { get; set; }
        public double? DewPoint2m { get; set; }
        public double? ApparentTemperature { get; set; }

        // Barometric & Cloud
        public int? WeatherCode { get; set; }
        public double? PressureMsl { get; set; }
        public double? SurfacePressure { get; set; }
        public double? CloudCover { get; set; }
        public double? CloudCoverLow { get; set; }
        public double? CloudCoverMid { get; set; }
        public double? CloudCoverHigh { get; set; }

        // Solar Radiation (Required for Digital Twin calculations)
        public double? ShortwaveRadiation { get; set; }
        public double? ClearSkyRadiation { get; set; }

        // Evaporation
        public double? Et0FaoEvapotranspiration { get; set; }
        public double? VapourPressureDeficit { get; set; } // Native Open-Meteo VPD

        // Wind Metrics
        public double? WindSpeed10m { get; set; }
        public double? WindSpeed100m { get; set; }
        public double? WindDirection10m { get; set; }
        public double? WindDirection100m { get; set; }
        public double? WindGusts10m { get; set; }

        // Soil Temperatures
        public double? SoilTemperature0To7cm { get; set; }
        public double? SoilTemperature7To28cm { get; set; }
        public double? SoilTemperature28To100cm { get; set; }
        public double? SoilTemperature100To255cm { get; set; }

        // Soil Moistures
        public double? SoilMoisture0To7cm { get; set; }
        public double? SoilMoisture7To28cm { get; set; }
        public double? SoilMoisture28To100cm { get; set; }
        public double? SoilMoisture100To255cm { get; set; }

        // --- NEW DERIVED VARIABLES (Calculated in Seeder) ---

        [Ignore]
        public double? Humidex { get; set; }

        [Ignore]
        public double? AirDensity { get; set; }

        [Ignore]
        public double? WindPowerDensity { get; set; }

        [Ignore]
        public double? PhotoThermalUnit { get; set; }

        [Ignore]
        public double? CloudRadiativeForcing { get; set; }
    }
}