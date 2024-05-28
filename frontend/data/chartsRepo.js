const baseUrl = "http://localhost:3000/chart";
const barChartContainer = document.querySelector("#barChart");
const lineChartContainer = document.querySelector("#lineChart");

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
  const storeNames = [];
  const storeSums = [];

  storeDataMap.forEach((storeObject, storeName) => {
    storeNames.push(storeName);
    storeSums.push(storeObject.totalSum);
  });
  console.log(storeSums);
  const ctx = document.getElementById("barChart").getContext("2d");
  const barChart = new Chart(ctx, {
    type: "bar",
    data: {
      labels: storeNames,
      datasets: [
        {
          label: "# revenue",
          data: storeSums,
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
      scales: {
        y: {
          beginAtZero: true,
        },
      },
      onClick: (event, elements) => {
        if (elements.length > 0) {
          const elementIndex = elements[0].index;
          const storeName = storeNames[elementIndex];
          const storeSum = storeSums[elementIndex];
          handleBarClick(storeName, storeSums);
        }
      },
    },
  });
}

function handleBarClick(storeName, storeObject) {
  const ctx = document.getElementById("lineChart").getContext("2d");
  console.log(storeObject);
  // Extract months and their corresponding revenues
  const months = Object.keys(storeObject).filter((key) => key !== "totalSum");
  const revenues = months.map((month) => storeObject[month]);

  // Create the line chart
  window.lineChart = new Chart(ctx, {
    type: "line",
    data: {
      labels: months,
      datasets: [
        {
          label: `Revenue for ${storeName}`,
          data: revenues,
          borderColor: "rgba(75, 192, 192, 1)",
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          fill: true,
          borderWidth: 2,
        },
      ],
    },
    options: {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true,
        },
      },
    },
  });
}

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
      text: "US unemployment rate in Dec. 2017",
      align: "left",
    },

    subtitle: {
      text: "Small US map with data labels",
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
        [0.2, "#188e2a"], // Green
        [0.5, "#fee401"], // Yellow
        [1, "#df1309"], // Red
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
