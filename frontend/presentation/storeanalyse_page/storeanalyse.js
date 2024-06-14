const storeBaseUrl = "http://localhost:3000/analyseStore";

function analyseStore() {
  getData()
    .then((data) => {
      if (data) {
        var firstChartContainer = document.getElementById(
          "firstChartContainer"
        );
        firstChartContainer.style.display = "block";
        createBarChart(data, "Test Chart", testOnclick, "test");
      }
    })
    .catch((error) => {
      console.error("Error in analyseStore:", error);
    });
}

function testOnclick() {
  console.log("test on click");
}

function getData() {
  const attribute = document.getElementById("attributeSelect").value;
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  const url = `${storeBaseUrl}?StartTime=${dateFrom}&EndTime=${dateTo}&Metrics=${attribute}`;

  return fetch(url, {
    method: "GET",
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .then((data) => {
      console.log(data);
      return data;
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function createMapChart() {}

function createLineChart() {}

function createDonutChart() {}

function createBarChart(dataMap, chartName, onClick, canvaId) {
  const ctx = document.getElementById(canvaId).getContext("2d");

  // Extrahieren der Schlüssel und Werte aus der Map
  const labels = Object.keys(dataMap);
  const dataValues = Object.values(dataMap);
  if (myChart != null) {
    myChart.destroy();
  }
  // Erstellen des Charts
  myChart = new Chart(ctx, {
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
