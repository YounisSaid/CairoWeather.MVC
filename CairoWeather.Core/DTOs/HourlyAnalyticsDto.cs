namespace CairoWeather.Core.DTOs
{
    public class ChartDataDto
    {
        public List<string> Labels { get; set; } = new List<string>();
        public Dictionary<string, List<double>> Datasets { get; set; } = new Dictionary<string, List<double>>();
    }

    public class HourlyKpiStats
    {
        public double Temperature { get; set; }
        public double ApparentTemperature { get; set; }
        public double CloudCover { get; set; }
        public double WindSpeed100m { get; set; }
        public double WindSpeed10m { get; set; }
        public double VapourPressureDeficit { get; set; }
        public double DewPoint { get; set; }
        public double Et0Evapotranspiration { get; set; }
        // --- ADDED THESE 3 NEW VARIABLES ---
        public double WindGusts10m { get; set; }
        public double RelativeHumidity { get; set; }
        public double SurfacePressure { get; set; }
    }

    public class HourlyDashboardDto
    {
        public HourlyKpiStats KpiSummary { get; set; }

        // ADDED: Holds the exact values for all 24 hours so the slider can read them
        public List<HourlyKpiStats> HourlyLog { get; set; }

        public ChartDataDto RadiativeTimeLag { get; set; }
        public ChartDataDto WindShearProfile { get; set; }
        public ChartDataDto CloudCoverThickness { get; set; }
        public ChartDataDto ApparentHeatStress { get; set; }
        public ChartDataDto MoistureDynamics { get; set; }
    }

    public class MonthlyDashboardDto
    {
        public ChartDataDto CloudRadiativeForcing { get; set; }
        public ChartDataDto RadiationWindSynergy { get; set; }
        public ChartDataDto DiurnalTempRange { get; set; }
        public ChartDataDto FogRisk { get; set; }
    }

    public class YearlyDashboardDto
    {
        public ChartDataDto AnnualRadiativeHysteresis { get; set; }
        public ChartDataDto PhotoThermalUnits { get; set; }
        public ChartDataDto RadiativeAridityIndex { get; set; }
        public ChartDataDto NetLongwaveSeasonality { get; set; }
        public ChartDataDto ExtremeGustDays { get; set; }
        public ChartDataDto CloudCoverStrata { get; set; }
        public ChartDataDto ApparentTempEnvelope { get; set; }
    }

    public class DecadalDashboardDto
    {
        public ChartDataDto GlobalDimmingTrend { get; set; }
        public ChartDataDto UrbanRadiativeDecoupling { get; set; }
        public ChartDataDto DecadalCloudCoverShift { get; set; }
        public ChartDataDto SensibleHeatEscalation { get; set; }
        public ChartDataDto WindStagnationVsGusts { get; set; }
    }
}