const goalApiBaseUrl = "http://localhost:5004/api/Goal";
const actualDataApiBaseUrl =
  "http://localhost:5004/api/TotalNumber/FilteredStoreInfo";

// Fetch all goals
function fetchGoals() {
  return fetch(goalApiBaseUrl)
    .then((response) => {
      //if (!response.ok) {
      //throw new Error(`Network response was not ok: ${response.statusText}`);
      //}
      return response.json();
    })
    .catch((error) => {
      console.error("Fetch goals error:", error);
      return [];
    });
}

// Fetch actual data
function fetchActualData(period, topic) {
  const url = `${actualDataApiBaseUrl}?period=${period}&topic=${topic}`;
  console.log(`Fetching actual data from URL: ${url}`);
  return fetch(url)
    .then((response) => {
      if (!response.ok) {
        throw new Error(`Network response was not ok: ${response.statusText}`);
      }
      return response.json();
    })
    .catch((error) => {
      console.error("Fetch actual data error:", error);
      return [];
    });
}

// Add a new goal
function addGoal(goal) {
  return fetch(goalApiBaseUrl, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(goal),
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error(`Network response was not ok: ${response.statusText}`);
      }
      return response.json();
    })
    .catch((error) => {
      console.error("Add goal error:", error);
      return null;
    });
}

// Delete a goal
function deleteGoal(goalId) {
  return fetch(`${goalApiBaseUrl}/${goalId}`, {
    method: "DELETE",
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error(`Network response was not ok: ${response.statusText}`);
      }
      return true;
    })
    .catch((error) => {
      console.error("Delete goal error:", error);
      return false;
    });
}

// Main logic for managing goals and charts
document.addEventListener("DOMContentLoaded", function () {
  let currentIndex = 0;
  const slidesContainer = document.getElementById("slides");
  const chartContainer = document.getElementById("chart-slides");
  const leftArrow = document.getElementById("left-arrow");
  const rightArrow = document.getElementById("right-arrow");
  const barChartCtx = document.getElementById("barChart").getContext("2d");
  let barChart;

  const goalsByPeriod = {};
  const actualDataByPeriod = {};
  const chartInstances = {};

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

      const goal = {
        name: topicSelect, // This should match the backend expectation
        period: parseInt(periodSelect),
        number: parseInt(dataInput),
      };

      addGoal(goal).then((newGoal) => {
        if (newGoal) {
          if (!goalsByPeriod[periodSelect]) {
            goalsByPeriod[periodSelect] = [];
          }

          goalsByPeriod[periodSelect].push(newGoal);

          // Fetch and update actual data
          fetchActualData(goal.period, goal.name).then((actualData) => {
            actualDataByPeriod[goal.period] = actualData;

            // Create or update the slide for the period
            updatePeriodSlides(goal.period);

            // Reset form and close wrapper
            document.getElementById("dataForm").reset();
            closeWrapper();

            // Update slides to show the new one
            currentIndex = slidesContainer.children.length - 1;
            updateSlidePosition();
          });
        }
      });
    });

  function initializeGoals() {
    fetchGoals().then((goals) => {
      goals.forEach((goal) => {
        const period = goal.period;
        if (!goalsByPeriod[period]) {
          goalsByPeriod[period] = [];
        }
        goalsByPeriod[period].push(goal);
        fetchActualData(goal.period, goal.name).then((actualData) => {
          actualDataByPeriod[goal.period] = actualData;
          updatePeriodSlides(period);
        });
      });
    });
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
      listItem.innerHTML = `Topic: ${goal.name}, Number: ${goal.number} <br> <button class="zoom-icon" onclick="zoomGoal('${period}', ${index})"><i class="fas fa-search-plus"></i></button>`;
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
          const label = chartInstances[period].data.labels[firstPoint.index];
          const datasetLabel =
            chartInstances[period].data.datasets[firstPoint.datasetIndex].label;
          if (datasetLabel === "Actual") {
            displayAreaChart(
              period,
              goalsByPeriod[period][firstPoint.index].name
            );
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
          deleteGoal(period).then((deleteSuccess) => {
            if (deleteSuccess) {
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
      });
  }

  // Zoom into a specific goal
  function zoomGoal(period, index) {
    const chart = chartInstances[period];
    const labels = chart.data.labels;
    const datasets = chart.data.datasets;

    chart.data.labels = [labels[index]];
    datasets.forEach((dataset) => {
      dataset.data = [dataset.data[index]];
    });

    chart.update();
  }

  // Get chart configuration
  function getChartConfig(period) {
    const periodGoals = goalsByPeriod[period];
    const labels = periodGoals.map((goal) => goal.name);
    const targetData = periodGoals.map((goal) => goal.number);
    const actualData = actualDataByPeriod[period].map((data) => data.Value); // Fetch actual data from backend

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

  // Update chart with original data
  function updateChart(canvas, period) {
    const chart = chartInstances[period];
    const periodGoals = goalsByPeriod[period];
    const labels = periodGoals.map((goal) => goal.name);
    const targetData = periodGoals.map((goal) => goal.number);
    const actualData = actualDataByPeriod[period].map((data) => data.Value); // Fetch actual data from backend

    chart.data.labels = labels;
    chart.data.datasets[0].data = targetData;
    chart.data.datasets[1].data = actualData;
    chart.update();
  }

  // Display area chart for a specific goal and period
  function displayAreaChart(period, topic) {
    // Fetch actual data for the topic and period
    fetchActualData(period, topic).then((data) => {
      const labels = data.map((item) => item.date);
      const actualData = data.map((item) => item.value);

      // Destroy existing chart if any
      if (barChart) {
        barChart.destroy();
      }

      barChart = new Chart(barChartCtx, {
        type: "line",
        data: {
          labels: labels,
          datasets: [
            {
              label: "Actual Data",
              data: actualData,
              fill: true,
              backgroundColor: "rgba(255, 206, 86, 0.2)",
              borderColor: "rgba(255, 206, 86, 1)",
              borderWidth: 1,
              pointBackgroundColor: "rgba(255, 206, 86, 1)",
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
      });

      // Scroll to the chart
      document
        .getElementById("barChart")
        .scrollIntoView({ behavior: "smooth" });
    });
  }

  // Update slide position
  function updateSlidePosition() {
    slidesContainer.style.transform = `translateX(-${currentIndex * 100}%)`;
    chartContainer.style.transform = `translateX(-${currentIndex * 100}%)`;
  }

  // Initialize goals on page load
  initializeGoals();

  // Event listeners for arrow buttons
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
});
