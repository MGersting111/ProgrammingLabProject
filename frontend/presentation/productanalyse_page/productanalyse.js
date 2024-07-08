const productBaseUrl = "http://localhost:5004/api/ProductSales";
const pieChartContainer = document.querySelector("#pieChartContainer");
const lineChartContainer = document.querySelector(".lineChartsContainer");
const revenueLineChartContainer = document.querySelector(".revenueLineChartsContainer");

let typeChart;
let categoryChart;
let totalSalesChart;
let cumulativeSalesChart;
let totalRevenueChart;
let cumulativeRevenueChart;
let donutChart;

function analyseProduct() {
  getData();
}

function getData() {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);

  const url = `${productBaseUrl}?fromDate=${dateFrom.toISOString()}&toDate=${dateTo.toISOString()}`;
  console.log(url);

  fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  })
  .then(response => {
    if (!response.ok) {
      throw new Error(`Network response was not ok: ${response.status} ${response.statusText}`);
    }
    return response.json();
  })
  .then(data => {
    console.log("Fetched data based on input:", data);

    if (!data.totalSize || Object.keys(data.totalSize).length === 0) {
      console.error("totalSize data is missing or empty");
      return;
    }

    if (!data.totalCategory || Object.keys(data.totalCategory).length === 0) {
      console.error("totalCategory data is missing or empty");
      return;
    }

    const productSize = {
      Small: data.totalSize.Small || 0,
      Large: data.totalSize.Large || 0,
      Medium: data.totalSize.Medium || 0,
      "Extra Large": data.totalSize["Extra Large"] || 0
    };

    const productCategory = {
      Classic: data.totalCategory.Classic || 0,
      Vegetarian: data.totalCategory.Vegetarian || 0,
      Specialty: data.totalCategory.Specialty || 0
    };

    updatePieCharts(productSize, productCategory);

    const label = 'Product Data';  // Provide a default label for charts

    if (data.productSalesBySize) {
      updateLineCharts(data.productSalesBySize, label, 'Sales by Size', [
        'rgba(173, 216, 230, 0.8)', // Light Blue for Small
        'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
        'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
        'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
      ], 'totalSalesChart', 'cumulativeSalesChart', lineChartContainer);
    }

    if (data.productSalesByCategory) {
      updateLineCharts(data.productSalesByCategory, label, 'Sales by Category', [
        'rgba(0, 123, 255, 0.8)',   // Blue for Classic
        'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
        'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
      ], 'totalSalesChart', 'cumulativeSalesChart', lineChartContainer);
    }

    if (data.productRevenueBySize) {
      updateLineCharts(data.productRevenueBySize, label, 'Revenue by Size', [
        'rgba(173, 216, 230, 0.8)', // Light Blue for Small
        'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
        'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
        'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
      ], 'totalRevenueChart', 'cumulativeRevenueChart', revenueLineChartContainer);
    }

    if (data.productRevenueByCategory) {
      updateLineCharts(data.productRevenueByCategory, label, 'Revenue by Category', [
        'rgba(0, 123, 255, 0.8)',   // Blue for Classic
        'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
        'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
      ], 'totalRevenueChart', 'cumulativeRevenueChart', revenueLineChartContainer);
    }
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updatePieCharts(productSize, productCategory) {
  if (typeChart) {
    typeChart.destroy();
  }

  if (categoryChart) {
    categoryChart.destroy();
  }

  const typeCtx = document.getElementById('typeChart').getContext('2d');
  typeChart = createPieChart(typeCtx, productSize, 'Product Sizes', [
    'rgba(173, 216, 230, 0.8)', // Light Blue for Small
    'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
    'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
    'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
  ]);

  const categoryCtx = document.getElementById('categoryChart').getContext('2d');
  categoryChart = createPieChart(categoryCtx, productCategory, 'Product Categories', [
    'rgba(0, 123, 255, 0.8)',   // Blue for Classic
    'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
    'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
  ]);
}

function createPieChart(ctx, data, title, colors) {
  const total = Object.values(data).reduce((sum, value) => sum + value, 0);
  const defaultColors = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'];

  return new Chart(ctx, {
    type: 'pie',
    data: {
      labels: Object.keys(data).map(key => `${key} (${data[key]} - ${(data[key] / total * 100).toFixed(2)}%)`),
      datasets: [{
        data: Object.values(data),
        backgroundColor: colors || defaultColors,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: title
        }
      },
      onClick: (e, elements) => {
        if (elements.length > 0) {
          const clickedElementIndex = elements[0].index;
          const label = Object.keys(data)[clickedElementIndex];
          fetchLineChartData(label, title === 'Product Sizes' ? 'size' : 'category');
        }
      }
    }
  });
}

function fetchLineChartData(label, chartType) {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);

  const url = `${productBaseUrl}?fromDate=${dateFrom.toISOString()}&toDate=${dateTo.toISOString()}`;
  console.log(`Fetching line chart data from URL: ${url}`);

  fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json"
    }
  })
  .then(response => {
    if (!response.ok) {
      throw new Error(`Network response was not ok: ${response.status} ${response.statusText}`);
    }
    return response.json();
  })
  .then(data => {
    console.log("Fetched line chart data:", data);
    if (chartType === 'size') {
      updateLineCharts(data.productSalesBySize, label, 'Size', [
        'rgba(173, 216, 230, 0.8)', // Light Blue for Small
        'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
        'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
        'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
      ], 'totalSalesChart', 'cumulativeSalesChart', lineChartContainer);
      updateLineCharts(data.productRevenueBySize, label, 'Revenue by Size', [
        'rgba(173, 216, 230, 0.8)', // Light Blue for Small
        'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
        'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
        'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
      ], 'totalRevenueChart', 'cumulativeRevenueChart', revenueLineChartContainer);
    } else if (chartType === 'category') {
      updateLineCharts(data.productSalesByCategory, label, 'Category', [
        'rgba(0, 123, 255, 0.8)',   // Blue for Classic
        'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
        'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
      ], 'totalSalesChart', 'cumulativeSalesChart', lineChartContainer);
      updateLineCharts(data.productRevenueByCategory, label, 'Revenue by Category', [
        'rgba(0, 123, 255, 0.8)',   // Blue for Classic
        'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
        'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
      ], 'totalRevenueChart', 'cumulativeRevenueChart', revenueLineChartContainer);
    }
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updateLineCharts(data, label, chartType, colors, chartId, cumulativeChartId, container) {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);
  const year = dateFrom.getFullYear();
  const months = [];

  // Generate month labels based on the selected date range
  let currentDate = new Date(dateFrom);
  while (currentDate <= dateTo) {
    months.push(currentDate.toLocaleString('default', { month: 'short' }));
    currentDate.setMonth(currentDate.getMonth() + 1);
  }

  const datasets = Object.keys(data).map((key, index) => {
    const metrics = data[key][year];
    const values = months.map((month, idx) => {
      const monthNumber = new Date(dateFrom.getFullYear(), idx).toLocaleString('default', { month: 'long' }).substring(0, 3);
      return metrics[monthNumber] || 0;
    });
    return {
      label: key,
      data: values,
      borderColor: colors[index % colors.length],
      borderWidth: 2,
      fill: false,
      pointBackgroundColor: colors[index % colors.length]
    };
  });

  const cumulativeDatasets = datasets.map(dataset => {
    const cumulativeValues = dataset.data.reduce((acc, val) => {
      if (acc.length > 0) {
        acc.push(acc[acc.length - 1] + val);
      } else {
        acc.push(val);
      }
      return acc;
    }, []);
    return { ...dataset, data: cumulativeValues };
  });

  const chartData = {
    labels: months,
    datasets: datasets
  };

  const cumulativeChartData = {
    labels: months,
    datasets: cumulativeDatasets
  };

  const ctx = document.getElementById(chartId).getContext('2d');
  const cumulativeCtx = document.getElementById(cumulativeChartId).getContext('2d');

  if (window[chartId]) window[chartId].destroy();
  if (window[cumulativeChartId]) window[cumulativeChartId].destroy();

  window[chartId] = new Chart(ctx, {
    type: 'line',
    data: chartData,
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        title: {
          display: true,
          text: `Monthly ${chartType} for ${label}`
        }
      },
      scales: {
        x: {
          title: {
            display: true,
            text: 'Month'
          }
        },
        y: {
          title: {
            display: true,
            text: chartType
          },
          beginAtZero: true
        }
      },
      onClick: (e, elements) => {
        if (elements.length > 0) {
          const clickedElementIndex = elements[0].index;
          const month = months[clickedElementIndex];
          showDonutChart(month, data, chartType);
        }
      }
    }
  });

  window[cumulativeChartId] = new Chart(cumulativeCtx, {
    type: 'line',
    data: cumulativeChartData,
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        title: {
          display: true,
          text: `Cumulative ${chartType} for ${label}`
        }
      },
      scales: {
        x: {
          title: {
            display: true,
            text: 'Month'
          }
        },
        y: {
          title: {
            display: true,
            text: `Cumulative ${chartType}`
          },
          beginAtZero: true
        }
      },
      onClick: (e, elements) => {
        if (elements.length > 0) {
          const clickedElementIndex = elements[0].index;
          const month = months[clickedElementIndex];
          showDonutChart(month, data, chartType);
        }
      }
    }
  });
}

function showDonutChart(month, data, chartType) {
  const year = new Date(document.getElementById("fromDate").value).getFullYear();
  const monthData = Object.keys(data).reduce((acc, key) => {
    const metrics = data[key][year];
    acc[key] = metrics[month] || 0;
    return acc;
  }, {});

  const ctx = document.getElementById('donutChart').getContext('2d');
  if (donutChart) {
    donutChart.destroy();
  }

  donutChart = new Chart(ctx, {
    type: 'doughnut',
    data: {
      labels: Object.keys(monthData),
      datasets: [{
        data: Object.values(monthData),
        backgroundColor: [
          'rgba(255, 99, 132, 0.6)',
          'rgba(54, 162, 235, 0.6)',
          'rgba(255, 206, 86, 0.6)',
          'rgba(75, 192, 192, 0.6)',
          'rgba(153, 102, 255, 0.6)',
          'rgba(255, 159, 64, 0.6)'
        ]
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: `Breakdown for ${month} (${chartType})`
        }
      }
    }
  });
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('analyseButton').addEventListener('click', analyseProduct);
});
