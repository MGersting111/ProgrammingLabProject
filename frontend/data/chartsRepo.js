const baseUrl = "http://localhost:3000/chart";
const mapUrl = "http://localhost:3000/map";
const barChartContainer = document.querySelector("#barChart");
const lineChartContainer = document.querySelector("#lineChart");
const scatterChartContainer = document.querySelector("#scatterChart");
var barChart;
var lineChart;

function getData() {
  const model = document.getElementById("modelSelect").value;
  const attribute = document.getElementById("attributeSelect").value;
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  const limit = document.getElementById("limitSelect").value;

  const url = `${baseUrl}?model=${model}&attribute=${attribute}&dateFrom=${dateFrom}&dateTo=${dateTo}&limit=${limit}`;
  console.log(url);
  fetch(baseUrl, {
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
      processData(data);
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function processData(data) {
  const storeDataMap = new Map();

  data.forEach((store) => {
    Object.keys(store).forEach((storeName) => {
      const monthlyData = store[storeName][0];
      const totalSum = Object.values(monthlyData).reduce(
        (sum, value) => sum + value,
        0
      );

      // Create store object
      const storeObject = {
        totalSum,
        ...monthlyData,
      };

      storeDataMap.set(storeName, storeObject);
    });
  });

  console.log(storeDataMap);

  createBarChart(storeDataMap); // Call the chart creation function with the data map
}

function createBarChart(storeDataMap) {
  var barChartDiv = document.getElementById("barChartDiv");
  //barChartDiv.innerHTML = " ";
  barChartDiv.style.display = "block";
  var lineChartDiv = document.getElementById("lineChartDiv");
  //lineChartDiv.innerHTML = " ";
  lineChartDiv.style.display = "block";
  var mapChartDiv = document.getElementById("mapChartDiv");
  //mapChartDiv.innerHTML = " ";
  mapChartDiv.style.display = "none";
  var scatterChartDiv = document.getElementById("scatterChartDiv");
  //scatterChartDiv.innerHTML = " ";
  scatterChartDiv.style.display = "none";
  const storeNames = [];
  const storeSums = [];

  storeDataMap.forEach((storeObject, storeName) => {
    storeNames.push(storeName);
    storeSums.push(storeObject.totalSum);
  });
  if (barChart != null) {
    barChart.destroy();
  }
  console.log(storeSums);
  const ctx = document.getElementById("barChart").getContext("2d");
  barChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: storeNames,
      datasets: [
        {
          label: "# revenue",
          data: storeSums,
          backgroundColor: [
            "#EC9740",
            "#FFB347",
            "#FFC966",
            "#EFA94A",
            "#FFD700",
            "#8FBC8F",
            "#4682B4",
            "#6A5ACD",
            "#DAA520",
            "#CD5C5C",
            "#F08080",
          ],
          borderColor: [
            "#EC9740",
            "#FFB347",
            "#FFC966",
            "#EFA94A",
            "#FFD700",
            "#8FBC8F",
            "#4682B4",
            "#6A5ACD",
            "#DAA520",
            "#CD5C5C",
            "#F08080",
          ],
          borderWidth: 1,
        },
      ],
    },
    options: {
      scales: {
        x: {
          ticks: { color: "white" },
        },
        y: {
          ticks: { color: "white" },
          beginAtZero: true,
        },
      },
      onClick: (event, elements) => {
        console.log(elements);
        if (elements.length > 0) {
          const elementIndex = elements[0].index;
          const storeName = storeNames[elementIndex];
          const storeSum = storeSums[elementIndex];
          handleBarClick(storeName, storeDataMap);
        }
      },
    },
  });
}

function handleBarClick(storeName, storeDataMap) {
  const ctx = document.getElementById("lineChart").getContext("2d");
  const storeObject = storeDataMap.get(storeName);
  // Extract months and their corresponding revenues
  const months = Object.keys(storeObject).filter((key) => key !== "totalSum");
  const revenues = months.map((month) => storeObject[month]);
  if (window.lineChart != null) {
    window.lineChart.destroy();
  }
  // Create the line chart
  window.lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: months,
      datasets: [
        {
          label: `Revenue for ${storeName}`,
          data: revenues,
          backgroundColor: [
            "#EC9740",
            "#FFB347",
            "#FFC966",
            "#EFA94A",
            "#FFD700",
            "#8FBC8F",
            "#4682B4",
            "#6A5ACD",
            "#DAA520",
            "#CD5C5C",
            "#F08080",
          ],
          borderColor: [
            "#EC9740",
            "#FFB347",
            "#FFC966",
            "#EFA94A",
            "#FFD700",
            "#8FBC8F",
            "#4682B4",
            "#6A5ACD",
            "#DAA520",
            "#CD5C5C",
            "#F08080",
          ],
          fill: true,
          borderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      scales: {
        x: {
          ticks: { color: "white" },

          beginAtZero: true,
        },
        y: {
          ticks: { color: "white" },
          beginAtZero: true,
        },
      },
    },
  });
}

