const storeBaseUrl = "http://localhost:5004/api/StoreMetrics";
var barChart;
var lineChart;
var donutChart;

async function analyseStore() {
  var data = await getData();
  if (data) {
    dataSet = orderData(data);
    var firstChartContainer = document.getElementById("firstChartContainer");
    firstChartContainer.style.display = "block";
    createDonutChart(data, "Test Chart", testOnclick, "testLineChart");
  }
}

function getData() {
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  const url = `${storeBaseUrl}?fromDate=${dateFrom}&toDate=${dateTo}`;
  return fetch(url, {
    method: "GET",
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function orderData(data) {
  console.log(data);
}

function createMapChart() {}

function createLineChart(dataMap, chartName, onClick, canvaId) {
  const ctx = document.getElementById(canvaId).getContext("2d");
  const labels = dataMap.map((obj) => Object.keys(obj)[0]);
  const dataValues = dataMap.map((obj) => Object.values(obj)[0]);
  if (lineChart != null) {
    lineChart.destroy();
  }
  // Erstellen des Charts
  lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: labels,
      datasets: [
        {
          label: chartName,
          data: dataValues,
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          borderColor: "rgba(75, 192, 192, 1)",
          borderWidth: 1,
        },
      ],
    },
    options: {
      scales: {
        y: {
          beginAtZero: true,
        },
      },
      onClick: (event, elements) => {
        if (elements.length > 0) {
          const index = elements[0].index;
          const label = labels[index];
          const value = dataValues[index];
          onClick(label, value);
        }
      },
    },
  });
}

function createDonutChart(dataMap, chartName, onClick, canvaId) {
  const ctx = document.getElementById(canvaId).getContext("2d");
  const labels = dataMap.map((obj) => Object.keys(obj)[0]);
  const dataValues = dataMap.map((obj) => Object.values(obj)[0]);
  if (donutChart != null) {
    donutChart.destroy();
  }
  // Erstellen des Charts
  donutChart = new Chart(ctx, {
    type: "doughnut",
    data: {
      labels: labels,
      datasets: [
        {
          label: chartName,
          data: dataValues,
          backgroundColor: [
            "rgba(255, 99, 132, 0.2)",
            "rgba(54, 162, 235, 0.2)",
            "rgba(255, 206, 86, 0.2)",
            "rgba(75, 192, 192, 0.2)",
            "rgba(153, 102, 255, 0.2)",
            "rgba(255, 159, 64, 0.2)",
          ],
          borderColor: [
            "rgba(255, 99, 132, 1)",
            "rgba(54, 162, 235, 1)",
            "rgba(255, 206, 86, 1)",
            "rgba(75, 192, 192, 1)",
            "rgba(153, 102, 255, 1)",
            "rgba(255, 159, 64, 1)",
          ],
          borderWidth: 1,
        },
      ],
    },
    options: {
      onClick: (event, elements) => {
        if (elements.length > 0) {
          const index = elements[0].index;
          const label = labels[index];
          const value = dataValues[index];
          onClick(label, value);
        }
      },
    },
  });
}

function createBarChart(dataMap, chartName, onClick, canvaId) {
  const ctx = document.getElementById(canvaId).getContext("2d");
  const labels = dataMap.map((obj) => Object.keys(obj)[0]);
  const dataValues = dataMap.map((obj) => Object.values(obj)[0]);
  if (barChart != null) {
    barChart.destroy();
  }
  // Erstellen des Charts
  barChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: labels,
      datasets: [
        {
          label: chartName,
          data: dataValues,
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          borderColor: "rgba(75, 192, 192, 1)",
          borderWidth: 1,
        },
      ],
    },
    options: {
      scales: {
        y: {
          beginAtZero: true,
        },
      },
      onClick: (event, elements) => {
        if (elements.length > 0) {
          const index = elements[0].index;
          const label = labels[index];
          const value = dataValues[index];
          onClick(label, value);
        }
      },
    },
  });
}
