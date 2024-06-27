const productBaseUrl = "http://localhost:5004/api/ProductSales";
const pieChartContainer = document.querySelector("#pieChartContainer");

const pieCharts = {
  typeChart: null,
  categoryChart: null,
};

function analyseProduct() {
  getData();
}

function getData() {
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  const url = `${productBaseUrl}?StartTime=${dateFrom}&EndTime=${dateTo}`;
  fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .then((data) => {
      console.log(data);
      updatePieCharts(data);
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function updatePieCharts(data) {
  const productTypes = data.productSalesBySize;
  const productCategories = data.productSalesByCategory;

  if (pieCharts.typeChart) {
    pieCharts.typeChart.destroy();
  }
  if (pieCharts.categoryChart) {
    pieCharts.categoryChart.destroy();
  }

  const typeCtx = document.getElementById('typeChart').getContext('2d');
  pieCharts.typeChart = createPieChart(typeCtx, productTypes, 'Product Types');

  const categoryCtx = document.getElementById('categoryChart').getContext('2d');
  pieCharts.categoryChart = createPieChart(categoryCtx, productCategories, 'Product Categories');
}

function createPieChart(ctx, data, title) {
  const total = Object.values(data).reduce((sum, value) => sum + value, 0);

  return new Chart(ctx, {
    type: 'pie',
    data: {
      labels: Object.keys(data).map(key => `${key} (${data[key]} - ${(data[key] / total * 100).toFixed(2)}%)`),
      datasets: [{
        data: Object.values(data),
        backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0'],
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
