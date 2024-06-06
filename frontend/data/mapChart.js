const mapUrl = "http://localhost:3000/map";
function createMapChart() {
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

  d3.csv(
    "https://raw.githubusercontent.com/plotly/datasets/master/2014_us_cities.csv",
    function (err, rows) {
      function unpack(rows, key) {
        return rows.map(function (row) {
          return row[key];
        });
      }

      var cityName = unpack(rows, "name"),
        cityPop = unpack(rows, "pop"),
        cityLat = unpack(rows, "lat"),
        cityLon = unpack(rows, "lon"),
        color = [
          ,
          "rgb(255,65,54)",
          "rgb(133,20,75)",
          "rgb(255,133,27)",
          "lightgrey",
        ],
        citySize = [],
        hoverText = [],
        scale = 50000;

      for (var i = 0; i < cityPop.length; i++) {
        var currentSize = cityPop[i] / scale;
        var currentText = cityName[i] + " pop: " + cityPop[i];
        citySize.push(currentSize);
        hoverText.push(currentText);
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
            line: {
              color: "black",
              width: 2,
            },
          },
        },
      ];

      var layout = {
        title: "2014 US City Populations",
        showlegend: false,
        geo: {
          scope: "usa",
          projection: {
            type: "albers usa",
          },
          showland: true,
          landcolor: "rgb(217, 217, 217)",
          subunitwidth: 1,
          countrywidth: 1,
          subunitcolor: "rgb(255,255,255)",
          countrycolor: "rgb(255,255,255)",
        },
      };

      Plotly.newPlot("mapChartDiv", data, layout, { showLink: false });
    }
  );
}
