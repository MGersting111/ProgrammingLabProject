const productBaseUrl = "http://localhost:5004/api/ProductSales";
const pieChartContainer = document.querySelector("#pieChartContainer");

let typeChart;
let categoryChart;

function analyseProduct() {
  getData();
}

function getData() {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);

  // Extract the year from the selected dates
  const yearFrom = dateFrom.getFullYear();
  const yearTo = dateTo.getFullYear();

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
    console.log("Complete data object:", data);

    if (!data.productSalesBySize || Object.keys(data.productSalesBySize).length === 0) {
      console.error("productSalesBySize data is missing or empty");
      return;
    }

    if (!data.productSalesByCategory || Object.keys(data.productSalesByCategory).length === 0) {
      console.error("productSalesByCategory data is missing or empty");
      return;
    }

    // Dynamically use the year
    const sizeData = data.productSalesBySize[yearFrom];
    console.log(`Size data:`, sizeData);

    const productSize = {
      Small: sizeData.Small || 0,
      Large: sizeData.Large || 0,
      Medium: sizeData.Medium || 0,
      "Extra Large": sizeData["Extra Large"] || 0
    };

    const categoryData = data.productSalesByCategory[yearFrom];
    console.log(`Category data:`, categoryData);

    const productCategory = {
      Classic: categoryData.Classic || 0,
      Vegetarian: categoryData.Vegetarian || 0,
      Specialty: categoryData.Specialty || 0
      // Add more categories as needed
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
  categoryChart = createPieChart(categoryCtx, productCategory, 'Product Categories');
}

function createPieChart(ctx, data, title) {
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
      }
    }
  });
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('analyseButton').addEventListener('click', analyseProduct);
});
