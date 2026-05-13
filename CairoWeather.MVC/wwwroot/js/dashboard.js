// --- CHART.JS CONFIGURATION ---
Chart.defaults.font.family = "'Segoe UI', Roboto, sans-serif";
Chart.defaults.color = '#64748b';
Chart.defaults.plugins.legend.position = 'bottom';

const verticalLinePlugin = {
    id: 'verticalLine',
    afterDatasetsDraw: function (chart, args, options) {
        if (options && options.draw && options.hour !== undefined) {
            const ctx = chart.ctx;
            const topY = chart.chartArea.top;
            const bottomY = chart.chartArea.bottom;

            // THE FIX FOR MOBILE: Grab X coordinate from the actual data point instead of the axis tick
            const meta = chart.getDatasetMeta(0);
            if (!meta || !meta.data[options.hour]) return;
            const x = meta.data[options.hour].x;

            ctx.save();
            ctx.beginPath();
            ctx.moveTo(x, topY + 22);
            ctx.lineTo(x, bottomY);
            ctx.lineWidth = 2;
            ctx.strokeStyle = '#ef4444';
            ctx.setLineDash([5, 5]);
            ctx.stroke();

            ctx.fillStyle = '#ef4444';
            const text = options.hour + 'h';
            ctx.font = 'bold 12px "Segoe UI"';
            const textWidth = ctx.measureText(text).width;
            const boxWidth = textWidth + 16;
            const boxHeight = 22;

            let boxX = x - boxWidth / 2;
            boxX = Math.max(chart.chartArea.left, Math.min(boxX, chart.chartArea.right - boxWidth));

            ctx.fillRect(boxX, topY, boxWidth, boxHeight);
            ctx.fillStyle = 'white';
            ctx.textAlign = 'center';
            ctx.textBaseline = 'middle';
            ctx.fillText(text, boxX + boxWidth / 2, topY + boxHeight / 2);
            ctx.restore();
        }
    }
};
Chart.register(verticalLinePlugin);

let charts = {};
function initChart(id, type, data, options) {
    if (charts[id]) charts[id].destroy();
    const ctx = document.getElementById(id);
    if (!ctx) return;
    const defaultColors = ['#f59e0b', '#ef4444', '#3b82f6', '#10b981', '#8b5cf6', '#64748b'];
    if (data && data.datasets) {
        data.datasets.forEach((ds, i) => {
            if (!ds.borderColor && !ds.backgroundColor) {
                ds.borderColor = defaultColors[i % defaultColors.length];
                ds.backgroundColor = defaultColors[i % defaultColors.length];
            }
        });
    }
    charts[id] = new Chart(ctx, { type, data, options: { responsive: true, maintainAspectRatio: false, ...options } });
}
const createDatasets = (dict) => {
    if (!dict) return []; return Object.keys(dict).map(key => ({ label: key, data: dict[key] }));
};

// --- REAL AI REPORT LOGIC ---
async function triggerContextualAIReport() {
    const myOffcanvas = new bootstrap.Offcanvas(document.getElementById('aiReportPanel'));
    myOffcanvas.show();

    const loader = document.getElementById('aiLoading');
    const content = document.getElementById('aiContent');
    const textBody = document.getElementById('aiTextBody');
    const activeTab = document.getElementById('currentTabContext').value;
    let paramValue = "";

    // التعديل هنا: ضفنا Tmy عشان يقرأ من الـ datePicker زي الـ Hourly بالظبط
    if (activeTab === "Hourly" || activeTab === "Tmy") {
        paramValue = document.getElementById('datePicker') ? document.getElementById('datePicker').value : "";
    }
    else if (activeTab === "Monthly") { paramValue = document.getElementById('monthPicker') ? document.getElementById('monthPicker').value : ""; }
    else if (activeTab === "Yearly") { paramValue = document.getElementById('yearPicker') ? document.getElementById('yearPicker').value : ""; }
    else if (activeTab === "Decadal") { paramValue = "2015-2025"; }

    loader.classList.remove('d-none');
    content.classList.add('d-none');
    textBody.innerHTML = "";

    try {
        const response = await fetch(`/api/AiInsights/generate?timeframe=${activeTab}&param=${paramValue}`);
        if (!response.ok) throw new Error("Failed to generate AI report");
        const data = await response.json();

        loader.classList.add('d-none');
        content.classList.remove('d-none');
        typeWriter(data.reportHtml, "aiTextBody");
    } catch (error) {
        console.error("AI Generation Error:", error);
        loader.classList.add('d-none');
        content.classList.remove('d-none');
        textBody.innerHTML = `<div class='alert alert-danger'><i class='fa-solid fa-triangle-exclamation me-2'></i><strong>Connection Error:</strong> ${error.message}</div>`;
    }
}

function typeWriter(html, elementId) {
    const element = document.getElementById(elementId);
    let i = 0;
    element.innerHTML = "";
    const interval = setInterval(() => {
        element.innerHTML = html.substring(0, i) + "<span class='ai-typing-cursor'></span>";
        i += 5;
        if (i >= html.length) {
            clearInterval(interval);
            element.innerHTML = html;
        }
        const panel = document.querySelector('.ai-offcanvas .offcanvas-body');
        panel.scrollTop = panel.scrollHeight;
    }, 20);
}

// --- RAW DATA EXPORT LOGIC (CSV/EXCEL) ---
function exportRealData(format) {
    const activeTab = document.getElementById('currentTabContext').value;
    let paramValue = "";

    // التعديل هنا كمان للـ Export عشان الداتا تنزل صح
    if (activeTab === "Hourly" || activeTab === "Tmy") {
        paramValue = document.getElementById('datePicker') ? document.getElementById('datePicker').value : "";
    }
    else if (activeTab === "Monthly") { paramValue = document.getElementById('monthPicker') ? document.getElementById('monthPicker').value : ""; }
    else if (activeTab === "Yearly") { paramValue = document.getElementById('yearPicker') ? document.getElementById('yearPicker').value : ""; }
    else if (activeTab === "Decadal") { paramValue = "2015-2025"; }

    // Building the URL for DataExportController
    const url = `/api/DataExport/download?format=${format}&timeframe=${activeTab}&param=${paramValue}`;
    window.location.href = url;
}

// --- EXPORT TO PDF LOGIC (AI Report) ---
function exportAIToPDF() {
    const element = document.getElementById('pdfExportArea');
    const activeTab = document.getElementById('currentTabContext').value;
    let filename = `Cairo_Climate_Report_${activeTab}.pdf`;
    var opt = {
        margin: [0.5, 0.5, 0.5, 0.5],
        filename: filename,
        image: { type: 'jpeg', quality: 0.98 },
        html2canvas: { scale: 2 },
        jsPDF: { unit: 'in', format: 'letter', orientation: 'portrait' }
    };
    html2pdf().set(opt).from(element).save();
}