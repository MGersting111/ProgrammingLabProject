const lineBarChartBaseUrl =
  "http://localhost:5004/api/CompareCharts/ChartsInfos";
const mapChartBaseUrl = "http://localhost:5004/api/Mapcharts/MapChart";
const barChartContainer = document.querySelector("#barChart");
const lineChartContainer = document.querySelector("#lineChart");
const scatterChartContainer = document.querySelector("#scatterChart");
var barChart;
var lineChart;
var scatterChart;

function createBarLineChart() {
  const model = document.getElementById("modelSelect").value;
  const attribute = document.getElementById("attributeSelect").value;
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  const limit = document.getElementById("limitSelect").value;

  const url = `${lineBarChartBaseUrl}?StartTime=${dateFrom}&EndTime=${dateTo}&Metrics=${attribute}&comparisonType=${model}&Limit=${limit}`;
  console.log(url);
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
      processData(data);
      console.log(data);
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function processData(data) {
  const storeDataMap = new Map();

  data.forEach((store) => {
    const storeName = getKeyByValue(store.storeId);
    const monthlyData = store.metrics;
    const totalSum = store.total;

    // Create store object
    const storeObject = {
      totalSum,
      ...monthlyData,
    };

    storeDataMap.set(storeName, storeObject);
  });

  console.log(storeDataMap);

  createBarChart(storeDataMap); // Call the chart creation function with the data map
  createInitialLineChart(storeDataMap); // Create the initial line chart for all stores
}

function getKeyByValue(searchValue) {
  const storeNames = new StoreData().storeData;
  for (let [key, value] of Object.entries(storeNames)) {
    if (value === searchValue) {
      return key;
    }
  }
  return searchValue;
}

function createBarChart(storeDataMap) {
  var initialLineChartCreated = true;
  var barChartDiv = document.getElementById("barChartDiv");
  barChartDiv.style.display = "block";
  var lineChartDiv = document.getElementById("lineChartDiv");
  lineChartDiv.style.display = "block";
  var mapChartDiv = document.getElementById("mapChartDiv");
  mapChartDiv.style.display = "none";
  var scatterChartDiv = document.getElementById("scatterChartDiv");
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
        if (elements.length > 0) {
          const elementIndex = elements[0].index;
          const storeName = storeNames[elementIndex];
          if (initialLineChartCreated) {
            handleBarClick(storeName, storeDataMap);
            initialLineChartCreated = false;
          } else {
            createInitialLineChart(storeDataMap);
            initialLineChartCreated = true;
          }
        }
      },
    },
  });
}

