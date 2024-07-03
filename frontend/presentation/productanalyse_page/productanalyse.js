const productBaseUrl = "http://localhost:5004/api/ProductSales";
const lineBaseUrl = "http://localhost:5004/api/CompareCharts/ChartsInfos";
const pieChartContainer = document.querySelector("#pieChartContainer");
const lineChartContainer = document.querySelector(".lineChartsContainer");

let typeChart;
let categoryChart;
let totalRevenueChart;
let avgRevenueChart;
let totalSalesChart;
let avgSalesChart;

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

    if (!data.productSalesBySize || Object.keys(data.productSalesBySize).length === 0) {
      console.error("productSalesBySize data is missing or empty");
      return;
    }

    if (!data.productSalesByCategory || Object.keys(data.productSalesByCategory).length === 0) {
      console.error("productSalesByCategory data is missing or empty");
      return;
    }

    const sizeData = data.productSalesBySize[dateFrom.getFullYear()];
    console.log(`Size data:`, sizeData);

    const productSize = {
      Small: sizeData.Small || 0,
      Large: sizeData.Large || 0,
      Medium: sizeData.Medium || 0,
      "Extra Large": sizeData["Extra Large"] || 0
    };

    const categoryData = data.productSalesByCategory[dateFrom.getFullYear()];
    console.log(`Category data:`, categoryData);

    const productCategory = {
      Classic: categoryData.Classic || 0,
      Vegetarian: categoryData.Vegetarian || 0,
      Specialty: categoryData.Specialty || 0
    };

    updatePieCharts(productSize, productCategory);
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
  typeChart = createPieChart(typeCtx, productSize, 'Product Sizes');

  const categoryCtx = document.getElementById('categoryChart').getContext('2d');
  categoryChart = createPieChart(categoryCtx, productCategory, 'Product Categories', 'category');
}

function createPieChart(ctx, data, title, chartType) {
  const total = Object.values(data).reduce((sum, value) => sum + value, 0);

  return new Chart(ctx, {
    type: 'pie',
    data: {
      labels: Object.keys(data).map(key => `${key} (${data[key]} - ${(data[key] / total * 100).toFixed(2)}%)`),
      datasets: [{
        data: Object.values(data),
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'],
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
          fetchLineChartData(label, chartType);
        }
      }
    }
  });
}

function fetchLineChartData(label, chartType) {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);

  const url = `${lineBaseUrl}?storeId=${label}&comparisonType=${chartType === 'category' ? 'Category' : 'Product'}&startDate=${dateFrom.toISOString()}&endDate=${dateTo.toISOString()}`;
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
    updateLineCharts(data, label);
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updateLineCharts(data, label) {
  const year = new Date(document.getElementById("fromDate").value).getFullYear();
  const metrics = data.find(item => item.storeId === label).metricsByYear[year].metrics;
  const months = Object.keys(metrics);
  const values = Object.values(metrics);

  const totalRevenueData = {
    labels: months,
    datasets: [{
      label: `Total Revenue (${label})`,
      data: values,
      borderColor: 'rgba(75, 192, 192, 1)',
      borderWidth: 1,
      fill: false
    }]
  };

  const ctx1 = document.getElementById('totalRevenueChart').getContext('2d');

  if (totalRevenueChart) totalRevenueChart.destroy();

  totalRevenueChart = new Chart(ctx1, {
    type: 'line',
    data: totalRevenueData,
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: `Total Revenue for ${label}`
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
            text: 'Revenue'
          },
          beginAtZero: true
        }
      }
    }
  });

  // Repeat similar setup for avgRevenueChart, totalSalesChart, and avgSalesChart if needed
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('analyseButton').addEventListener('click', analyseProduct);
});
