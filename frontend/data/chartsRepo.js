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
  var limit = document.getElementById("limitSelect").value;
  var stores = document.getElementById("stores");
  const selectedOptions = Array.from(stores.selectedOptions);
  const selectedValues = selectedOptions.map((option) => option.value);
  const selectedStoreId = selectedValues.join(",");
  if (selectedStoreId !== "") {
    limit = selectedValues.length;
  }

  const url = `${lineBarChartBaseUrl}?StoreId=${selectedStoreId}&StartTime=${dateFrom}&EndTime=${dateTo}&Metrics=${attribute}&comparisonType=${model}&Limit=${limit}`;
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
      var barChartDiv = document.getElementById("barChartDiv");
      barChartDiv.style.display = "block";
      var lineChartDiv = document.getElementById("lineChartDiv");
      lineChartDiv.style.display = "block";
      var mapChartDiv = document.getElementById("mapChartDiv");
      mapChartDiv.style.display = "none";
      var scatterChartDiv = document.getElementById("scatterChartDiv");
      scatterChartDiv.style.display = "none";
      var infoDiv = document.getElementById("infoDiv");
      infoDiv.style.display = "none";
      const objectNameList = data.map((store) => store.storeId);
      const objectTotalData = data.map((store) => store.total);
      const objectMonths = data.map((store) => {
        const months = [];
        for (const year in store.metricsByYear) {
          const yearMetrics = store.metricsByYear[year].metrics;
          const yearMonths = Object.keys(yearMetrics);
          months.push(...yearMonths);
        }
        return {
          storeId: store.storeId,
          months: months,
        };
      });
      const objectValues = data.map((store) => {
        const values = [];
        for (const year in store.metricsByYear) {
          const yearMetrics = store.metricsByYear[year].metrics;
          const yearMonths = Object.values(yearMetrics);
          values.push(...yearMonths);
        }
        return {
          storeId: store.storeId,
          values: values,
        };
      });
      if (model === "Store") {
        objectNameList.forEach((element) => {
          storeName = getKeyByValue(element);
          objectNameList[objectNameList.indexOf(element)] = storeName;
        });
      }
      console.log("StoreNames:", objectNameList);
      console.log("StoreTotalData:", objectTotalData);
      console.log("StoreMonths:", objectMonths);
      console.log("StoreValues:", objectValues);
      createInitialLineChart(
        objectNameList,
        objectTotalData,
        objectMonths,
        objectValues
      );
      createBarChart(
        objectNameList,
        objectTotalData,
        objectMonths,
        objectValues
      );
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function createBarChart(
  objectNameList,
  objectTotalData,
  objectMonths,
  objectValues
) {
  var initialLineChartCreated = true;

  if (barChart != null) {
    barChart.destroy();
  }

  const ctx = document.getElementById("barChart").getContext("2d");
  barChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: objectNameList,
      datasets: [
        {
          label: "# revenue",
          data: objectTotalData,
          backgroundColor: interpolateColors(objectNameList.length),
          borderColor: interpolateColors(objectNameList.length),
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
          var values = objectValues[elementIndex].values;
          var months = objectMonths[elementIndex].months;
          var objectName = objectNameList[elementIndex];
          var objectNameStore = getKeyByValue(objectName);
          if (objectNameStore !== undefined) {
            objectName = objectNameStore;
          }

          const storeName = objectNameList[elementIndex];
          if (initialLineChartCreated) {
            handleBarClick(
              months,
              values,
              objectName,
              interpolateColors(objectNameList.length)[elementIndex]
            );
            initialLineChartCreated = false;
          } else {
            createInitialLineChart(
              objectNameList,
              objectTotalData,
              objectMonths,
              objectValues
            );
            initialLineChartCreated = true;
          }
        }
      },
    },
  });
}

