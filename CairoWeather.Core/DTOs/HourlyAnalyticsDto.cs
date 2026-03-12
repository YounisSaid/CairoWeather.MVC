namespace CairoWeather.Core.DTOs
{
    // Used by Section 1: Hourly
    public class HourlyAnalyticsDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public Dictionary<string, List<double>> Datasets { get; set; } = new Dictionary<string, List<double>>();
        public WeatherStats Summary { get; set; }
    }

    // Used inside HourlyAnalyticsDto for the top KPI cards
    public class WeatherStats
    {
        public double Temp { get; set; }
        public double ApparentTemp { get; set; }
        public double Humidity { get; set; }
        public double Radiation { get; set; }
        public double WindSpeed { get; set; }
        public double WindGust { get; set; }
        public double Et0 { get; set; }
        public double Vpd { get; set; }
        public double Pressure { get; set; }
        public double CloudCover { get; set; }
        public double DewPoint { get; set; }
    }

    // Used by Section 2 (Yearly) and Section 3 (Decade)
    public class MetricChartDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<double> Values { get; set; } = new List<double>();
    }
}