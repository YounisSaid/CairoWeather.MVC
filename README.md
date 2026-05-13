# 🌞 Cairo Radiative Climate Digital Twin

An advanced, interactive platform for Solar ☀️, Thermodynamics 🌡️, and Micro-climate monitoring 🌦️, bridging theoretical physics with real-world historical data and AI-driven insights.

## 📖 Project Overview

Cairo Radiative Climate Digital Twin is a comprehensive engineering and meteorological web application built for solar engineers 👷‍♂️, academic researchers 🔬, and infrastructure planners 🗺️.

The platform moves far beyond static estimations by utilizing a rigorous C# physics engine paired with over a decade of historical weather records sourced from Open-Meteo. It offers a 360-degree view of environmental dynamics, generates a Typical Meteorological Year (TMY) using the empirical Sandia Method, and visualizes complex solar phenomena through real-time, interactive "Digital Twins".

**Live Demo:** 🔗 [https://cairoweather.runasp.net](https://cairoweather.runasp.net)

---

## 💡 What Does This App Solve?

Designing solar energy infrastructure often relies on static, generic weather data or rigid spreadsheet estimations, leading to over-engineered (costly) or under-performing plants. This application solves this problem by:

* 🗑️ **Eliminating the "Garbage In, Garbage Out" Problem:** By utilizing 10 years of validated historical data, the engine synthesizes a highly accurate Typical Meteorological Year (TMY) specific to the micro-climate.
* 💵 **Bridging Physics and Finance:** It translates complex thermodynamic losses (like temperature derating, dust accumulation, and inverter clipping) directly into tangible financial metrics (e.g., EGP lost per month).
* 🎛️ **Interactive "What-If" Scenarios & Yield Comparison:** It replaces static spreadsheets with interactive Digital Twins and an advanced Station Configurator. Engineers can visually drag sliders to compare yields across different topologies (e.g., Fixed vs. Tracking, Mono vs. Poly) and instantly see the physical and financial impact of their design choices.
* 🤖 **Automating Engineering Reports:** Powered by AI, it instantly translates massive meteorological datasets into readable strategic recommendations for infrastructure planning and thermal load mitigation.

---

## ✨ Core Modules & Navigation Tabs

The application is structured into comprehensive tabs, offering varying granularities of data and simulation tools:

### 🌍 Historical & Analytical Dashboards
* ⏱️ **Historical Hourly:** Features an interactive 24-hour slider to inspect exact meteorological conditions (Midnight to Midnight), including Humidex, Dew Point, VPD, and Evapotranspiration (ETo).
* 📅 **1-Month Analysis:** Granular, day-by-day inspection of a specific month.
* 🔄 **1-Year Seasonality:** Annual trends mapped to identify seasonal peaks and thermal loads.
* 📈 **10-Year Trends:** Decadal historical data analysis to spot long-term climate shifts, Urban Heat Island (UHI) anomalies, and extreme gust frequencies.
* 🎯 **TMY (2025 Baseline):** The synthesized "Perfect Year" generated via the Sandia Method, acting as the gold standard for solar yield forecasting.
* 🔍 **Data Provenance:** Full transparency tracking the origin of the atmospheric data back to the Open-Meteo API.
* 📥 **Universal Export:** 1-click downloads to CSV and Excel across all data views for external processing.

### 🧠 AI Insights (Powered by Gemini)
* 💡 **Generate AI Insights:** Powered by Google's Gemini 2.5 Flash, this module reads the raw dashboard data and generates automated, strategic text reports. It provides actionable recommendations for:
  * 🔥 Thermal Load Management & Urban Heat Island (UHI) profiling.
  * 🏜️ Aridity Index tracking.
  * 💨 Wind Impact and Extreme Gust Mitigation.

### ⚙️ The Physics Engine (DataSeeder)
* 🧮 **Physics Formulas Tab:** A dedicated library displaying every mathematical formula running in the C# backend. From cell temperature (NOCT) to Lambert's Cosine Law and bypass diode mechanics, everything is transparent.
* 🌤️ **Solar Radiation Fallback Modeling:** Physically calculates Clear-Sky Radiation using solar hour angles when raw irradiance data is missing.
* 📐 **Thermodynamic Derivations:** Calculates Moist Air Density, Photo-Thermal Units (PTU), and Wind Power Density dynamically.

### 🎮 Interactive Solar Digital Twins (Simulators)
* 🌡️ **PV Thermodynamics & Bypass Logic:** Simulates Cell Temperature (NOCT) thermal derating alongside partial shading impacts. It dynamically demonstrates how bypass diodes activate to rescue power output when localized soiling or shading occurs across the module strings.
* 🏗️ **Advanced Station Configurator & Yield Comparison:** A comprehensive module to configure full solar stations. Run side-by-side yield comparisons by adjusting parameters (panel capacity, tilt, and degradation limits) to find the most financially viable configuration.
* 🔄 **Single-Axis Tracking Simulator:** Visualizes Lambert's Cosine Law and Air Mass attenuation comparing Fixed-Tilt arrays versus tracked arrays in real-time.
* ✂️ **Inverter Clipping Visualizer:** Demonstrates the financial/physical impact of DC/AC Ratio over-sizing using the Web Animations API (WAAPI) to show energy bottlenecks.
* 🧹 **Dust & Soiling Impact Simulator:** Calculates degradation over consecutive dry days and translates physical wattage drops into direct financial waste.
* 🏠 **Household Rooftop Planner:** A drag-and-drop grid tool linking physical panel placement (Mono 450W / Poly 350W) directly to monthly financial goals and tariffs.
* ☀️ **Bifacial Albedo Calculator:** Computes rear-side power generation based on ground reflectance coefficients (e.g., Asphalt vs. Desert Sand).

---
## ✨ Full Project Structure
The project strictly follows the N-Tier Clean Architecture to decouple the physics engine from the presentation layer.
```text
CairoWeather.MVC/
├── CairoWeather.sln                                # Main Solution File linking all architectural layers
├── README.md                                       # Project documentation
├── LICENSE.txt                                     # MIT License
│
├── CairoWeather.Core/                              # 1️⃣ DOMAIN LAYER (Class Library)
│   ├── CairoWeather.Core.csproj
│   └── Models/
│       ├── HourlyReading.cs                        # Base entity for historical hourly weather data
│       ├── TmyReading.cs                           # Entity for the synthesized Typical Meteorological Year
│       ├── AiInsightRequest.cs                     # DTO for Gemini 2.5 Flash prompts
│       ├── MonthlySummary.cs                       # Aggregated model for 1-Month Analysis charts
│       ├── YearlySummary.cs                        # Aggregated model for 1-Year Seasonality charts
│       └── DecadeTrend.cs                          # Aggregated model for 10-Year historical trends
│
├── CairoWeather.Data/                              # 2️⃣ DATA ACCESS LAYER (Class Library)
│   ├── CairoWeather.Data.csproj
│   ├── DbContexts/
│   │   └── EnergyDbContext.cs                      # Main EF Core DbContext mapping models to SQL tables
│   └── Migrations/
│       ├── 202405..._InitialCreate.cs              # Auto-generated EF Core migration files
│       └── EnergyDbContextModelSnapshot.cs         # Current snapshot of the database schema
│
└── CairoWeather.MVC/                               # 3️⃣ PRESENTATION & SERVICE LAYER (Web App)
    ├── CairoWeather.MVC.csproj
    ├── Program.cs                                  # Application entry point, middleware pipeline & DI container
    ├── appsettings.json                            # Local configurations (DB Connection, Gemini API Key)
    ├── appsettings.Development.json                # Dev-specific configuration overrides
    │
    ├── Controllers/
    │   ├── HomeController.cs                       # Handles micro-climate dashboards, historical trends, AI Insights
    │   ├── SimulatorsController.cs                 # Routes for the interactive Solar Digital Twins
    │   └── ExportController.cs                     # Manages CSV and Excel generation and file downloads
    │
    ├── Data/
    │   └── Seeding/
    │       └── DataSeeder.cs                       # The Physics Engine: TMY Synthesis, Thermodynamics & CsvHelper
    │
    ├── Services/
    │   ├── GeminiAiService.cs                      # Service handling REST calls to Google Gemini API
    │   └── PhysicsEngineService.cs                 # Centralized C# thermodynamics math calculations
    │
    ├── Views/
    │   ├── Home/
    │   │   ├── Index.cshtml                        # Main Analytical Dashboard (24H slider, historical charts)
    │   │   └── _AiInsightsPartial.cshtml           # Partial view for dynamically loaded Gemini text reports
    │   ├── Simulators/
    │   │   ├── BifacialAlbedoCalculator.cshtml     # Rear-side generation twin (Ground reflectance)
    │   │   ├── DustSoilingSimulator.cshtml         # Degradation & Financial Waste twin (Wash animation)
    │   │   ├── Formulas.cshtml                     # Transparent physics formulas and documentation library
    │   │   ├── HouseholdRooftopPlanner.cshtml      # Drag-and-drop PV configuration twin (Grid UI)
    │   │   ├── InverterClippingSimulator.cshtml    # WAAPI Inverter Bottleneck twin (DC/AC Ratio)
    │   │   ├── SolarTrackingSimulator.cshtml       # Lambert's Cosine Law twin (Single-Axis tracking)
    │   │   └── StationConfigurator.cshtml          # Advanced full station yield comparison topology
    │   ├── Shared/
    │   │   ├── _Layout.cshtml                      # Global HTML structure, Navbar, and Footer
    │   │   ├── _ValidationScriptsPartial.cshtml    # Standard ASP.NET Core client-side validation
    │   │   └── Error.cshtml                        # Global error handling UI
    │   ├── _ViewImports.cshtml                     # Global namespace imports for Razor views
    │   └── _ViewStart.cshtml                       # Sets default layout for all views
    │
    └── wwwroot/                                    # Static Web Assets
        ├── css/
        │   └── site.css                            # Global CSS, Glassmorphism, and Neon UI color variables
        ├── js/
        │   ├── charts-init.js                      # Initialization logic for Chart.js dashboards
        │   ├── simulators-waapi.js                 # Web Animations API logic for digital twin modules
        │   └── site.js                             # Common interactivity and event listeners
        ├── lib/
        │   ├── bootstrap/                          # Bootstrap 5 framework files
        │   ├── chart.js/                           # Chart.js library for 10-year and seasonality graphs
        │   ├── fontawesome/                        # Icons used across the dashboards and simulators
        │   └── jquery/                             # jQuery library
        └── favicon.ico                             # Application browser tab icon
```
## 🛠 Tech Stack

* 🖥️ **Backend:** C#, ASP.NET Core MVC, Entity Framework Core
* 🗄️ **Database:** Microsoft SQL Server (`Microsoft.Data.SqlClient`)
* 🤖 **AI Integration:** Google Gemini 2.5 Flash
* 📡 **Data Source:** Open-Meteo Historical Weather API
* 🧠 **Data Processing & Seeding:** CsvHelper, LINQ, `System.Text.Json` (deep cloning)
* 🎨 **Frontend:** HTML5, CSS3 (Glassmorphism & Neon UI), JavaScript (Vanilla & WAAPI), Bootstrap 5, FontAwesome, Chart.js
* 📤 **Data Export:** CSV & Excel generation pipelines

---

## ⚙️ Prerequisites

* ✅ .NET 8.0 SDK
* ✅ SQL Server (LocalDB, Express, or Developer)
* ✅ Historical Weather CSV Data *(Required for the DataSeeder to build the TMY baseline)*

---

## 🚀 Installation & Setup

**1. Clone the repository** ⬇️
```bash
git clone [[(https://github.com/YounisSaid/CairoWeather.MVC)](https://github.com/YounisSaid/CairoWeather.MVC)]
cd CairoWeather
