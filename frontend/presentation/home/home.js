function createDashboard() {
  console.log("createDashboard");
  const products = new Products();
  const dashboardUrl =
    "http://localhost:5004/api/Dashboard?startDate=2021-01-01&endDate=2022-12-30";

  fetch(dashboardUrl)
    .then((response) => response.json())
    .then((data) => {
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
        productName = products.productsData[product.sku];
        topProductsElement.innerHTML += `SKU: ${product.sku} - ${productName} <br>  Total Orders: ${product.totalOrders}<br>`;
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
              ticks: { color: "white" },
              title: {
                display: true,
                text: "Month",
                color: "white",
              },
            },
            y: {
              display: true,
              ticks: { color: "white" },
              title: {
                display: true,
                text: "Revenue",
                color: "white",
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
        var roundedValue = Math.round(summedValuesPerStore[i]);
        var currentText = storeNames[i] + " revenue: " + roundedValue + "$";
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
        title: `Revenue per Store`,
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
function getStoreKeyByValue(searchValue) {
  const storeNames = new StoreData().storeData;
  for (let [key, value] of Object.entries(storeNames)) {
    if (value === searchValue) {
      return key;
    }
  }
  return searchValue;
}

document.addEventListener("DOMContentLoaded", createDashboard);
