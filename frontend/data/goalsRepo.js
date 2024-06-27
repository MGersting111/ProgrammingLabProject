const goalApiBaseUrl = "http://localhost:5004/api/Goal";
const actualDataApiBaseUrl = "http://localhost:5004/api/TotalNumber/FilteredStoreInfo";
const salesApiBaseUrl = "http://localhost:5004/api/ProductSales"; // New endpoint for product sales
const barChartContainer = document.querySelector("#barChart");
let barChart;
let revenueGoal = null;
let salesGoal = null;
let actualRevenue = null;
let actualSales = null;

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
        actualRevenue = data.productRevenue["2022"].TotalRevenue;
        actualSales = data.productSalesByMonth["2022"].Total;
        updateFrontend(dataInput, actualRevenue, actualSales, topic);  // Update frontend with the user input and actual data
        return { salesGoal: parseInt(dataInput), revenueGoal: parseFloat(dataInput) };
    })
    .catch(error => {
        console.error("Fetch data error:", error);
        throw error;
    });
}

function updateFrontend(userGoal, fetchedActualRevenue, fetchedActualSales, topic) {
    const period = document.getElementById("periodSelect").value;
    if (topic === "RevenueGoal") {
        revenueGoal = parseFloat(userGoal);
        actualRevenue = fetchedActualRevenue;
        document.getElementById("revenueGoal").innerText = `Goal: ${userGoal}`;
        document.getElementById("actualRevenue").innerText = `Actual Revenue: ${actualRevenue}`;
    } else if (topic === "SalesGoal") {
        salesGoal = parseInt(userGoal);
        actualSales = fetchedActualSales;
        document.getElementById("salesGoal").innerText = `Goal: ${userGoal}`;
        document.getElementById("actualSales").innerText = `Actual Sales: ${actualSales}`;
    }
    createBarChart(period); // Update the chart
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
});

// Funktion zum Erstellen des Bar-Charts
function createBarChart(period) {
    const ctx = document.querySelector("#barChart").getContext('2d');
    const labels = [];
    const goalData = [];
    const actualData = [];

    if (revenueGoal !== null && actualRevenue !== null) {
        labels.push('Revenue');
        goalData.push(revenueGoal);
        actualData.push(actualRevenue);
    }

    if (salesGoal !== null && actualSales !== null) {
        labels.push('Sales');
        goalData.push(salesGoal);
        actualData.push(actualSales);
    }

    const data = {
        labels: labels,
        datasets: [
            {
                label: 'Goal',
                data: goalData,
                backgroundColor: 'rgba(75, 192, 192, 1)',
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1
            },
            {
                label: 'Actual',
                data: actualData,
                backgroundColor: 'rgba(153, 102, 255, 1)',
                borderColor: 'rgba(153, 102, 255, 1)',
                borderWidth: 1
            }
        ]
    };
    const options = {
        responsive: true,
        scales: {
            y: {
                beginAtZero: true
            }
        },
        plugins: {
            title: {
                display: true,
                text: `Period ${period}`
            },
            zoom: {
                pan: {
                    enabled: true,
                    mode: 'x',
                },
                zoom: {
                    wheel: {
                        enabled: true,
                    },
                    pinch: {
                        enabled: true
                    },
                    mode: 'x',
                }
            }
        }
    };

    if (barChart) {
        barChart.destroy();
    }
    barChart = new Chart(ctx, {
        type: 'bar',
        data: data,
        options: options
    });
}

// Zoom-Funktion für Revenue
document.getElementById("zoomRevenueButton").addEventListener("click", function() {
    barChart.zoom(1.5);
    barChart.update();
});

// Zoom-Funktion für Sales
document.getElementById("zoomSalesButton").addEventListener("click", function() {
    barChart.zoom(1.5);
    barChart.update();
});

// Reset Zoom-Funktion
document.getElementById("resetZoomButton").addEventListener("click", function() {
    barChart.resetZoom();
});
