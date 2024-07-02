let currentIndex = 0;
const slidesContainer = document.getElementById("slides");
const chartContainer = document.getElementById("chart-slides");
const leftArrow = document.getElementById("left-arrow");
const rightArrow = document.getElementById("right-arrow");
//const areaChartCtx = document.getElementById("areaChart").getContext("2d");
let areaChart;

const goalsByPeriod = {};
const actualDataByPeriod = {}; // To store actual data for each period
const chartInstances = {}; // To store chart instances

document.getElementById("openModal").addEventListener("click", function () {
  document.getElementById("inputWrapper").classList.add("show"); // Show the wrapper
});

function closeWrapper() {
  document.getElementById("inputWrapper").classList.remove("show"); // Hide the wrapper
}

// Optional: Close when clicking outside the form box
window.addEventListener("click", function (event) {
  let formBox = document.querySelector(".form-box");
  if (
    event.target == document.getElementById("inputWrapper") &&
    !formBox.contains(event.target)
  ) {
    closeWrapper();
  }
});

document
  .getElementById("dataForm")
  .addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent the form from submitting traditionally

    // Get the values from the form
    const topicSelect = document.getElementById("topicSelect").value;
    const periodSelect = document.getElementById("periodSelect").value;
    const dataInput = document.getElementById("dataInput").value;

    if (!goalsByPeriod[periodSelect]) {
      goalsByPeriod[periodSelect] = [];
    }

    goalsByPeriod[periodSelect].push({
      topic: topicSelect,
      target: parseInt(dataInput),
    });

    // Generate fake actual data for the period
    generateFakeActualData(periodSelect);

    // Create or update the slide for the period
    updatePeriodSlides(periodSelect);

    // Reset form and close wrapper
    document.getElementById("dataForm").reset();
    closeWrapper();

    // Update slides to show the new one
    currentIndex = slidesContainer.children.length - 1;
    updateSlidePosition();
  });

function generateFakeActualData(period) {
  if (!actualDataByPeriod[period]) {
    actualDataByPeriod[period] = [];
  }

  const weeksInPeriod = 12;
  let cumulativeSum = 0;
  let actualData = Math.floor(Math.random() * 1200) + 800; // Random actual value between 800 and 2000

  for (let i = 0; i < weeksInPeriod; i++) {
    let weeklyValue;
    if (i === weeksInPeriod - 1) {
      weeklyValue = actualData - cumulativeSum; // Make sure the last week's value makes the sum equal to the actual data
    } else {
      weeklyValue = Math.floor(Math.random() * (actualData / weeksInPeriod));
      cumulativeSum += weeklyValue;
    }
    actualDataByPeriod[period].push(weeklyValue);
  }

  // Set the actual value in the bar chart to the total cumulative value
  actualDataByPeriod[`${period}_total`] = actualData;
}

function updatePeriodSlides(period) {
  // Create or update the goal slide
  let goalSlide = document.querySelector(`.slide[data-period='${period}']`);
  if (!goalSlide) {
    goalSlide = document.createElement("div");
    goalSlide.classList.add("slide");
    goalSlide.setAttribute("data-period", period);
    goalSlide.innerHTML = `<h3>Period: ${period}</h3><ul></ul><button class="delete-button">Delete Period</button>`;
    slidesContainer.appendChild(goalSlide);
  }

  const goalList = goalSlide.querySelector("ul");
  goalList.innerHTML = ""; // Clear previous list
  goalsByPeriod[period].forEach((goal, index) => {
    const listItem = document.createElement("li");
    listItem.innerHTML = `Topic: ${goal.topic}, Number: ${goal.target} <br> <button class="zoom-icon" onclick="zoomGoal('${period}', ${index})"><i class="fas fa-search-plus"></i></button>`;
    goalList.appendChild(listItem);
  });

  // Create or update the chart slide
  let chartSlide = document.querySelector(
    `.chart-slide[data-period='${period}']`
  );
  if (!chartSlide) {
    chartSlide = document.createElement("div");
    chartSlide.classList.add("slide", "chart-slide");
    chartSlide.setAttribute("data-period", period);
    const chartCanvas = document.createElement("canvas");
    chartCanvas.style.width = "100%";
    chartCanvas.style.height = "250px"; // Adjust height to fit the box
    chartSlide.appendChild(chartCanvas);
    chartContainer.appendChild(chartSlide);

    // Create new chart instance
    chartInstances[period] = new Chart(chartCanvas, getChartConfig(period));
    chartCanvas.onclick = function (evt) {
      const activePoints = chartInstances[period].getElementsAtEventForMode(
        evt,
        "nearest",
        { intersect: true },
        false
      );
      if (activePoints.length) {
        const firstPoint = activePoints[0];
        const datasetLabel =
          chartInstances[period].data.datasets[firstPoint.datasetIndex].label;
        if (datasetLabel === "Actual") {
          const topic = goalsByPeriod[period][firstPoint.index].topic;
          createAreaChart(period, topic);
        }
      }
    };
  } else {
    // Update existing chart instance
    const chartCanvas = chartSlide.querySelector("canvas");
    updateChart(chartCanvas, period);
  }

  // Add delete event listener for the period
  goalSlide
    .querySelector(".delete-button")
    .addEventListener("click", function () {
      if (
        confirm("Are you sure you want to delete all goals for this period?")
      ) {
        delete goalsByPeriod[period];
        goalSlide.remove();
        chartSlide.remove();
        if (currentIndex >= slidesContainer.children.length) {
          currentIndex = slidesContainer.children.length - 1;
        }
        updateSlidePosition();
      }
    });
}

