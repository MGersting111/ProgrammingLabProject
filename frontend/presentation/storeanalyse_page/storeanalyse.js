const storeBaseUrl = "http://localhost:5004/api/StoreMetrics";
let months = [
  "Jan",
  "Feb",
  "Mar",
  "Apr",
  "May",
  "Jun",
  "Jul",
  "Aug",
  "Sep",
  "Oct",
  "Nov",
  "Dec",
];
let colors = [
  "#1f77b4",
  "#aec7e8",
  "#ff7f0e",
  "#ffbb78",
  "#2ca02c",
  "#98df8a",
  "#d62728",
  "#ff9896",
  "#9467bd",
  "#c5b0d5",
  "#8c564b",
  "#c49c94",
  "#e377c2",
  "#f7b6d2",
  "#7f7f7f",
  "#c7c7c7",
  "#bcbd22",
  "#dbdb8d",
  "#17becf",
  "#9edae5",
  "#7b4173",
  "#a55194",
  "#ce6dbd",
  "#de9ed6",
  "#3182bd",
  "#6baed6",
  "#e6550d",
  "#fd8d3c",
  "#fdae6b",
  "#fdd0a2",
  "#31a354",
  "#74c476",
];
let months24 = [...months, ...months];
const storeIds = [];
const storeNames = [];
const storeLat = [];
const storeLong = [];
const storeTotalSales = [];
const storeAvgSales = [];
const storeTotalRevenue = [];
const storeAvgRevenue = [];
const storeTotalCustomers = [];
const storeRevenueValues = [];
const storeSaleValues = [];
const storeCustomerValues = [];
const storeAvgRevenueValues = [];
var blackColors;
var dateFrom;
var dateTo;
var dataSet;
var barChart;
var lineChart;
var donutChart;
let revenueChart;
let salesChart;
let avgRevenueChart;
let customerChart;
let currentStores = [];

function resetStoreData() {
  currentStores = [];
  revenueChart = null;
  salesChart = null;
  avgRevenueChart = null;
  customerChart = null;
  document.getElementById("canvasContainer").innerHTML = "";
  document.getElementById("mapContainer").innerHTML = "";
  Plotly.purge("mapContainer");
  blackColors = new Array(storeIds.length).fill("#000000");
  storeIds.length = 0;
  storeNames.length = 0;
  storeLat.length = 0;
  storeLong.length = 0;
  storeTotalSales.length = 0;
  storeAvgSales.length = 0;
  storeTotalRevenue.length = 0;
  storeAvgRevenue.length = 0;
  storeTotalCustomers.length = 0;
  storeRevenueValues.length = 0;
  storeSaleValues.length = 0;
  storeCustomerValues.length = 0;
  storeAvgRevenueValues.length = 0;
}

async function analyseStore() {
  resetStoreData();

  var data = await getData();
  if (data) {
    dataSet = orderData(data);
    createMapChart(
      dataSet.storeLat,
      dataSet.storeLong,
      dataSet.storeTotalRevenue,
      dataSet.storeNames
    );
  }
}

