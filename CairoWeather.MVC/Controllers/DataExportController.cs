using CairoWeather.Services;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;
using System.Text.Json;

namespace CairoWeather.MVC.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataExportController : ControllerBase
    {
        private readonly IWeatherAnalyticsService _analyticsService;

        public DataExportController(IWeatherAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("download")]
        public async Task<IActionResult> DownloadData([FromQuery] string format, [FromQuery] string timeframe, [FromQuery] string param)
        {
            object realData = null;
            string fileNameContext = "Data";

            // 1. Fetch the exact same data the AI uses based on the UI context
            switch (timeframe.ToLower())
            {
                case "hourly":
                    if (DateTime.TryParse(param, out DateTime date))
                    {
                        realData = await _analyticsService.GetHourlyAnalyticsAsync(date);
                        fileNameContext = $"Hourly_{date:yyyy-MM-dd}";
                    }
                    break;
                case "monthly":
                    var parts = param.Split('-');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int y) && int.TryParse(parts[1], out int m))
                    {
                        realData = await _analyticsService.GetMonthlyAnalyticsAsync(y, m);
                        fileNameContext = $"Monthly_{y}-{m:D2}";
                    }
                    break;
                case "yearly":
                    if (int.TryParse(param, out int year))
                    {
                        realData = await _analyticsService.GetYearlyAnalyticsAsync(year);
                        fileNameContext = $"Yearly_{year}";
                    }
                    break;
                case "decadal":
                    realData = await _analyticsService.GetDecadalAnalyticsAsync(2015, 2025);
                    fileNameContext = "Decadal_2015-2025";
                    break;
            }

            if (realData == null) return NotFound("Data not found for the selected parameters.");

            // 2. Convert the dynamic C# object into a clean standard data table
            DataTable table = ConvertToDataTable(realData, fileNameContext);

            // 3. Return the requested file format
            if (format.ToLower() == "csv")
            {
                return GenerateCsvFile(table, $"CairoWeather_{fileNameContext}.csv");
            }
            else if (format.ToLower() == "xlsx")
            {
                return GenerateExcelFile(table, $"CairoWeather_{fileNameContext}.xlsx");
            }

            return BadRequest("Invalid format. Use 'csv' or 'xlsx'.");
        }

        // --- HELPER: GENERATE CSV ---
        private FileResult GenerateCsvFile(DataTable table, string fileName)
        {
            var sb = new StringBuilder();

            // Headers
            string[] columnNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToArray();
            sb.AppendLine(string.Join(",", columnNames));

            // Rows
            foreach (DataRow row in table.Rows)
            {
                string[] fields = row.ItemArray.Select(field => field?.ToString()?.Replace(",", " ")).ToArray();
                sb.AppendLine(string.Join(",", fields));
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", fileName);
        }

        // --- HELPER: GENERATE EXCEL (Using ClosedXML) ---
        private FileResult GenerateExcelFile(DataTable table, string fileName)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(table, "Weather Data");
            worksheet.Columns().AdjustToContents(); // Auto-fit columns

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        // --- HELPER: DYNAMIC DATA FLATTENER ---
        // This takes whatever object your service returns and dynamically builds a table from it.
        private DataTable ConvertToDataTable(object data, string tableName)
        {
            var table = new DataTable(tableName);
            var jsonOptions = new JsonSerializerOptions { ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles };
            var jsonString = JsonSerializer.Serialize(data, jsonOptions);
            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            var dataRows = new Dictionary<string, Dictionary<string, string>>();
            var allColumnNames = new SortedSet<string>();

            // SAFETY CHECK: Only iterate if the root is an Object
            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in root.EnumerateObject())
                {
                    var val = property.Value;

                    // CRITICAL FIX: Only call TryGetProperty if 'val' is an Object
                    if (val.ValueKind == JsonValueKind.Object)
                    {
                        bool hasLabels = val.TryGetProperty("labels", out var labels) || val.TryGetProperty("Labels", out labels);
                        bool hasDatasets = val.TryGetProperty("datasets", out var datasets) || val.TryGetProperty("Datasets", out datasets);

                        if (hasLabels && hasDatasets && labels.ValueKind == JsonValueKind.Array)
                        {
                            var labelsList = labels.EnumerateArray().Select(x => x.ToString()).ToList();

                            foreach (var dataset in datasets.EnumerateObject())
                            {
                                string colName = $"{property.Name}_{dataset.Name}";
                                allColumnNames.Add(colName);
                                var dataPoints = dataset.Value.EnumerateArray().ToList();

                                for (int i = 0; i < labelsList.Count; i++)
                                {
                                    string timeLabel = labelsList[i];
                                    if (!dataRows.ContainsKey(timeLabel)) dataRows[timeLabel] = new Dictionary<string, string>();
                                    if (i < dataPoints.Count) dataRows[timeLabel][colName] = dataPoints[i].ToString();
                                }
                            }
                        }
                    }
                }
            }

            // 2. Build the actual DataTable
            if (dataRows.Count > 0)
            {
                table.Columns.Add("Timeframe");
                foreach (var col in allColumnNames) table.Columns.Add(col);

                foreach (var entry in dataRows)
                {
                    var row = table.NewRow();
                    row["Timeframe"] = entry.Key;
                    foreach (var col in allColumnNames)
                    {
                        row[col] = entry.Value.ContainsKey(col) ? entry.Value[col] : "";
                    }
                    table.Rows.Add(row);
                }
            }
            // Fallback for simple List responses
            else if (root.ValueKind == JsonValueKind.Array)
            {
                var first = root.EnumerateArray().FirstOrDefault();
                if (first.ValueKind == JsonValueKind.Object)
                {
                    foreach (var prop in first.EnumerateObject()) table.Columns.Add(prop.Name);
                    foreach (var element in root.EnumerateArray())
                    {
                        var row = table.NewRow();
                        foreach (var prop in element.EnumerateObject()) row[prop.Name] = prop.Value.ToString();
                        table.Rows.Add(row);
                    }
                }
            }

            return table;
        }
    }
}