function zoomGoal(period, index) {
  const chart = chartInstances[period];
  const labels = chart.data.labels.slice(); // Copy labels
  const datasets = chart.data.datasets.map((dataset) => {
    return {
      ...dataset,
      data: dataset.data.slice(), // Copy data
    };
  });

  chart.data.labels = [labels[index]];
  chart.data.datasets.forEach((dataset, i) => {
    dataset.data = [datasets[i].data[index]];
  });

  chart.update();
}

function getChartConfig(period) {
  const periodGoals = goalsByPeriod[period];
  const labels = periodGoals.map((goal) => goal.topic);
  const targetData = periodGoals.map((goal) => goal.target);
  const actualData = periodGoals.map(
    (goal) => actualDataByPeriod[`${period}_total`] || 0
  ); // Use the total cumulative value

  return {
    type: "bar",
    data: {
      labels: labels,
      datasets: [
        {
          label: "Goal",
          data: targetData,
          backgroundColor: "#007bff",
        },
        {
          label: "Actual",
          data: actualData,
          backgroundColor: "#ffc107",
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
      plugins: {
        zoom: {
          zoom: {
            wheel: {
              enabled: false, // Disable zooming with mouse wheel
            },
            pinch: {
              enabled: true, // Enable zooming with pinch gesture
            },
            mode: "xy", // Allow zooming in both x and y directions
          },
          pan: {
            enabled: true,
            mode: "xy", // Allow panning in both x and y directions
            threshold: 10,
          },
        },
      },
    },
  };
}

function updateChart(canvas, period) {
  const chart = chartInstances[period];
  const periodGoals = goalsByPeriod[period];
  const labels = periodGoals.map((goal) => goal.topic);
  const targetData = periodGoals.map((goal) => goal.target);
  const actualData = periodGoals.map(
    (goal) => actualDataByPeriod[`${period}_total`] || 0
  ); // Use the total cumulative value

  chart.data.labels = labels;
  chart.data.datasets[0].data = targetData;
  chart.data.datasets[1].data = actualData;
  chart.update();
}

document
  .getElementById("resetZoomButton")
  .addEventListener("click", function () {
    const period = Object.keys(chartInstances)[currentIndex];
    if (period) {
      const chart = chartInstances[period];
      updateChart(chart.canvas, period); // Reset zoom by updating chart with original data
    }
  });

leftArrow.addEventListener("click", function () {
  if (currentIndex > 0) {
    currentIndex--;
    updateSlidePosition();
  }
});

rightArrow.addEventListener("click", function () {
  if (currentIndex < slidesContainer.children.length - 1) {
    currentIndex++;
    updateSlidePosition();
  }
});

function updateSlidePosition() {
  slidesContainer.style.transform = `translateX(-${currentIndex * 100}%)`;
  chartContainer.style.transform = `translateX(-${currentIndex * 100}%)`;
}

function createAreaChart(period, topic) {
  // Use the generated actual data and create cumulative data
  const weeklyData = actualDataByPeriod[period];
  const cumulativeData = weeklyData.reduce((acc, val, index) => {
    acc.push((acc[index - 1] || 0) + val);
    return acc;
  }, []);

  const weeklyLabels = Array.from({ length: 12 }, (_, i) => `Week ${i + 1}`);

  // Create or update the area chart
  if (areaChart) {
    areaChart.destroy();
  }

  areaChart = new Chart(areaChartCtx, {
    type: "line",
    data: {
      labels: weeklyLabels,
      datasets: [
        {
          label: "Cumulative Data",
          data: cumulativeData,
          fill: true,
          backgroundColor: "rgba(75, 192, 192, 0.2)",
          borderColor: "rgba(75, 192, 192, 1)",
          borderWidth: 1,
          pointBackgroundColor: "rgba(75, 192, 192, 1)",
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

function createDashboard() {
  const dashboardUrl =
    "http://localhost:5004/api/Dashboard?startDate=2021-01-01&endDate=2022-12-30";

  fetch(dashboardUrl)
    .then((response) => response.json())
    .then((data) => {
      console.log(data);

      const avgRevenuePerStore = Math.round(data.avgRevenuePerStore);
      const bestSellingProduct = data.bestSellingProduct;
      const mostPurchasedSize = data.mostPurchasedSize;
      const mostPurchasedCategory = data.mostPurchasedCategory;
      const averageCustomers = Math.round(data.averageCustomers);
      const averageSales = Math.round(data.averageSales);

      const top3Stores = data.top3Stores.map((store) => ({
        storeId: store.storeId,
        totalRevenue: Math.round(store.totalRevenue),
      }));

      const top3Products = data.top3Products.map((product) => ({
        sku: product.sku,
        totalOrders: Math.round(product.totalOrders),
      }));

      const top3Customers = data.top3Customers.map((customer) => ({
        customerId: customer.customerId,
        totalSpent: Math.round(customer.totalSpent),
      }));

      document.getElementById("avgSalesValue").innerHTML = averageSales;
      document.getElementById("totalSalesValue").innerHTML =
        Math.round(data.revenueByYearAndMonth["2021"]["Total"]) +
        Math.round(data.revenueByYearAndMonth["2022"]["Total"]);
      document.getElementById("avgRevenueValue").innerHTML = avgRevenuePerStore;
      document.getElementById("totalRevenueValue").innerHTML =
        Math.round(data.revenueByYearAndMonth["2021"]["Total"]) +
        Math.round(data.revenueByYearAndMonth["2022"]["Total"]);

      const topProductsElement = document.getElementById("topProductsValue");
      topProductsElement.innerHTML = ""; // Clear existing content
      top3Products.forEach((product) => {
        topProductsElement.innerHTML += `SKU: ${product.sku} <br>  Total Orders: ${product.totalOrders}<br>`;
      });

      const topStoresElement = document.getElementById("topStoresValue");
      topStoresElement.innerHTML = ""; // Clear existing content
      top3Stores.forEach((store) => {
        var storeName = getStoreKeyByValue(store.storeId);
        topStoresElement.innerHTML += `Store: ${storeName} <br>Total Revenue: ${store.totalRevenue}<br>`;
      });

      const topCustomersElement = document.getElementById("topCustomersValue");
      topCustomersElement.innerHTML = ""; // Clear existing content
      top3Customers.forEach((customer) => {
        topCustomersElement.innerHTML += `Customer ID: ${customer.customerId} <br>Total Spent: ${customer.totalSpent}<br>`;
      });

      // Prepare data for the chart
      const months = [
        "Jan",
        "Feb",
        "Mär",
        "Apr",
        "Mai",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Okt",
        "Nov",
        "Dez",
      ];
      const revenue2021 = months.map(
        (month) => data.revenueByYearAndMonth["2021"][month]
      );
      const revenue2022 = months.map(
        (month) => data.revenueByYearAndMonth["2022"][month]
      );

      // Create the chart
      const ctx = document.getElementById("revenueChart").getContext("2d");
      new Chart(ctx, {
        type: "line",
        data: {
          labels: months,
          datasets: [
            {
              label: "2021",
              data: revenue2021,
              borderColor: "rgba(75, 192, 192, 1)",
              borderWidth: 2,
              fill: false,
            },
            {
              label: "2022",
              data: revenue2022,
              borderColor: "rgba(153, 102, 255, 1)",
              borderWidth: 2,
              fill: false,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            x: {
              display: true,
              title: {
                display: true,
                text: "Month",
              },
            },
            y: {
              display: true,
              title: {
                display: true,
                text: "Revenue",
              },
            },
          },
        },
      });
      createMapChart();
    })
    .catch((error) => {
      console.error("Error:", error);
    });
}

function createMapChart() {
  const url = `http://localhost:5004/api/Mapcharts/MapChart?StartTime=2021-01-01&EndTime=2021-12-30&Attribute=revenue`;
  var mapChartDiv = document.getElementById("mapChartDiv");
  mapChartDiv.style.display = "block";

  fetch(url)
    .then((response) => response.json())
    .then((rows) => {
      const response = rows;
      const summedValuesPerStore = response.map((store) => {
        const { StoreId, State, City, Latitude, Longitude, ...months } = store;
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
        title: `Revenue per Store from Jan 2021 to Dec 2021`,
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
function getStoreKeyByValue(searchValue) {
  const storeNames = new StoreData().storeData;
  for (let [key, value] of Object.entries(storeNames)) {
    if (value === searchValue) {
      return key;
    }
  }
  return searchValue;
}

//document.addEventListener("DOMContentLoaded", createDashboard);