function createInitialLineChart(storeDataMap) {
  console.log(storeDataMap);
  const ctx = document.getElementById("lineChart").getContext("2d");

  const datasets = [];
  storeDataMap.forEach((storeObject, storeName) => {
    const months = Object.keys(storeObject).filter((key) => key !== "totalSum");
    const revenues = months.map((month) => storeObject[month]);
    datasets.push({
      label: storeName,
      data: revenues,
      fill: false,
      borderColor: getRandomColor(),
      borderWidth: 2,
    });
  });

  if (lineChart != null) {
    lineChart.destroy();
  }

  lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: Object.keys(storeDataMap.values().next().value).filter(
        (key) => key !== "totalSum"
      ),
      datasets: datasets,
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

function getRandomColor() {
  const letters = "0123456789ABCDEF";
  let color = "#";
  for (let i = 0; i < 6; i++) {
    color += letters[Math.floor(Math.random() * 16)];
  }
  return color;
}

function createMapChart() {
  const dateFrom = document.getElementById("fromDateMap").value;
  const dateTo = document.getElementById("toDateMap").value;
  const attribute = document.getElementById("attributeMap").value;
  const url = `${mapChartBaseUrl}?StartTime=${dateFrom}&EndTime=${dateTo}&Attribute=${attribute}`;

  var barChartDiv = document.getElementById("barChartDiv");
  barChartDiv.style.display = "none";
  var lineChartDiv = document.getElementById("lineChartDiv");
  lineChartDiv.style.display = "none";
  var scatterChartDiv = document.getElementById("scatterChartDiv");
  scatterChartDiv.style.display = "none";

  var mapChartDiv = document.getElementById("mapChartDiv");
  mapChartDiv.style.display = "block";

  fetch(url)
    .then((response) => response.json())
    .then((rows) => {
      const response = rows;
      const summedValuesPerStore = response.map((store) => {
        const { State, City, Latitude, Longitude, ...months } = store;
        const total = Object.values(months).reduce((acc, val) => acc + val, 0);
        return total;
      });

      // Liste mit Storenamen (State + City)
      const storeNames = response.map(
        (store) => `${store.State} ${store.City}`
      );

      // Map/Liste mit Storenamen und den jeweiligen Monaten mit Werten
      const storeDataMap = response.reduce((acc, store) => {
        const { State, City, Latitude, Longitude, ...months } = store;
        const storeName = `${State} ${City}`;
        acc[storeName] = months;
        return acc;
      }, {});

      function unpack(rows, key) {
        return rows.map((row) => row[key]);
      }
      var cityMonthlyValueList = {};
      var cityLat = unpack(rows, "Latitude");
      var cityLon = unpack(rows, "Longitude");

      (citySize = []), (hoverText = []), (scale = 50000);

      for (var i = 0; i < summedValuesPerStore.length; i++) {
        var currentSize = summedValuesPerStore[i] / scale;
        var currentText = storeNames[i] + " value: " + summedValuesPerStore[i];
        cityMonthlyValueList[storeNames[i]] = summedValuesPerStore[i];
        citySize.push(currentSize);
        hoverText.push(currentText);
      }

      // Berechnung der geografischen Mitte
      var avgLat =
        cityLat.reduce((a, b) => a + parseFloat(b), 0) / cityLat.length;
      var avgLon =
        cityLon.reduce((a, b) => a + parseFloat(b), 0) / cityLon.length;

      var data = [
        {
          type: "scattergeo",
          locationmode: "USA-states",
          lat: cityLat,
          lon: cityLon,
          hoverinfo: "text",
          text: hoverText,
          marker: {
            size: citySize,
            line: {
              color: "black",
              width: 2,
            },
          },
        },
      ];

      var layout = {
        title: `Revenue per Store from ${dateFrom} to ${dateTo}`,
        showlegend: false,

        width: 1200, // Breite des Diagramms
        height: 800, // Höhe des Diagramms
        margin: {
          l: 100, // linker Rand
          r: 100, // rechter Rand
          t: 100, // oberer Rand
          b: 100, // unterer Rand
        },
        paper_bgcolor: "#77b0aa",
        geo: {
          scope: "nevada",
          projection: {
            type: "albers usa",
          },
          showland: true,
          landcolor: "rgb(217, 217, 217)",
          subunitwidth: 1,
          countrywidth: 1,
          subunitcolor: "rgb(255,255,255)",
          countrycolor: "rgb(255,255,255)",
          center: {
            lat: avgLat,
            lon: avgLon,
          },
          zoom: 4, // erhöhtes Zoom-Level
        },
      };

      Plotly.newPlot("mapChartDiv", data, layout, { showLink: false });

      // Event-Handler für das Klicken auf einen Punkt
      mapChartDiv.on("plotly_click", function (data) {
        // Extrahiere die Indexnummer des angeklickten Punktes
        var pointIndex = data.points[0].pointIndex;
        // Extrahiere die Informationen für die angeklickte Stadt
        var clickedCityValues = storeDataMap[storeNames[pointIndex]];
        createMapStoreLineChart(storeNames[pointIndex], clickedCityValues);
      });
    })
    .catch((err) => console.error(err));
}

async function createCorrelationChart() {
  const dateFrom = document.getElementById("fromDateCor").value;
  const dateTo = document.getElementById("toDateCor").value;
  const firstModel = document.getElementById("firstModel").value;
  const xAttribute = document.getElementById("xAttribute").value;
  const yAttribute = document.getElementById("yAttribute").value;
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

  var corUrl = `http://localhost:5004/api/Correlation/Calculate?StartTime=${dateFrom}&EndTime=${dateTo}&FirstModel=${firstModel}&XAttribute=${xAttribute}&YAttribute=${yAttribute}`;

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
    if (scatterChart != null) {
      scatterChart.destroy();
    }
    scatterChart = new Chart(ctx, {
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

function createMapStoreLineChart(storeName, monthValueMap) {
  var lineChartDiv = document.getElementById("lineChartDiv");
  lineChartDiv.style.display = "block";
  const ctx = document.getElementById("lineChart").getContext("2d");

  const months = Object.keys(monthValueMap);
  const revenues = months.map((month) => monthValueMap[month]);
  console.log(months);
  console.log(revenues);

  const dataset = {
    label: storeName,
    data: revenues,
    fill: false,
    borderColor: getRandomColor(),
    borderWidth: 2,
  };

  if (lineChart != null) {
    lineChart.destroy();
  }

  lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: months,
      datasets: [dataset],
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

function toggleChartOptions() {
  var chartType = document.getElementById("chartType").value;
  var barLineOptions = document.getElementById("barLineOptions");
  var correlationOptions = document.getElementById("correlationOptions");
  var mapOptions = document.getElementById("mapOptions");

  if (chartType === "barLine") {
    barLineOptions.style.display = "flex";
    correlationOptions.style.display = "none";
    mapOptions.style.display = "none";
  } else if (chartType === "correlation") {
    barLineOptions.style.display = "none";
    correlationOptions.style.display = "flex";
    mapOptions.style.display = "none";
  } else if (chartType === "map") {
    barLineOptions.style.display = "none";
    correlationOptions.style.display = "none";
    mapOptions.style.display = "flex";
  }
}
