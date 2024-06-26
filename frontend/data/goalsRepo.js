const goalApiBaseUrl = "http://localhost:5004/api/Goal";
const actualDataApiBaseUrl = "http://localhost:5004/api/TotalNumber/FilteredStoreInfo";
const salesApiBaseUrl = "http://localhost:5004/api/ProductSales"; // New endpoint for product sales

function postDataBasedOnInput(salesGoal, revenueGoal) {
    const topic = document.getElementById("topicSelect").value;
    const period = document.getElementById("periodSelect").value;
    const dataInput = document.getElementById("dataInput").value;

    const goal = {
        name: topic,
        period: parseInt(period),
        salesGoal: salesGoal !== undefined ? salesGoal : parseInt(dataInput),
        revenueGoal: revenueGoal !== undefined ? revenueGoal : parseFloat(dataInput)  // Use revenueGoal if available
    };

    console.log("Posting new goal:", goal);

    return fetch(goalApiBaseUrl, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(goal)
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.status} ${response.statusText}`);
        }
        return response.json();
    })
    .then(data => {
        console.log("Posted new goal:", data);
        return data;
    })
    .catch(error => {
        console.error("Post goal error:", error);
        throw error;
    });
}

function getPeriodDateRange(period) {
    switch (parseInt(period)) {
        case 1:
            return { fromDate: "2022-01-01", toDate: "2022-02-28" };
        case 2:
            return { fromDate: "2022-03-01", toDate: "2022-05-31" };
        case 3:
            return { fromDate: "2022-06-01", toDate: "2022-08-31" };
        case 4:
            return { fromDate: "2022-09-01", toDate: "2022-12-31" };
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
            "Content-Type": "application/json"
        }
    })
    .then(response => {
        if (!response.ok) {
            throw new Error(`Network response was not ok: ${response.status} ${response.statusText}`);
        }
        return response.json();
    })
    .then(data => {
        console.log("Fetched data based on input:", data);
        const actualRevenue = data.totalRevenue;
        const actualSales = data.totalSales;
        updateFrontend(dataInput, actualRevenue, actualSales, topic);  // Update frontend with the user input and actual data
        return { salesGoal: parseInt(dataInput), revenueGoal: parseFloat(dataInput) };
    })
    .catch(error => {
        console.error("Fetch data error:", error);
        throw error;
    });
}

function updateFrontend(userGoal, actualRevenue, actualSales, topic) {
  if (topic === "RevenueGoal") {
      document.getElementById("revenueGoal").innerText = `Goal: ${userGoal}`;
      document.getElementById("actualRevenue").innerText = `Actual Revenue: ${actualRevenue}`;
  } else if (topic === "SalesGoal") {
      document.getElementById("salesGoal").innerText = `Goal: ${userGoal}`;
      document.getElementById("actualSales").innerText = `Actual Sales: ${actualSales}`;
  }
}


document.getElementById("dataForm").addEventListener("submit", function(event) {
    event.preventDefault(); // Prevent the form from submitting traditionally

    fetchDataBasedOnInput()
        .then(({ salesGoal, revenueGoal }) => {
            return postDataBasedOnInput(salesGoal, revenueGoal);  // Pass salesGoal and revenueGoal to the post function
        })
        .then(() => {
            document.getElementById("dataForm").reset();
            closeWrapper();
        })
        .catch(error => {
            console.error("Error during form submission:", error);
            alert("Error: " + error.message); // Display error message
        });
});

function closeWrapper() {
    document.getElementById("inputWrapper").classList.remove("show"); // Hide the wrapper
}

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
            listItem.innerHTML = `Topic: ${goal.name}, Sales Goal: ${goal.salesGoal}, Revenue Goal: ${goal.revenueGoal} <br> <button class="zoom-icon" onclick="zoomGoal('${period}', ${index})"><i class="fas fa-search-plus"></i></button>`;
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
        const targetSalesData = periodGoals.map((goal) => goal.salesGoal);
        const targetRevenueData = periodGoals.map((goal) => goal.revenueGoal);
        const actualSalesData = actualDataByPeriod[period].map((data) => data.totalSales); // Fetch actual sales data from backend
        const actualRevenueData = actualDataByPeriod[period].map((data) => data.totalRevenue); // Fetch actual revenue data from backend

        const datasets = [];

        if (targetSalesData.length > 0) {
            datasets.push({
                label: "Sales Goal",
                data: targetSalesData,
                backgroundColor: "#007bff",
            });
        }
        if (actualSalesData.length > 0) {
            datasets.push({
                label: "Actual Sales",
                data: actualSalesData,
                backgroundColor: "#ffc107",
            });
        }
        if (targetRevenueData.length > 0) {
            datasets.push({
                label: "Revenue Goal",
                data: targetRevenueData,
                backgroundColor: "#28a745",
            });
        }
        if (actualRevenueData.length > 0) {
            datasets.push({
                label: "Actual Revenue",
                data: actualRevenueData,
                backgroundColor: "#dc3545",
            });
        }

        return {
            type: "bar",
            data: {
                labels: labels,
                datasets: datasets,
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
        const targetSalesData = periodGoals.map((goal) => goal.salesGoal);
        const targetRevenueData = periodGoals.map((goal) => goal.revenueGoal);
        const actualSalesData = actualDataByPeriod[period].map((data) => data.totalSales); // Fetch actual sales data from backend
        const actualRevenueData = actualDataByPeriod[period].map((data) => data.totalRevenue); // Fetch actual revenue data from backend

        chart.data.labels = labels;
        chart.data.datasets[0].data = targetSalesData;
        chart.data.datasets[1].data = actualSalesData;
        chart.data.datasets[2].data = targetRevenueData;
        chart.data.datasets[3].data = actualRevenueData;
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
