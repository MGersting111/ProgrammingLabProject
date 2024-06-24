// productanalyse.js
document.addEventListener('DOMContentLoaded', (event) => {
    createPieCharts();
});

const productData = {
    types: ['Large', 'Medium', 'Small'],
    typeValues: [30, 25, 45],
    categories: ['Special', 'Veggie', 'Classic'],
    categoryValues: [40, 20, 60]
};

const lineChartData = {
    labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May'],
    totalRevenue: [20, 30, 25, 35, 40],
    avgRevenue: [18, 28, 23, 33, 38],
    totalSales: [22, 32, 27, 37, 42],
    avgSales: [20, 30, 25, 35, 40]
};

function createPieCharts() {
    const ctxType = document.getElementById('typeChart').getContext('2d');
    const ctxCategory = document.getElementById('categoryChart').getContext('2d');

    new Chart(ctxType, {
        type: 'pie',
        data: {
            labels: productData.types,
            datasets: [{
                data: productData.typeValues,
                backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56']
            }]
        },
        options: {
            onClick: (e, item) => {
                if (item.length > 0) {
                    updateLineCharts(item[0].index);
                }
            }
        }
    });

    new Chart(ctxCategory, {
        type: 'pie',
        data: {
            labels: productData.categories,
            datasets: [{
                data: productData.categoryValues,
                backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56']
            }]
        },
        options: {
            onClick: (e, item) => {
                if (item.length > 0) {
                    updateLineCharts(item[0].index);
                }
            }
        }
    });
}

function updateLineCharts(index) {
    const ctxTotalRevenue = document.getElementById('totalRevenueChart').getContext('2d');
    const ctxAvgRevenue = document.getElementById('avgRevenueChart').getContext('2d');
    const ctxTotalSales = document.getElementById('totalSalesChart').getContext('2d');
    const ctxAvgSales = document.getElementById('avgSalesChart').getContext('2d');

    new Chart(ctxTotalRevenue, {
        type: 'line',
        data: {
            labels: lineChartData.labels,
            datasets: [{
                label: 'Total Revenue',
                data: lineChartData.totalRevenue,
                borderColor: '#FF6384'
            }]
        }
    });

    new Chart(ctxAvgRevenue, {
        type: 'line',
        data: {
            labels: lineChartData.labels,
            datasets: [{
                label: 'Avg Revenue',
                data: lineChartData.avgRevenue,
                borderColor: '#36A2EB'
            }]
        }
    });

    new Chart(ctxTotalSales, {
        type: 'line',
        data: {
            labels: lineChartData.labels,
            datasets: [{
                label: 'Total Sales',
                data: lineChartData.totalSales,
                borderColor: '#FFCE56'
            }]
        }
    });

    new Chart(ctxAvgSales, {
        type: 'line',
        data: {
            labels: lineChartData.labels,
            datasets: [{
                label: 'Avg Sales',
                data: lineChartData.avgSales,
                borderColor: '#FF6384'
            }]
        }
    });
}
