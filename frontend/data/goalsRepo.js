const goalApiBaseUrl = "http://localhost:5004/api/Goal";
const actualDataApiBaseUrl =
  "http://localhost:5004/api/TotalNumber/FilteredStoreInfo";
const salesApiBaseUrl = "http://localhost:5004/api/ProductSales"; // New endpoint for product sales
const pieChartContainer = document.querySelector("#pieChartContainer");
let revenueChart;
let salesChart;
let pieChart;
let revenueGoal = {};
let salesGoal = {};
let actualRevenue = {};
let actualSales = {};
let monthlyRevenue = {};
let monthlySales = {}; // Add monthlySales for storing sales data
let currentPeriod = 1;

function postDataBasedOnInput(salesGoalValue, revenueGoalValue) {
  const topic = document.getElementById("topicSelect").value;
  const period = document.getElementById("periodSelect").value;
  const dataInput = document.getElementById("dataInput").value;

  const goal = {
    name: topic,
    period: parseInt(period),
    salesGoal:
      salesGoalValue !== undefined ? salesGoalValue : parseInt(dataInput),
    revenueGoal:
      revenueGoalValue !== undefined ? revenueGoalValue : parseFloat(dataInput), // Use revenueGoal if available
  };

  console.log("Posting new goal:", goal);

  return fetch(goalApiBaseUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(goal),
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Network response was not ok: ${response.status} ${response.statusText}`
        );
      }
      return response.json();
    })
    .then((data) => {
      console.log("Posted new goal:", data);
      return data;
    })
    .catch((error) => {
      console.error("Post goal error:", error);
      throw error;
    });
}

function getPeriodDateRange(period) {
  switch (parseInt(period)) {
    case 1:
      return { fromDate: "2022-01-01", toDate: "2022-03-31" };
    case 2:
      return { fromDate: "2022-04-01", toDate: "2022-06-30" };
    case 3:
      return { fromDate: "2022-07-01", toDate: "2022-09-30" };
    case 4:
      return { fromDate: "2022-10-01", toDate: "2022-12-31" };
    default:
      throw new Error("Invalid period selected.");
  }
}

function fetchDataBasedOnInput() {
  const topic = document.getElementById("topicSelect").value;
  const period = document.getElementById("periodSelect").value;
  const dataInput = document.getElementById("dataInput").value;
  const { fromDate, toDate } = getPeriodDateRange(period);

  let url = actualDataApiBaseUrl;
  if (topic === "SalesGoal" || topic === "RevenueGoal") {
    url = salesApiBaseUrl;
  }

  url = `${url}?fromDate=${fromDate}&toDate=${toDate}&topic=${topic}`;
  console.log(`Fetching data from URL: ${url}`);

  return fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error(
          `Network response was not ok: ${response.status} ${response.statusText}`
        );
      }
      return response.json();
    })
    .then((data) => {
      console.log("Fetched data based on input:", data);
      actualRevenue[period] = data.productRevenue["2022"].TotalRevenue;
      actualSales[period] = data.productSalesByMonth["2022"].Total;

      switch (parseInt(period)) {
        case 1:
          monthlyRevenue[period] = {
            Jan: data.productRevenue["2022"].Jan,
            Feb: data.productRevenue["2022"].Feb,
            Mar: data.productRevenue["2022"].Mär,
          };
          monthlySales[period] = {
            Jan: data.productSalesByMonth["2022"].Jan,
            Feb: data.productSalesByMonth["2022"].Feb,
            Mar: data.productSalesByMonth["2022"].Mär,
          };
          break;
        case 2:
          monthlyRevenue[period] = {
            Apr: data.productRevenue["2022"].Apr,
            May: data.productRevenue["2022"].Mai,
            Jun: data.productRevenue["2022"].Jun,
          };
          monthlySales[period] = {
            Apr: data.productSalesByMonth["2022"].Apr,
            May: data.productSalesByMonth["2022"].Mai,
            Jun: data.productSalesByMonth["2022"].Jun,
          };
          break;
        case 3:
          monthlyRevenue[period] = {
            Jul: data.productRevenue["2022"].Jul,
            Aug: data.productRevenue["2022"].Aug,
            Sep: data.productRevenue["2022"].Sep,
          };
          monthlySales[period] = {
            Jul: data.productSalesByMonth["2022"].Jul,
            Aug: data.productSalesByMonth["2022"].Aug,
            Sep: data.productSalesByMonth["2022"].Sep,
          };
          break;
        case 4:
          monthlyRevenue[period] = {
            Oct: data.productRevenue["2022"].Okt,
            Nov: data.productRevenue["2022"].Nov,
            Dec: data.productRevenue["2022"].Dez,
          };
          monthlySales[period] = {
            Oct: data.productSalesByMonth["2022"].Okt,
            Nov: data.productSalesByMonth["2022"].Nov,
            Dec: data.productSalesByMonth["2022"].Dez,
          };
          break;
        default:
          throw new Error("Invalid period selected.");
      }

      updateFrontend(
        dataInput,
        actualRevenue[period],
        actualSales[period],
        topic,
        period
      ); // Update frontend with the user input and actual data
      createSlide(period, topic, actualRevenue[period], actualSales[period]);
      return {
        salesGoal: parseInt(dataInput),
        revenueGoal: parseFloat(dataInput),
      };
    })
    .catch((error) => {
      console.error("Fetch data error:", error);
      throw error;
    });
}

function updateFrontend(
  userGoal,
  fetchedActualRevenue,
  fetchedActualSales,
  topic,
  period
) {
  if (topic === "RevenueGoal") {
    revenueGoal[period] = parseFloat(userGoal);
    actualRevenue[period] = fetchedActualRevenue;
  } else if (topic === "SalesGoal") {
    salesGoal[period] = parseInt(userGoal);
    actualSales[period] = fetchedActualSales;
  }
  createBarChart(period); // Update the chart
}

document
  .getElementById("dataForm")
  .addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent the form from submitting traditionally

    fetchDataBasedOnInput()
      .then(({ salesGoal, revenueGoal }) => {
        return postDataBasedOnInput(salesGoal, revenueGoal); // Pass salesGoal and revenueGoal to the post function
      })
      .then(() => {
        document.getElementById("dataForm").reset();
        closeWrapper();
      })
      .catch((error) => {
        console.error("Error during form submission:", error);
        alert("Error: " + error.message); // Display error message
      });
  });

function closeWrapper() {
  document.getElementById("inputWrapper").classList.remove("show"); // Hide the wrapper
}

document.addEventListener("DOMContentLoaded", function () {
  document.getElementById("openModal").addEventListener("click", function () {
    document.getElementById("inputWrapper").classList.add("show"); // Show the wrapper
  });

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

  // Arrow button event listeners
  document.getElementById("left-arrow").addEventListener("click", slideLeft);
  document.getElementById("right-arrow").addEventListener("click", slideRight);
});

function createSlide(period, topic, actualRevenue, actualSales) {
  const slidesContainer = document.getElementById("slides");

  // Find existing slide for the period or create a new one
  let slide = slidesContainer.querySelector(`.slide[data-period="${period}"]`);
  if (!slide) {
    slide = document.createElement("div");
    slide.className = "slide";
    slide.dataset.period = period;
    slidesContainer.appendChild(slide);
  }

  // Populate the slide with the necessary content
  let slideContent = `
        <h5>Period: ${period}</h5>
    `;
  if (revenueGoal[period] !== undefined) {
    slideContent += `<p>Actual Revenue: ${actualRevenue}</p>`;
    slideContent += `<p>Goal Revenue: ${revenueGoal[period]}</p>`;
  }
  if (salesGoal[period] !== undefined) {
    slideContent += `<p>Actual Sales: ${actualSales}</p>`;
    slideContent += `<p>Goal Sales: ${salesGoal[period]}</p>`;
  }

  // Add zoom buttons
  if (revenueGoal[period] !== undefined) {
    slideContent += `<button class="zoom-button" onclick="zoom('revenue', ${period})"><i class="fas fa-search-plus"></i></button>`;
  }
  if (salesGoal[period] !== undefined) {
    slideContent += `<button class="zoom-button" onclick="zoom('sales', ${period})"><i class="fas fa-search-plus"></i></button>`;
  }

  slide.innerHTML = slideContent;
  updateSlidesVisibility();
}

function updateSlidesVisibility() {
  const slides = document.querySelectorAll(".slide");
  slides.forEach((slide) => {
    slide.style.display =
      slide.dataset.period == currentPeriod ? "block" : "none";
  });
}

function slideLeft() {
  currentPeriod = currentPeriod > 1 ? currentPeriod - 1 : currentPeriod;
  updateSlidesVisibility();
  createBarChart(currentPeriod);
}

function slideRight() {
  currentPeriod = currentPeriod < 4 ? currentPeriod + 1 : currentPeriod;
  updateSlidesVisibility();
  createBarChart(currentPeriod);
}

function createBarChart(period) {
  const revenueCtx = document.querySelector("#revenueGoalChart").getContext("2d");

  // Data for revenue chart
  const revenueLabels = ["Revenue"];
  const revenueGoalData = revenueGoal[period] !== undefined ? [revenueGoal[period]] : [];
  const actualRevenueData = actualRevenue[period] !== undefined ? [actualRevenue[period]] : [];

  // Revenue chart data
  const revenueData = {
    labels: revenueLabels,
    datasets: [
      {
        label: "Goal",
        data: revenueGoalData,
        backgroundColor: "rgba(75, 192, 192, 1)",
        borderColor: "rgba(75, 192, 192, 1)",
        borderWidth: 1,
      },
      {
        label: "Actual",
        data: actualRevenueData,
        backgroundColor: "rgba(153, 102, 255, 1)",
        borderColor: "rgba(153, 102, 255, 1)",
        borderWidth: 1,
      },
    ],
  };

  const options = {
    responsive: true,
    scales: {
      y: {
        beginAtZero: true,
      },
    },
    plugins: {
      title: {
        display: true,
        text: `Period ${period}`,
      },
      zoom: {
        pan: {
          enabled: true,
          mode: "x",
        },
        zoom: {
          wheel: {
            enabled: true,
          },
          pinch: {
            enabled: true,
          },
          mode: "x",
        },
      },
    },
    onClick: (e, elements) => {
      if (elements.length > 0) {
        const clickedElementIndex = elements[0].index;
        const label = revenueData.labels[clickedElementIndex];
        if (label === "Revenue" && monthlyRevenue[period]) {
          createPieChart(period, monthlyRevenue[period], "Revenue Breakdown");
        }
      }
    },
  };

  if (revenueChart) {
    revenueChart.destroy();
  }
  revenueChart = new Chart(revenueCtx, {
    type: "bar",
    data: revenueData,
    options: options,
  });

  // Conditionally create the sales chart only if salesGoal exists for the period
  if (salesGoal[period] !== undefined) {
    const salesCtx = document.querySelector("#salesChart").getContext("2d");

    // Data for sales chart
    const salesLabels = ["Sales"];
    const salesGoalData = [salesGoal[period]];
    const actualSalesData = [actualSales[period]];

    // Sales chart data
    const salesData = {
      labels: salesLabels,
      datasets: [
        {
          label: "Goal",
          data: salesGoalData,
          backgroundColor: "rgba(75, 192, 192, 1)",
          borderColor: "rgba(75, 192, 192, 1)",
          borderWidth: 1,
        },
        {
          label: "Actual",
          data: actualSalesData,
          backgroundColor: "rgba(153, 102, 255, 1)",
          borderColor: "rgba(153, 102, 255, 1)",
          borderWidth: 1,
        },
      ],
    };

    const salesOptions = {
      responsive: true,
      scales: {
        y: {
          beginAtZero: true,
        },
      },
      plugins: {
        title: {
          display: true,
          text: `Period ${period}`,
        },
        zoom: {
          pan: {
            enabled: true,
            mode: "x",
          },
          zoom: {
            wheel: {
              enabled: true,
            },
            pinch: {
              enabled: true,
            },
            mode: "x",
          },
        },
      },
      onClick: (e, elements) => {
        if (elements.length > 0) {
          const clickedElementIndex = elements[0].index;
          const label = salesData.labels[clickedElementIndex];
          if (label === "Sales" && monthlySales[period]) {
            createPieChart(period, monthlySales[period], "Sales Breakdown");
          }
        }
      },
    };

    if (salesChart) {
      salesChart.destroy();
    }
    salesChart = new Chart(salesCtx, {
      type: "bar",
      data: salesData,
      options: salesOptions,
    });
  }
}

function createPieChart(period, data, title) {
  const pieChartContainer = document.querySelector("#pieChartContainer");
  pieChartContainer.style.display = "block";
  pieChartContainer.innerHTML =
    '<canvas id="pieChart" style="display: block;"></canvas>'; // Reset pie chart container

  const ctx = document.querySelector("#pieChart").getContext("2d");

  const pieData = {
    labels: Object.keys(data),
    datasets: [
      {
        label: `${title} for Period ${period}`,
        data: Object.values(data),
        backgroundColor: [
          'rgba(0, 123, 255, 0.8)',  // Blue
          'rgba(123, 0, 255, 0.8)',   // Purple
          'rgba(64, 224, 208, 0.8)' 
        ],
        borderColor: [
          'rgba(0, 123, 255, 0.8)',  // Blue
          'rgba(123, 0, 255, 0.8)',   // Purple
          'rgba(64, 224, 208, 0.8)' 
        ],
        borderWidth: 1,
      },
    ],
  };

  const options = {
    responsive: true,
    //maintainAspectRatio: false, // Ensure the pie chart is smaller
    plugins: {
      title: {
        display: true,
        text: `${title} for Period ${period}`,
      },
    },
  };

  if (pieChart) {
    pieChart.destroy();
  }
  pieChart = new Chart(ctx, {
    type: "pie",
    data: pieData,
    options: options,
  });
}

function zoom(type, period) {
  if (type === "revenue") {
    createPieChart(period, monthlyRevenue[period], "Revenue Breakdown");
  } else if (type === "sales") {
    createPieChart(period, monthlySales[period], "Sales Breakdown");
  }
}

// Reset Zoom-Funktion
document
  .getElementById("resetZoomButton")
  .addEventListener("click", function () {
    if (revenueChart) revenueChart.resetZoom();
    if (salesChart) salesChart.resetZoom();
  });
