(() => {
    const chartContainer = document.getElementById("salesChart");
    if (!chartContainer || typeof Chart === "undefined" || !window.invexaChartData) {
        return;
    }

    const { labels, values } = window.invexaChartData;

    // eslint-disable-next-line no-new
    new Chart(chartContainer, {
        type: "bar",
        data: {
            labels,
            datasets: [{
                label: "Revenue",
                data: values,
                borderWidth: 1,
                backgroundColor: ["#0ea5a4", "#14b8a6", "#0f766e"]
            }]
        },
        options: {
            scales: {
                y: { beginAtZero: true }
            },
            plugins: {
                legend: { display: false }
            }
        }
    });
})();