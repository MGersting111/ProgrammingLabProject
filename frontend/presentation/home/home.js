document.addEventListener('DOMContentLoaded', function() {
    // Function to initialize Chart.js chart
    function initializeChart() {
        const canvas = document.getElementById('myChart');
        const ctx = canvas.getContext('2d');

        const data = {
            datasets: [{
                data: [59, 41], // Example data to represent 59% progress
                backgroundColor: ['#FFA726', '#E0E0E0'], // Colors for the chart
                borderWidth: 0 // No border
            }]
        };

        const options = {
            rotation: -0.5 * Math.PI, // Start angle for the circular chart
            circumference: 2 * Math.PI, // Full circle
            cutout: '80%', // Creates the thickness of the ring
            responsive: true, // Make the chart responsive
            maintainAspectRatio: false, // Don't maintain aspect ratio
            plugins: {
                tooltip: {
                    enabled: false // Disable tooltips
                },
                legend: {
                    display: false // Hide the legend
                }
            }
        };

        const myChart = new Chart(ctx, {
            type: 'doughnut', // Specify chart type
            data: data,
            options: options
        });

        // Adding the percentage text in the center
        Chart.pluginService.register({
            beforeDraw: function(chart) {
                const width = chart.chart.width,
                    height = chart.chart.height,
                    ctx = chart.chart.ctx;

                ctx.restore();
                const fontSize = (height / 114).toFixed(2);
                ctx.font = fontSize + "em Arial";
                ctx.textBaseline = "middle";

                const text = "59%",
                    textX = Math.round((width - ctx.measureText(text).width) / 2),
                    textY = height / 2;

                ctx.fillText(text, textX, textY);
                ctx.save();
            }
        });
    }

    // Initialize the chart
    initializeChart();
});
