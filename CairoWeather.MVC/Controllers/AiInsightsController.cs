using CairoWeather.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace CairoWeather.MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiInsightsController : ControllerBase
    {
        private readonly IWeatherAnalyticsService _analyticsService;
        private readonly IConfiguration _configuration;

        public AiInsightsController(IWeatherAnalyticsService analyticsService, IConfiguration configuration)
        {
            _analyticsService = analyticsService;
            _configuration = configuration;
        }

        [HttpGet("generate")]
        public async Task<IActionResult> GenerateInsight([FromQuery] string timeframe, [FromQuery] string param)
        {
            try
            {
                object realData = null;
                string htmlTemplate = "";
                string promptContext = "";

                // 1. Fetch REAL Data & Assign Exact HTML Templates to keep your styling
                switch (timeframe.ToLower())
                {
                    case "hourly":
                        if (DateTime.TryParse(param, out DateTime date))
                        {
                            // isTmy = false for historical data
                            realData = await _analyticsService.GetHourlyAnalyticsAsync(date, isTmy: false);
                            promptContext = $"24-Hour Historical Micro-Climate data for {date:yyyy-MM-dd}";

                            htmlTemplate = @"<h3 style='color:#0f172a; font-weight:800;'>Historical Hourly Analysis</h3>
                            <h6 style='color:#f59e0b; text-transform:uppercase; font-weight:700;'>Date Analyzed: " + date.ToString("yyyy-MM-dd") + @"</h6>
                            <hr style='border-color:#e2e8f0; margin: 15px 0;'>
                            <p>Based on our real-time thermodynamic modeling, here is the detailed historical analysis:</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-chart-line me-2'></i>1. Radiation vs Air Temp (Thermal Lag)</h6>
                            <p style='font-size:0.9rem;'>[AI: Write 2 sentences analyzing the thermal time lag between GHI and Air Temp based on the data.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-wind me-2'></i>2. Wind Shear Profile</h6>
                            <p style='font-size:0.9rem;'>[AI: Write 2 sentences comparing 10m vs 100m wind speeds from the data.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-cloud me-2'></i>3. Cloud Cover Attenuation</h6>
                            <p style='font-size:0.9rem;'>[AI: Write 2 sentences analyzing cloud cover impact on solar radiation.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-fire me-2'></i>4. Apparent Heat Stress</h6>
                            <p style='font-size:0.9rem;'>[AI: Write 2 sentences analyzing the Apparent Temperature vs actual Temperature. Cite peak heat stress numbers.]</p>
                            <div style='background-color:#fef3c7; border-left:4px solid #f59e0b; padding:15px; margin-top:25px; border-radius:4px;'>
                                <h6 style='color:#b45309; font-weight:bold; margin-bottom:5px;'>Strategic Recommendations:</h6>
                                <ul style='margin-bottom:0; padding-left:20px; font-size:0.85rem; color:#78350f;'>
                                    <li style='margin-bottom:5px;'><b>[AI: Short Title 1]:</b> [AI: Specific operational recommendation based on the data]</li>
                                    <li><b>[AI: Short Title 2]:</b> [AI: Second operational recommendation based on the data]</li>
                                </ul>
                            </div>";
                        }
                        break;

                    // =========================================================
                    // الحـالـة الجـديـدة الخـاصـة بـ TMY 
                    // =========================================================
                    case "tmy":
                        if (DateTime.TryParse(param, out DateTime tmyDate))
                        {
                            // isTmy = true to read from the 2025 Baseline table
                            realData = await _analyticsService.GetHourlyAnalyticsAsync(tmyDate, isTmy: true);
                            promptContext = $"Typical Meteorological Year (TMY 2025 Baseline) design data for {tmyDate:yyyy-MM-dd}";

                            htmlTemplate = @"<h3 style='color:#0f172a; font-weight:800;'>TMY Baseline Analysis</h3>
                            <h6 style='color:#10b981; text-transform:uppercase; font-weight:700;'>Design Date: " + tmyDate.ToString("MMM dd, yyyy") + @"</h6>
                            <hr style='border-color:#e2e8f0; margin: 15px 0;'>
                            <p>Based on the Typical Meteorological Year algorithm, here is the standard design condition analysis:</p>
                            <h6 style='color:#10b981; margin-top:20px; font-weight:700;'><i class='fa-solid fa-sun me-2'></i>1. Standard Radiative Profile</h6>
                            <p style='font-size:0.9rem;'>[AI: Write 2 sentences analyzing the typical thermal lag and peak irradiance for this baseline day.]</p>
                            <h6 style='color:#10b981; margin-top:20px; font-weight:700;'><i class='fa-solid fa-wind me-2'></i>2. Baseline Wind Stresses</h6>
                            <p style='font-size:0.9rem;'>[AI: Write 2 sentences evaluating expected wind loads and gusts for structural design.]</p>
                            <h6 style='color:#10b981; margin-top:20px; font-weight:700;'><i class='fa-solid fa-cloud-sun me-2'></i>3. Typical Cloud Attenuation</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze standard cloud cover impact on PV generation for this day.]</p>
                            <h6 style='color:#10b981; margin-top:20px; font-weight:700;'><i class='fa-solid fa-droplet me-2'></i>4. Condensation & Soiling Risk</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze the gap between Temperature and Dew Point to assess panel condensation and mud formation risks.]</p>
                            <div style='background-color:#d1fae5; border-left:4px solid #10b981; padding:15px; margin-top:25px; border-radius:4px;'>
                                <h6 style='color:#065f46; font-weight:bold; margin-bottom:5px;'>Engineering Design Recommendations:</h6>
                                <ul style='margin-bottom:0; padding-left:20px; font-size:0.85rem; color:#064e3b;'>
                                    <li style='margin-bottom:5px;'><b>[AI: Short Title 1]:</b> [AI: Specific engineering sizing/design recommendation based on this TMY data]</li>
                                    <li><b>[AI: Short Title 2]:</b> [AI: Operational maintenance protocol based on this TMY data]</li>
                                </ul>
                            </div>";
                        }
                        break;

                    case "monthly":
                        var parts = param.Split('-');
                        if (parts.Length == 2 && int.TryParse(parts[0], out int y) && int.TryParse(parts[1], out int m))
                        {
                            realData = await _analyticsService.GetMonthlyAnalyticsAsync(y, m);
                            promptContext = $"30-Day Aggregation data for {y}-{m:D2}";

                            htmlTemplate = @"<h3 style='color:#0f172a; font-weight:800;'>30-Day Radiative Aggregation</h3>
                            <h6 style='color:#8b5cf6; text-transform:uppercase; font-weight:700; letter-spacing:1px;'>Period Analyzed: " + $"{y}-{m:D2}" + @"</h6>
                            <hr style='border-color:#e2e8f0; margin: 15px 0;'>
                            <p>The trailing analysis reveals these macro-patterns impacting total energy capture:</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-cloud-sun me-2'></i>1. Cloud Radiative Forcing</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze Clear Sky vs Actual GHI from data.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-fan me-2'></i>2. Radiation-Wind Synergy</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze if high wind compensated for low solar days based on data.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-temperature-arrow-up me-2'></i>3. Diurnal Temp Range (DTR)</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze daily temp spreads.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'><i class='fa-solid fa-smog me-2'></i>4. Fog Risk Profile</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze Min Temp vs Dew Point convergence for fog risk.]</p>
                            <div style='background-color:#e0f2fe; border-left:4px solid #3b82f6; padding:15px; margin-top:25px; border-radius:4px;'>
                                <h6 style='color:#0369a1; font-weight:bold; margin-bottom:5px;'>Strategic Recommendations:</h6>
                                <ul style='margin-bottom:0; padding-left:20px; font-size:0.85rem; color:#0c4a6e;'>
                                    <li style='margin-bottom:5px;'><b>[AI: Short Title 1]:</b> [AI: Specific operational recommendation]</li>
                                    <li><b>[AI: Short Title 2]:</b> [AI: Second operational recommendation]</li>
                                </ul>
                            </div>";
                        }
                        break;

                    case "yearly":
                        if (int.TryParse(param, out int year))
                        {
                            realData = await _analyticsService.GetYearlyAnalyticsAsync(year);
                            promptContext = $"Annual Seasonality data for {year}";

                            htmlTemplate = @"<h3 style='color:#0f172a; font-weight:800;'>Annual Seasonality Report</h3>
                            <h6 style='color:#10b981; text-transform:uppercase; font-weight:700; letter-spacing:1px;'>Fiscal Year: " + year + @"</h6>
                            <hr style='border-color:#e2e8f0; margin: 15px 0;'>
                            <p>The 365-day Earth/Sun analytics show distinct seasonal shifts:</p>
                            <div style='display:grid; grid-template-columns: 1fr 1fr; gap:15px; margin-top:20px;'>
                                <div>
                                    <h6 style='color:#3b82f6; font-size:0.85rem; font-weight:700;'>1. Radiative Hysteresis</h6>
                                    <p style='font-size:0.8rem; line-height:1.4;'>[AI: Analyze the thermal lag across the year based on the data.]</p>
                                </div>
                                <div>
                                    <h6 style='color:#3b82f6; font-size:0.85rem; font-weight:700;'>2. Photo-Thermal Units</h6>
                                    <p style='font-size:0.8rem; line-height:1.4;'>[AI: Analyze PTU accumulation.]</p>
                                </div>
                                <div>
                                    <h6 style='color:#3b82f6; font-size:0.85rem; font-weight:700;'>3. Aridity Index</h6>
                                    <p style='font-size:0.8rem; line-height:1.4;'>[AI: Analyze Net Radiation vs Latent heat.]</p>
                                </div>
                                <div>
                                    <h6 style='color:#3b82f6; font-size:0.85rem; font-weight:700;'>4. Extreme Gusts</h6>
                                    <p style='font-size:0.8rem; line-height:1.4;'>[AI: Analyze the extreme gust days across the months.]</p>
                                </div>
                            </div>
                            <div style='background-color:#d1fae5; border-left:4px solid #10b981; padding:15px; margin-top:25px; border-radius:4px;'>
                                <h6 style='color:#065f46; font-weight:bold; margin-bottom:5px;'>Strategic Recommendations:</h6>
                                <ul style='margin-bottom:0; padding-left:20px; font-size:0.85rem; color:#064e3b;'>
                                    <li style='margin-bottom:5px;'><b>[AI: Short Title 1]:</b> [AI: Specific operational recommendation]</li>
                                    <li><b>[AI: Short Title 2]:</b> [AI: Second operational recommendation]</li>
                                </ul>
                            </div>";
                        }
                        break;

                    case "decadal":
                        realData = await _analyticsService.GetDecadalAnalyticsAsync(2015, 2025);
                        promptContext = "Decadal Macro-Trends from 2015 to 2025";

                        htmlTemplate = @"<h3 style='color:#0f172a; font-weight:800;'>Decadal Macro-Trends</h3>
                            <h6 style='color:#ef4444; text-transform:uppercase; font-weight:700; letter-spacing:1px;'>Span: 2015 - 2025</h6>
                            <hr style='border-color:#e2e8f0; margin: 15px 0;'>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'>1. Global Dimming vs UHI Decoupling</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze the 10-year trend between Temperature and GHI. Mention Urban Heat Island effect.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'>2. Sensible Heat Escalation</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze the trend in sensible heat escalation over the decade.]</p>
                            <h6 style='color:#3b82f6; margin-top:20px; font-weight:700;'>3. Wind Stagnation & Extremes</h6>
                            <p style='font-size:0.9rem;'>[AI: Analyze the average wind speeds vs peak extreme gusts over the 10 years.]</p>
                            <div style='background-color:#fee2e2; border-left:4px solid #ef4444; padding:15px; margin-top:25px; border-radius:4px;'>
                                <h6 style='color:#991b1b; font-weight:bold; margin-bottom:5px;'>Decadal Policy Recommendations:</h6>
                                <ul style='margin-bottom:0; padding-left:20px; font-size:0.85rem; color:#7f1d1d;'>
                                    <li style='margin-bottom:5px;'><b>[AI: Short Title 1]:</b> [AI: Strategic urban planning recommendation based on data.]</li>
                                    <li><b>[AI: Short Title 2]:</b> [AI: Long-term energy/infrastructure recommendation.]</li>
                                </ul>
                            </div>";
                        break;

                    default:
                        return Ok(new { reportHtml = "<div class='alert alert-warning'>Invalid timeframe selected.</div>" });
                }

                if (realData == null)
                {
                    return Ok(new { reportHtml = $"<div class='alert alert-warning'><i class='fa-solid fa-database me-2'></i><strong>No Data Found:</strong><br/>We couldn't find any weather records in the database for the parameter: '{param}'. Please ensure you selected a valid date/month that has data.</div>" });
                }

                // FIX: Ignore Circular References which cause 500 Server Crashes
                var jsonOptions = new System.Text.Json.JsonSerializerOptions
                {
                    ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
                };
                string rawJsonData = System.Text.Json.JsonSerializer.Serialize(realData, jsonOptions);

                // 2. The Master System Prompt
                string systemPrompt = $@"
       You are a senior climate engineer analyzing Cairo's weather data.
       You output ONLY raw HTML. No markdown formatting. Follow the template exactly.
       
       RAW JSON DATA for {promptContext}:
       {rawJsonData}

       TASK:
       Generate a highly professional HTML report. Replace the bracketed instructions [AI: ...] with real analysis derived ONLY from the JSON data. 
       Do NOT use markdown. Do NOT output ```html. Output only the raw HTML string.
       Cite exact numbers, averages, and extremes from the JSON in your paragraphs.

       HTML TEMPLATE:
       {htmlTemplate}
   ";

                string aiHtmlResponse = await CallGeminiAsync(systemPrompt);
                return Ok(new { reportHtml = aiHtmlResponse });
            }
            catch (Exception ex)
            {
                // Output detailed error into UI panel
                return Ok(new { reportHtml = $"<div class='alert alert-danger'><strong>Backend Crash (500 Error):</strong><br/>{ex.Message}</div>" });
            }
        }
        private async Task<string> CallGeminiAsync(string prompt)
        {
            // 1. Get the key safely
            string apiKey = _configuration["Gemini:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return "<div class='alert alert-danger'><strong>Error:</strong> API Key is missing. C# cannot find 'Gemini:ApiKey' in appsettings.json. Check appsettings.Development.json too!</div>";
            }

            // Clean any invisible spaces off the key
            apiKey = apiKey.Trim();

            // 2. Build the URL
            // Use this if the flash-latest one above still gives you a 404
            string endpoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key=" + apiKey;

            using var client = new HttpClient();

            var requestBody = new
            {
                contents = new[] { new { parts = new[] { new { text = prompt } } } },
                generationConfig = new { temperature = 0.3 }
            };

            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // 3. Send the Request
                var response = await client.PostAsync(endpoint, content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    return $"<div class='alert alert-danger'><strong>Google API Rejected Request:</strong><br/>{error}</div>";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var jsonDoc = JsonDocument.Parse(responseJson);

                // Extract the generated text
                string generatedHtml = jsonDoc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text").GetString();

                return generatedHtml.Replace("```html", "").Replace("```", "").Trim();
            }
            catch (UriFormatException ex)
            {
                // If the URL breaks, print EXACTLY what it tried to send to find the bug
                return $"<div class='alert alert-danger'><strong>URI Format Crash:</strong><br/>The URL got mangled. <br/><strong>Tried to ping:</strong> {endpoint}<br/><strong>Error:</strong> {ex.Message}</div>";
            }
            catch (Exception ex)
            {
                return $"<div class='alert alert-danger'><strong>HTTP Client Crash:</strong><br/>{ex.Message}</div>";
            }
        }
    }
}