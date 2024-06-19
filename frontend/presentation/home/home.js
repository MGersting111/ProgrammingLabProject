import { fetchGoals, addGoal, updateGoal, deleteGoal } from './goalRepo.js';

let currentIndex = 0;
const slidesContainer = document.getElementById('slides');
const chartContainer = document.getElementById('chart-slides');
const leftArrow = document.getElementById('left-arrow');
const rightArrow = document.getElementById('right-arrow');
const lineChartCanvas = document.getElementById('lineChart');

const goalsByPeriod = {};
const chartInstances = {}; // To store chart instances
let lineChartInstance; // To store the line chart instance

document.getElementById('openModal').addEventListener('click', function() {
    document.getElementById('inputWrapper').classList.add('show'); // Show the wrapper
});

function closeWrapper() {
    document.getElementById('inputWrapper').classList.remove('show'); // Hide the wrapper
}

document.querySelector('.icon-close').addEventListener('click', closeWrapper);

document.getElementById('dataForm').addEventListener('submit', async function(event) {
    event.preventDefault(); // Prevent the form from submitting traditionally

    // Get the values from the form
    const topicSelect = document.getElementById('topicSelect').value;
    const periodSelect = document.getElementById('periodSelect').value;
    const dataInput = document.getElementById('dataInput').value;

    const goal = {
        name: topicSelect,
        period: parseInt(periodSelect),
        number: parseInt(dataInput)
    };

    const newGoal = await addGoal(goal);
    if (newGoal) {
        if (!goalsByPeriod[periodSelect]) {
            goalsByPeriod[periodSelect] = [];
        }

        goalsByPeriod[periodSelect].push(newGoal);

        // Create or update the slide for the period
        updatePeriodSlides(periodSelect);

        // Reset form and close wrapper
        document.getElementById('dataForm').reset();
        closeWrapper();

        // Update slides to show the new one
        currentIndex = slidesContainer.children.length - 1;
        updateSlidePosition();
    }
});

async function initializeGoals() {
    const goals = await fetchGoals();
    goals.forEach(goal => {
        const period = goal.period;
        if (!goalsByPeriod[period]) {
            goalsByPeriod[period] = [];
        }
        goalsByPeriod[period].push(goal);
        updatePeriodSlides(period);
    });
}

function updatePeriodSlides(period) {
    // Create or update the goal slide
    let goalSlide = document.querySelector(`.slide[data-period='${period}']`);
    if (!goalSlide) {
        goalSlide = document.createElement('div');
        goalSlide.classList.add('slide');
        goalSlide.setAttribute('data-period', period);
        goalSlide.innerHTML = `<h3>Period: ${period}</h3><ul></ul><button class="delete-button">Delete Period</button>`;
        slidesContainer.appendChild(goalSlide);
    }

    const goalList = goalSlide.querySelector('ul');
    goalList.innerHTML = ''; // Clear previous list
    goalsByPeriod[period].forEach((goal, index) => {
        const listItem = document.createElement('li');
        listItem.innerHTML = `Topic: ${goal.name}, Number: ${goal.number} <br> <button class="zoom-icon" onclick="zoomGoal('${period}', ${index})"><i class="fas fa-search-plus"></i></button>`;
        goalList.appendChild(listItem);
    });

    // Create or update the chart slide
    let chartSlide = document.querySelector(`.chart-slide[data-period='${period}']`);
    if (!chartSlide) {
        chartSlide = document.createElement('div');
        chartSlide.classList.add('slide', 'chart-slide');
        chartSlide.setAttribute('data-period', period);
        const chartCanvas = document.createElement('canvas');
        chartCanvas.style.width = '100%';
        chartCanvas.style.height = '250px'; // Adjust height to fit the box
        chartSlide.appendChild(chartCanvas);
        chartContainer.appendChild(chartSlide);

        // Create new chart instance
        chartInstances[period] = new Chart(chartCanvas, getChartConfig(period));
        chartCanvas.onclick = function(evt) {
            const activePoints = chartInstances[period].getElementsAtEventForMode(evt, 'nearest', { intersect: true }, false);
            if (activePoints.length) {
                const firstPoint = activePoints[0];
                const label = chartInstances[period].data.labels[firstPoint.index];
                const datasetLabel = chartInstances[period].data.datasets[firstPoint.datasetIndex].label;
                if (datasetLabel === 'Actual') {
                    displayAreaChart(period, goalsByPeriod[period][firstPoint.index].name);
                }
            }
        };
    } else {
        // Update existing chart instance
        const chartCanvas = chartSlide.querySelector('canvas');
        updateChart(chartCanvas, period);
    }

    // Add delete event listener for the period
    goalSlide.querySelector('.delete-button').addEventListener('click', async function() {
        if (confirm('Are you sure you want to delete all goals for this period?')) {
            const deleteSuccess = await deleteGoal(period);
            if (deleteSuccess) {
                delete goalsByPeriod[period];
                goalSlide.remove();
                chartSlide.remove();
                if (currentIndex >= slidesContainer.children.length) {
                    currentIndex = slidesContainer.children.length - 1;
                }
                updateSlidePosition();
            }
        }
    });
}

