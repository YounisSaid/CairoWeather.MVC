using CsvHelper.Configuration.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CairoWeather.Core.Models
{
    public class DailyReading
    {
        [Key]
        [Ignore] // 2. Tell CSV reader to ignore the database ID
        public int Id { get; set; }

        [Name("time")] // 3. Tell CSV reader to map the "time" column to this Date property
        public DateTime Date { get; set; }

        // Main Variables (Matched exactly to your screenshot)
        public int WeatherCode { get; set; }
        public double Temperature2mMean { get; set; }
        public double Temperature2mMax { get; set; }
        public double Temperature2mMin { get; set; }
        public double ApparentTemperatureMean { get; set; }
        public double ApparentTemperatureMax { get; set; }
        public double ApparentTemperatureMin { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }
        public double DaylightDuration { get; set; }
        public double SunshineDuration { get; set; }
        public double WindSpeed10mMax { get; set; }
        public double WindGusts10mMax { get; set; }
        public double WindDirection10mDominant { get; set; }
        public double ShortwaveRadiationSum { get; set; }
        public double Et0FaoEvapotranspirationSum { get; set; }

        // Additional 38/38 Variables
        public double CloudCoverMean { get; set; }
        public double CloudCoverMax { get; set; }
        public double CloudCoverMin { get; set; }
        public double DewPoint2mMean { get; set; }
        public double DewPoint2mMax { get; set; }
        public double DewPoint2mMin { get; set; }
        public double RelativeHumidity2mMean { get; set; }
        public double RelativeHumidity2mMax { get; set; }
        public double RelativeHumidity2mMin { get; set; }
        public double PressureMslMean { get; set; }
        public double PressureMslMax { get; set; }
        public double PressureMslMin { get; set; }
        public double SurfacePressureMean { get; set; }
        public double SurfacePressureMax { get; set; }
        public double SurfacePressureMin { get; set; }
        public double WindSpeed10mMean { get; set; }
        public double WindSpeed10mMin { get; set; }
        public double WindGusts10mMean { get; set; }
        public double WindGusts10mMin { get; set; }
        public double WetBulbTemperature2mMean { get; set; }
        public double WetBulbTemperature2mMax { get; set; }
        public double WetBulbTemperature2mMin { get; set; }
        public double VapourPressureDeficitMax { get; set; }

        // Extensive Soil
        public double SoilMoisture0To100cmMean { get; set; }
        public double SoilMoisture0To7cmMean { get; set; }
        public double SoilMoisture7To28cmMean { get; set; }
        public double SoilMoisture28To100cmMean { get; set; }
        public double SoilTemperature0To100cmMean { get; set; }
        public double SoilTemperature0To7cmMean { get; set; }
        public double SoilTemperature7To28cmMean { get; set; }
        public double SoilTemperature28To100cmMean { get; set; }

        // Snow (Checked in additional vars)
        public double SnowfallWaterEquivalentSum { get; set; }
    }
}