function createMapChart2() {
  var barChartDiv = document.getElementById("barChartDiv");
  //barChartDiv.innerHTML = " ";
  barChartDiv.style.display = "none";
  var lineChartDiv = document.getElementById("lineChartDiv");
  //lineChartDiv.innerHTML = " ";
  lineChartDiv.style.display = "none";
  var scatterChartDiv = document.getElementById("scatterChartDiv");
  //scatterChartDiv.innerHTML = " ";
  scatterChartDiv.style.display = "none";

  var mapChartDiv = document.getElementById("mapChartDiv");
  mapChartDiv.style.display = "block";
  fetchAndDisplayMapData();
  setTimeout(function () {
    // Code, der erst nach 2 Sekunden ausgeführt wird

    (async () => {
      const topology = await fetch(
        "https://code.highcharts.com/mapdata/countries/us/custom/us-small.topo.json"
      ).then((response) => response.json());

      // Load the data from the HTML table and tag it with an upper case name used
      // for joining
      const data = [];
      Highcharts.data({
        table: document.getElementById("data"),
        startColumn: 1,
        firstRowAsNames: false,
        complete: function (options) {
          options.series[0].data.forEach(function (p) {
            data.push({
              ucName: p[0],
              value: p[1],
            });
          });
        },
      });

      // Prepare map data for joining
      topology.objects.default.geometries.forEach(function (g) {
        if (g.properties && g.properties.name) {
          g.properties.ucName = g.properties.name.toUpperCase();
        }
      });

      // Initialize the chart
      Highcharts.mapChart("container", {
        title: {
          text: "Stores and their Revenue",
          align: "left",
        },

        subtitle: {
          text: "",
          align: "left",
        },

        mapNavigation: {
          enabled: true,
          enableButtons: false,
        },

        xAxis: {
          labels: {
            enabled: false,
          },
        },

        colorAxis: {
          labels: {
            format: "{value}%",
          },
          stops: [
            [0.2, "#808080"], // Grau
            [0.5, "#FFA500"], // Orange
            [1, "#00FF00"], // Strahlendes Grün
          ],
          min: 0,
          max: 8,
        },

        series: [
          {
            mapData: topology,
            data,
            joinBy: "ucName",
            name: "Unemployment rate per 2017",
            dataLabels: {
              enabled: true,
              format: "{point.properties.hc-a2}",
              style: {
                fontSize: "10px",
              },
            },
            tooltip: {
              valueSuffix: "%",
            },
          },
          {
            // The connector lines
            type: "mapline",
            data: Highcharts.geojson(topology, "mapline"),
            color: "silver",
            accessibility: {
              enabled: false,
            },
          },
        ],
      });
    })();
  }, 500);
}

async function fetchAndDisplayMapData() {
  try {
    const response = await fetch(mapUrl);
    const jsonData = await response.json();

    const table = document.getElementById("data");
    if (table.innerHTML != "") {
      table.innerHTML = "";
    }

    jsonData.forEach((item) => {
      const row = document.createElement("tr");

      const rankCell = document.createElement("td");
      rankCell.textContent = item.rank;
      row.appendChild(rankCell);

      const stateCell = document.createElement("td");
      stateCell.textContent = item.state;
      row.appendChild(stateCell);

      const valueCell = document.createElement("td");
      valueCell.textContent = item.value;
      row.appendChild(valueCell);

      // Apply background color based on value
      if (item.value <= 0.2) {
        valueCell.style.backgroundColor = "#808080"; // grey
      } else if (item.value <= 0.5) {
        valueCell.style.backgroundColor = "#FFA500"; //orange
      } else {
        valueCell.style.backgroundColor = "#7FFF00"; //green
      }

      table.appendChild(row);
    });
  } catch (error) {
    console.error("Error fetching the data:", error);
  }
}

async function createCorrelationChart() {
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  var barChartDiv = document.getElementById("barChartDiv");
  //barChartDiv.innerHTML = " ";
  barChartDiv.style.display = "none";
  var lineChartDiv = document.getElementById("lineChartDiv");
  //lineChartDiv.innerHTML = " ";
  lineChartDiv.style.display = "none";
  var scatterChartDiv = document.getElementById("scatterChartDiv");
  //scatterChartDiv.innerHTML = " ";
  scatterChartDiv.style.display = "block";

  var mapChartDiv = document.getElementById("mapChartDiv");
  mapChartDiv.style.display = "none";
  var corUrl = `http://localhost:5004/api/Correlation/Calculate?StartTime=${dateFrom}&EndTime=${dateTo}&FirstModel=Store&XAttribute=TotalRevenue&YAttribute=OrderCount`;

  try {
    const response = await fetch(corUrl);
    const data = await response.json();

    const xValues = data.xValues;
    const yValues = data.yValues;
    const correlation = data.correlation;

    // Display correlation above the chart
    document.getElementById(
      "correlation"
    ).innerText = `Correlation: ${correlation}`;

    // Prepare data for Chart.js
    const scatterData = xValues.map((x, index) => ({ x, y: yValues[index] }));

    // Create Scatter Chart
    const ctx = document.getElementById("scatterChart").getContext("2d");
    new Chart(ctx, {
      type: "scatter",
      data: {
        datasets: [
          {
            label: "Total Revenue vs Order Count",
            data: scatterData,
            backgroundColor: "#EC9740",
            borderColor: "#EC9740",
            pointBackgroundColor: "#EC9740",
            pointBorderColor: "#fff",
            pointHoverBackgroundColor: "#fff",
            pointHoverBorderColor: "#EC9740",
          },
        ],
      },
      options: {
        scales: {
          x: {
            type: "linear",
            position: "bottom",
            title: {
              display: true,
              text: "Total Revenue",
            },
            ticks: {
              color: "white",
            },
          },
          y: {
            title: {
              display: true,
              text: "Order Count",
            },
            ticks: {
              color: "white",
            },
          },
        },
      },
    });
  } catch (error) {
    console.error("Error fetching data:", error);
  }
}