function zoomGoal(period, index) {
    const chart = chartInstances[period];
    const labels = chart.data.labels;
    const datasets = chart.data.datasets;

    chart.data.labels = [labels[index]];
    datasets.forEach(dataset => {
        dataset.data = [dataset.data[index]];
    });

    chart.update();
}

function getChartConfig(period) {
    const periodGoals = goalsByPeriod[period];
    const labels = periodGoals.map(goal => goal.name);
    const targetData = periodGoals.map(goal => goal.number);
    const actualData = periodGoals.map(goal => Math.floor(Math.random() * goal.number)); // Hardcoded actual value for demonstration

    return {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {
                    label: 'Goal',
                    data: targetData,
                    backgroundColor: '#007bff'
                },
                {
                    label: 'Actual',
                    data: actualData,
                    backgroundColor: '#ffc107'
                }
            ]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            },
            plugins: {
                zoom: {
                    zoom: {
                        wheel: {
                            enabled: false // Disable zooming with mouse wheel
                        },
                        pinch: {
                            enabled: true // Enable pinch zooming
                        },
                        mode: 'x'
                    }
                }
            }
        }
    };
}

function displayAreaChart(period, topic) {
    const weeklyData = {
        'periodical_revenue': generateWeeklyData(12),
        'amount_of_customers': generateWeeklyData(12),
        'net_profit': generateWeeklyData(12),
        'employee_productivity': generateWeeklyData(12),
        'operational_efficiency': generateWeeklyData(12)
    };

    const labels = generateWeeklyLabels(12); // 12 weeks for the first 3 months
    const data = weeklyData[topic];

    const config = {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: `${topic} Actual Data`,
                data: data,
                borderColor: 'yellow',
                backgroundColor: 'rgba(255, 255, 0, 0.3)', // Yellow fill
                fill: true
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    };

    if (lineChartInstance) {
        lineChartInstance.destroy();
    }

    lineChartInstance = new Chart(lineChartCanvas, config);

    // Ensure the line chart is visible
    lineChartCanvas.style.display = 'block';

    // Scroll to the line chart
    lineChartCanvas.scrollIntoView({ behavior: 'smooth' });
}

// Function to generate fake weekly data for a given number of weeks
function generateWeeklyData(weeks) {
    const data = [];
    for (let i = 0; i < weeks; i++) {
        data.push(Math.floor(Math.random() * 1000) + 100); // Generate random data
    }
    return data;
}

// Function to generate labels for a given number of weeks
function generateWeeklyLabels(weeks) {
    const labels = [];
    for (let i = 1; i <= weeks; i++) {
        labels.push(`Week ${i}`);
    }
    return labels;
}

// Initialize goals on page load
initializeGoals();