function getData() {
  dateFrom = document.getElementById("fromDate").value;
  dateTo = document.getElementById("toDate").value;
  let yearFrom = dateFrom.split("-")[0];
  let yearTo = dateTo.split("-")[0];

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
  const storeLocator = new StoreLocator();
  const storeLocatorData = storeLocator.StoreLocatorData;
  data.forEach((store) => {
    storeIds.push(store.storeId);
    storeNames.push(store.city + ", " + store.state);
    if (storeLocatorData[store.storeId] != undefined) {
      storeLat.push(storeLocatorData[store.storeId].latitude);
      storeLong.push(storeLocatorData[store.storeId].longitude);
    }
    storeTotalSales.push(store.totalSales);
    storeAvgSales.push(store.avgSales);
    storeTotalRevenue.push(store.totalRevenue);
    storeAvgRevenue.push(store.avgRevenue);
    storeTotalCustomers.push(store.totalCustomers);
    let allMonthlyRevenues = [];
    let allMonthlySales = [];
    let allMonthlyCustomers = [];
    let allMonthlyAvgRevenuePerSale = [];
    for (let year in store.monthlyRevenue) {
      let yearlyRevenue = Object.values(store.monthlyRevenue[year]);
      allMonthlyRevenues = allMonthlyRevenues.concat(yearlyRevenue);
      let yearlySales = Object.values(store.monthlySales[year]);
      allMonthlySales = allMonthlySales.concat(yearlySales);
      let yearlyCustomers = Object.values(store.monthlyCustomers[year]);
      allMonthlyCustomers = allMonthlyCustomers.concat(yearlyCustomers);
      let yearlyAvgRevenuePerSale = Object.values(
        store.monthlyAvgRevenuePerSale[year]
      );
      allMonthlyAvgRevenuePerSale = allMonthlyAvgRevenuePerSale.concat(
        yearlyAvgRevenuePerSale
      );
    }
    storeRevenueValues.push(allMonthlyRevenues);
    storeSaleValues.push(allMonthlySales);
    storeAvgRevenueValues.push(allMonthlyAvgRevenuePerSale);
    storeCustomerValues.push(allMonthlyCustomers);
  });

  months24 = months24.slice(0, storeRevenueValues[0].length);
  return {
    storeIds,
    storeNames,
    storeLat,
    storeLong,
    storeTotalSales,
    storeAvgSales,
    storeTotalRevenue,
    storeAvgRevenue,
    storeTotalCustomers,
    storeRevenueValues,
    storeSaleValues,
    storeCustomerValues,
    storeAvgRevenueValues,
  };
}
//-------------------------------
function createMapChart(cityLat, cityLon, cityRevenue, cityName) {
  const mapContainer = document.getElementById("mapContainer");
  mapContainer.style.display = "block";
  const scale = 120000;
  var citySize = [];
  var hoverText = [];

  for (var i = 0; i < cityRevenue.length; i++) {
    citySize.push(cityRevenue[i] / scale);
    hoverText.push(
      cityName[i] +
        "<br>Revenue: " +
        Math.round(cityRevenue[i]) +
        "<br>Customer: " +
        Math.round(storeTotalCustomers[i])
    );
  }

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
        color: blackColors,
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

    width: 1000, // Breite des Diagramms
    height: 600, // Höhe des Diagramms
    margin: {
      l: 1, // linker Rand
      r: 1, // rechter Rand
      t: 50, // oberer Rand
      b: 1, // unterer Rand
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
        lat: 38,
        lon: -118,
      },
      zoom: 10,
    },
  };

  Plotly.newPlot("mapContainer", data, layout, { showLink: false });
  mapContainer.on("plotly_click", function (data) {
    const pointIndex = data.points[0].pointIndex;

    if (currentStores.includes(pointIndex)) {
      currentStores.splice(currentStores.indexOf(pointIndex), 1);
      blackColors[pointIndex] = "#000000";
      Plotly.restyle("mapContainer", { "marker.color": [blackColors] });
      removeDataFromLineChart(revenueChart, cityName[pointIndex] + " Revenue");
      removeDataFromLineChart(salesChart, cityName[pointIndex] + " Sales");
      removeDataFromLineChart(
        avgRevenueChart,
        cityName[pointIndex] + " Avg Revenue"
      );
      removeDataFromLineChart(
        customerChart,
        cityName[pointIndex] + " Customers"
      );
    } else {
      currentStores.push(pointIndex);

      blackColors[pointIndex] = colors[pointIndex];

      Plotly.restyle("mapContainer", { "marker.color": [blackColors] });

      if (revenueChart != null) {
        addDataToLineChart(
          revenueChart,
          storeRevenueValues[pointIndex],
          storeNames[pointIndex] + " Revenue",
          colors[pointIndex]
        );
        addDataToLineChart(
          salesChart,
          storeSaleValues[pointIndex],
          storeNames[pointIndex] + " Sales",
          colors[pointIndex]
        );
        addDataToLineChart(
          avgRevenueChart,
          storeAvgRevenueValues[pointIndex],
          storeNames[pointIndex] + " Avg Revenue",
          colors[pointIndex]
        );
        addDataToLineChart(
          customerChart,
          storeCustomerValues[pointIndex],
          storeNames[pointIndex] + " Customers",
          colors[pointIndex]
        );
      } else {
        revenueChart = createLineChart(
          months24,
          storeRevenueValues[pointIndex],
          storeNames[pointIndex] + " Revenue",
          () => {
            console.log("test on click");
          },
          colors[pointIndex]
        );
        salesChart = createLineChart(
          months24,
          storeSaleValues[pointIndex],
          storeNames[pointIndex] + " Sales",
          () => {
            console.log("test on click");
          },
          colors[pointIndex]
        );
        avgRevenueChart = createLineChart(
          months24,
          storeAvgRevenueValues[pointIndex],
          storeNames[pointIndex] + " Avg Revenue",
          () => {
            console.log("test on click");
          },
          colors[pointIndex]
        );
        customerChart = createLineChart(
          months24,
          storeCustomerValues[pointIndex],
          storeNames[pointIndex] + " Customers",
          () => {
            console.log("test on click");
          },
          colors[pointIndex]
        );
      }
    }
  });
}

//-------------------
function createLineChart(dataLabels, dataValues, chartName, onClick, color) {
  let canvasContainer = document.getElementById("canvasContainer");
  let newCanvas = document.createElement("canvas");
  newCanvas.className = "canvas-item";
  canvasContainer.appendChild(newCanvas);

  let ctx = newCanvas.getContext("2d");

  let chart = new Chart(ctx, {
    type: "line",
    data: {
      labels: dataLabels,
      datasets: [
        {
          label: chartName,
          data: dataValues,
          backgroundColor: color,
          borderColor: color,
          borderWidth: 1,
        },
      ],
    },
    options: {
      scales: {
        x: {
          ticks: { color: "white" },
          beginAtZero: true,
        },
        y: {
          ticks: { color: "white" },
          beginAtZero: false,
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

  return chart;
}

function addDataToLineChart(chart, dataValues, chartName, color) {
  chart.data.datasets.push({
    label: chartName,
    data: dataValues,
    backgroundColor: color,
    borderColor: color,
    borderWidth: 1,
  });
  chart.update();
}

function removeDataFromLineChart(chart, chartName) {
  const datasetIndex = chart.data.datasets.findIndex(
    (dataset) => dataset.label === chartName
  );
  if (datasetIndex !== -1) {
    chart.data.datasets.splice(datasetIndex, 1);
    chart.update();
  }
}

//----------------------

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