function createInitialLineChart(
  objectNameList,
  objectTotalData,
  objectMonths,
  objectValues
) {
  const ctx = document.getElementById("lineChart").getContext("2d");
  if (lineChart != null) {
    lineChart.destroy();
  }
  lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: objectMonths[0].months,
      datasets: objectValues.map((store, index) => ({
        label: objectNameList[index],
        data: store.values,
        backgroundColor: interpolateColors(objectNameList.length),
        borderColor: interpolateColors(objectNameList.length),
        borderWidth: 2,
      })),
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

function handleBarClick(months, values, storeName, color) {
  const ctx = document.getElementById("lineChart").getContext("2d");
  if (months.length > 12) {
    const valuesOne = values.slice(0, 12);
    const valuesTwo = values.slice(12, values.length);

    datasets = [
      {
        label: `Revenue for ${storeName} 2021`,
        data: valuesOne,
        borderColor: interpolateColors(2)[0],
        borderWidth: 2,
        fill: false,
      },
      {
        label: `Revenue for ${storeName} 2022`,
        data: valuesTwo,
        borderColor: interpolateColors(2)[1],
        borderWidth: 2,
        fill: false,
      },
    ];
  } else {
    datasets = [
      {
        label: `Revenue for ${storeName}`,
        data: values,
        borderColor: color,
        borderWidth: 2,
        fill: false,
      },
    ];
  }

  if (window.lineChart != null) {
    window.lineChart.destroy();
  }

  // Create the line chart
  window.lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: months.length <= 12 ? months : months.slice(0, 12),
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
      console.log(response);
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
        var currentText =
          storeNames[i] + " value: " + Math.round(summedValuesPerStore[i]);
        cityMonthlyValueList[storeNames[i]] = summedValuesPerStore[i];
        citySize.push(currentSize);
        hoverText.push(currentText);
      }

      // Berechnung der geografischen Mitte
      var avgLat =
        cityLat.reduce((a, b) => a + parseFloat(b), 0) / cityLat.length;
      var avgLon =
        cityLon.reduce((a, b) => a + parseFloat(b), 0) / cityLon.length;
      console.log(avgLat);
      console.log(avgLon);
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
      console.log(data);
      var layout = {
        title: `Revenue per Store from ${dateFrom} to ${dateTo}`,
        titlefont: {
          color: "White", // Farbe der Überschrift
        },
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
        var lineChartDiv = document.getElementById("mapLineChartDiv");
        lineChartDiv.style.display = "block";
        var mapInfoContainer = document.getElementById("mapInfoContainer");
        mapInfoContainer.style.display = "flex";
        mapInfoContainer.style.maxWidth = 800;
        var infoDiv = document.getElementById("infoDiv");
        infoDiv.style.display = "block";
        infoDiv.style.flexGrow = 1;
        lineChartDiv.style.flexGrow = 1;
        // Extrahiere die Indexnummer des angeklickten Punktes
        var pointIndex = data.points[0].pointIndex;
        // Extrahiere die Informationen für die angeklickte Stadt
        var clickedCityValues = storeDataMap[storeNames[pointIndex]];
        createMapStoreLineChart(storeNames[pointIndex], clickedCityValues);
        createMapInfoContainer(storeNames[pointIndex], dateFrom, dateTo);
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
  barChartDiv.style.display = "none";
  var lineChartDiv = document.getElementById("lineChartDiv");
  lineChartDiv.style.display = "none";
  var scatterChartDiv = document.getElementById("scatterChartDiv");
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
            backgroundColor: "rgb(65, 184, 213, 0.8)",
            borderWidth: 0,
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
  const ctx = document.getElementById("mapLineChart").getContext("2d");

  const months = Object.keys(monthValueMap);
  const revenues = months.map((month) => monthValueMap[month]);

  const dataset = {
    label: storeName,
    data: revenues,
    fill: false,
    borderColor: "rgb(65, 184, 213)",
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

function createMapInfoContainer(storeName, fromDate, toDate) {
  const storeData = new StoreData().storeData;
  storeName = storeName.replace(" ", " - ");
  const storeId = storeData[storeName];
  const url = `${baseUrl}?StoreId=${storeId}&OrderDateFrom=${fromDate}&OrderDateTo=${toDate}`;
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
      const infoDiv = document.getElementById("infoDiv");
      infoDiv.style.minWidth = 300;
      if (infoDiv.innerHTML !== "") {
        infoDiv.innerHTML = "";
      }
      data.forEach((item) => {
        const p = document.createElement("p");
        p.innerHTML = `
          <strong>Store ID:</strong> ${item.storeId}<br>
          <strong>Order Count:</strong> ${item.orderCount}<br>
          <strong>Total Revenue:</strong> ${Math.round(item.totalRevenue)}<br>
          <strong>Customer Count:</strong> ${item.customerCount}<br>
          <strong>Revenue Per Customer:</strong> ${Math.round(
            item.revenuePerCustomer
          )}<br>
          <hr>
        `;
        infoDiv.appendChild(p);
      });
    })
    .catch((error) => {
      console.error(
        "There has been a problem with your fetch operation:",
        error
      );
    });
}

//Hilfsfunktionen:
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

function interpolateColors(steps) {
  if (steps == 1) {
    return ["#004669"];
  }
  if (steps === undefined) {
    const steps = 35;
  }
  const color1 = "#004669";
  const color2 = "#00F9FF";

  const start = hexToRgb(color1);
  const end = hexToRgb(color2);
  const stepFactor = 1 / (steps - 1);
  const interpolatedColors = [];

  for (let i = 0; i < steps; i++) {
    const r = Math.round(start.r + (end.r - start.r) * (i * stepFactor));
    const g = Math.round(start.g + (end.g - start.g) * (i * stepFactor));
    const b = Math.round(start.b + (end.b - start.b) * (i * stepFactor));
    interpolatedColors.push(rgbToHex(r, g, b));
  }

  return interpolatedColors;
}

function hexToRgb(hex) {
  const bigint = parseInt(hex.slice(1), 16);
  const r = (bigint >> 16) & 255;
  const g = (bigint >> 8) & 255;
  const b = bigint & 255;
  return { r, g, b };
}

function rgbToHex(r, g, b) {
  return (
    "#" +
    ((1 << 24) + (r << 16) + (g << 8) + b).toString(16).slice(1).toUpperCase()
  );
}

//Update die Attribute je nach Model
const attributes = {
  store: [
    { value: "totalRevenue", text: "Revenue" },
    { value: "orderCount", text: "Order count" },
    { value: "averageordervalue", text: "Average Order-Value" },
    {
      value: "averageordervalueperstore",
      text: "Average Order-Value per Store",
    },
    {
      value: "averageordervaluepercustomer",
      text: "Average Order-Value per Customer",
    },
    {
      value: "totalrevenuepercustomerperstore",
      text: "Revenue per Customer per Store",
    },
    {
      value: "ordercountperproductperstore",
      text: "Orders per Product per Store",
    },
  ],
  product: [
    { value: "totalunitssold", text: "Total units sold" },
    { value: "price", text: "Price" },
    {
      value: "averageordervalueperproduct",
      text: "Average Order Value per Product",
    },
    { value: "totalrevenue", text: "Revenue" },
  ],
  customer: [
    { value: "averageordervalue", text: "Average order value" },
    { value: "ordercountpercustomer", text: "Orders per customer" },
    { value: "totalrevenuepercustomer", text: "Revenue per customer" },
  ],
};

function updateAttributes() {
  const model = document.getElementById("firstModel").value;
  const xSelect = document.getElementById("xAttribute");
  const ySelect = document.getElementById("yAttribute");

  xSelect.innerHTML = '<option value="" disabled selected>x-Attribute</option>';
  ySelect.innerHTML = '<option value="" disabled selected>y-Attribute</option>';

  if (model && attributes[model]) {
    attributes[model].forEach((attr) => {
      const optionX = document.createElement("option");
      optionX.value = attr.value;
      optionX.text = attr.text;
      xSelect.appendChild(optionX);

      const optionY = document.createElement("option");
      optionY.value = attr.value;
      optionY.text = attr.text;
      ySelect.appendChild(optionY);
    });
  }
}

//storeId zu storeName
function getKeyByValue(searchValue) {
  const storeNames = new StoreData().storeData;
  for (let [key, value] of Object.entries(storeNames)) {
    if (value === searchValue) {
      return key;
    }
  }
  return searchValue;
}
