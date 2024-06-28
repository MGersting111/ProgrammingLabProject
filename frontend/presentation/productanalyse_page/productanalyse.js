const productBaseUrl = "http://localhost:5004/api/ProductSales";
const pieChartContainer = document.querySelector("#pieChartContainer");

let typeChart;

function analyseProduct() {
  getData();
}

function getData() {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);
  const year = dateFrom.getFullYear();
  const url = `${productBaseUrl}?StartTime=${dateFrom.toISOString()}&EndTime=${dateTo.toISOString()}`;
  
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

    // Log the entire data object to inspect its structure
    console.log("Complete data object:", data);

    if (!data.productSalesBySize) {
      throw new Error("productSalesBySize data is missing");
    }

    if (!data.productSalesBySize[year]) {
      throw new Error(`productSalesBySize[${year}] data is missing`);
    }

    const sizeData = data.productSalesBySize[year];
    console.log(`Size data for ${year}:`, sizeData);

    const productSize = {
      Small: sizeData.Small || 0,
      Large: sizeData.Large || 0,
      Medium: sizeData.Medium || 0,
      "Extra Large": sizeData["Extra Large"] || 0
    };

    updatePieCharts(productSize);
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updatePieCharts(productSize) {
  if (typeChart) {
    typeChart.destroy();
  }

  const typeCtx = document.getElementById('typeChart').getContext('2d');
  typeChart = createPieChart(typeCtx, productSize, 'Product Sizes');
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
