const barCtx = document.getElementById('barChart').getContext('2d');
const barChart = new Chart(barCtx, {
    type: 'bar',
    data: {
        labels: ['Store1', 'Store2', 'Store3', 'Store4', 'Store5', 'Store6'],
        datasets: [{
            label: '# revenue',
            data: [82, 100, 30, 50, 60, 10],
            backgroundColor: [
                'rgba(255, 99, 132, 0.2)',
                'rgba(54, 162, 235, 0.2)',
                'rgba(255, 206, 86, 0.2)',
                'rgba(75, 192, 192, 0.2)',
                'rgba(153, 102, 255, 0.2)',
                'rgba(255, 159, 64, 0.2)'
            ],
            borderColor: [
                'rgba(255, 99, 132, 1)',
                'rgba(54, 162, 235, 1)',
                'rgba(255, 206, 86, 1)',
                'rgba(75, 192, 192, 1)',
                'rgba(153, 102, 255, 1)',
                'rgba(255, 159, 64, 1)'
            ],
            borderWidth: 1
        }]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});

// Function to fetch data based on filters
function getData() {
    // Get selected values from the dropdowns
    const category = document.getElementById('categorySelect').value;
    const order = document.getElementById('orderSelect').value;

    // Show or hide the bar chart based on the selected category
    const barChartContainer = document.getElementById('barChartContainer');
    if (category === 'revenue') {
        barChartContainer.style.display = 'block';
    } else {
        barChartContainer.style.display = 'none';
    }

    // Log selected values (for debugging)
    console.log('Category:', category, 'Order:', order);

    // Perform fetch or any other operations to get the data
    // Update the chart accordingly
}

document.addEventListener('DOMContentLoaded', function() {
    $('#categorySelect').select2();
    $('#orderSelect').select2();
    $('#sortSelect').select2();
});


const ctx = document.getElementById('lineChart').getContext('2d');
const lineChart = new Chart(ctx, {
    type: 'line',
    data: {
        labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
        datasets: [{
            label: 'Monthly Data',
            data: [10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120],
            backgroundColor: 'rgba(54, 162, 235, 0.2)',
            borderColor: 'rgba(54, 162, 235, 1)',
            borderWidth: 1,
            fill: false
        }]
    },
    options: {
        scales: {
            y: {
                beginAtZero: true,
                ticks: {
                    stepSize: 10
                }
            }
        }
    }
});

// Scatter chart initialization
const ctb = document.getElementById('scatterChart').getContext('2d');
const scatterChart = new Chart(ctb, {
    type: 'scatter',
    data: {
        datasets: [
            {
                label: 'Stores',
                data: [
                    { x: 'January', y: 50 },
                    { x: 'February', y: 100 },
                    { x: 'March', y: 150 },
                    { x: 'April', y: 200 },
                    { x: 'May', y: 250 },
                    { x: 'June', y: 300 },
                    { x: 'July', y: 350 },
                    { x: 'August', y: 400 },
                    { x: 'September', y: 450 },
                    { x: 'October', y: 500 },
                    { x: 'November', y: 550 },
                    { x: 'December', y: 600 },
                    // Additional fake scatter data
                    { x: 'January', y: 70 },
                    { x: 'February', y: 130 },
                    { x: 'March', y: 170 },
                    { x: 'April', y: 220 },
                    { x: 'May', y: 270 },
                    { x: 'June', y: 320 },
                    { x: 'July', y: 370 },
                    { x: 'August', y: 420 },
                    { x: 'September', y: 470 },
                    { x: 'October', y: 520 },
                    { x: 'November', y: 570 },
                    { x: 'December', y: 620 }
                ],
                backgroundColor: 'rgba(75, 192, 192, 0.6)',
                borderColor: 'rgba(75, 192, 192, 1)',
                pointRadius: 5
            },
            {
                label: 'Category',
                data: [
                    { x: 'January', y: 30 },
                    { x: 'February', y: 80 },
                    { x: 'March', y: 130 },
                    { x: 'April', y: 180 },
                    { x: 'May', y: 230 },
                    { x: 'June', y: 280 },
                    { x: 'July', y: 330 },
                    { x: 'August', y: 380 },
                    { x: 'September', y: 430 },
                    { x: 'October', y: 480 },
                    { x: 'November', y: 530 },
                    { x: 'December', y: 580 },
                    ],
                backgroundColor: 'rgb(255, 165, 0)',
                borderColor: 'rgba(153, 102, 255, 1)',
                pointRadius: 10
            }
        ]
    },
    options: {
        scales: {
            x: {
                type: 'category',
                labels: ['January', 'February', 'March', 'April', 'May', 'June', 'July', 'August', 'September', 'October', 'November', 'December'],
            },
            y: {
                beginAtZero: true,
                ticks: {
                    stepSize: 50
                }
            }
        }
    }
});

// Fetch data based on filters
function getData() {
    // Get selected values from the dropdowns
    const category = document.getElementById('modelSelect').value;
    const order = document.getElementById('orderSelect').value;

    // Perform fetch or any other operations to get the data
    // Update the table body or chart accordingly
    console.log('Category:', category, 'Order:', order);
}

document.addEventListener('DOMContentLoaded', function() {
    $('#modelSelect').select2();
    $('#orderSelect').select2();
    $('#sortSelect').select2();
});
// script.js

// Function to fetch the GeoJSON data and initialize the chart
async function initMapChart() {
    // Fetch the GeoJSON data for the United States
    const response = await fetch('https://raw.githubusercontent.com/PublicaMundi/MappingAPI/master/data/geojson/us-states.json');
    const usMapData = await response.json();

    // Initialize the map chart
    const ctx = document.getElementById('mapChart').getContext('2d');
    new Chart(ctx, {
        type: 'choropleth',
        data: {
            labels: usMapData.features.map(feature => feature.properties.name),
            datasets: [{
                label: 'Sales',
                data: usMapData.features.map(feature => ({
                    feature: feature,
                    value: Math.floor(Math.random() * 500) + 50 // Random sales data for illustration
                }))
            }]
        },
        options: {
            showOutline: true,
            showGraticule: true,
            legend: {
                display: true,
                position: 'bottom'
            },
            scale: {
                projection: 'albersUsa'
            },
            geo: {
                colorScale: {
                    display: true,
                    legend: {
                        display: true,
                        position: 'bottom-right'
                    }
                }
            }
        }
    });
}

// Call the function to initialize the map chart
initMapChart();

