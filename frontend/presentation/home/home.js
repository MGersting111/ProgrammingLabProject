let currentIndex = 0;
const slidesContainer = document.getElementById('slides');
const chartContainer = document.getElementById('chart-slides');
const leftArrow = document.getElementById('left-arrow');
const rightArrow = document.getElementById('right-arrow');

document.getElementById('openModal').addEventListener('click', function() {
    document.getElementById('inputWrapper').classList.add('show'); // Show the wrapper
});

function closeWrapper() {
    document.getElementById('inputWrapper').classList.remove('show'); // Hide the wrapper
}

// Optional: Close when clicking outside the form box
window.addEventListener('click', function(event) {
    let formBox = document.querySelector('.form-box');
    if (event.target == document.getElementById('inputWrapper') && !formBox.contains(event.target)) {
        closeWrapper();
    }
});

document.getElementById('dataForm').addEventListener('submit', function(event) {
    event.preventDefault(); // Prevent the form from submitting traditionally

    // Get the values from the form
    var topicSelect = document.getElementById('topicSelect').value;
    var periodSelect = document.getElementById('periodSelect').value;
    var dataInput = document.getElementById('dataInput').value;

    // Create a new slide for the goal
    var newSlide = document.createElement('div');
    newSlide.classList.add('slide');
    newSlide.innerHTML = `
        <h3>Topic: ${topicSelect}</h3>
        <p>Period: ${periodSelect}</p>
        <p>Number: ${dataInput}</p>
        <button class="delete-button">Delete</button>
    `;
    slidesContainer.appendChild(newSlide);

    // Create a new slide for the chart
    var newChartSlide = document.createElement('div');
    newChartSlide.classList.add('slide');
    var chartCanvas = document.createElement('canvas');
    newChartSlide.appendChild(chartCanvas);
    chartContainer.appendChild(newChartSlide);
    createChart(chartCanvas, topicSelect, dataInput);

    // Reset form and close wrapper
    document.getElementById('dataForm').reset();
    closeWrapper();

    // Update slides to show the new one
    currentIndex = slidesContainer.children.length - 1;
    updateSlidePosition();

    // Add event listener for delete button
    newSlide.querySelector('.delete-button').addEventListener('click', function() {
        if (confirm('Are you sure you want to delete the goal?')) {
            newSlide.remove();
            newChartSlide.remove();
            if (currentIndex >= slidesContainer.children.length) {
                currentIndex = slidesContainer.children.length - 1;
            }
            updateSlidePosition();
        }
    });
});

leftArrow.addEventListener('click', function() {
    if (currentIndex > 0) {
        currentIndex--;
        updateSlidePosition();
    }
});

rightArrow.addEventListener('click', function() {
    if (currentIndex < slidesContainer.children.length - 1) {
        currentIndex++;
        updateSlidePosition();
    }
});

function updateSlidePosition() {
    slidesContainer.style.transform = `translateX(-${currentIndex * 100}%)`;
    chartContainer.style.transform = `translateX(-${currentIndex * 100}%)`;
}

function createChart(canvas, topic, target) {
    const actual = Math.floor(Math.random() * target); // Hardcoded actual value for demonstration
    new Chart(canvas, {
        type: 'pie',
        data: {
            labels: ['Target', 'Actual'],
            datasets: [{
                data: [target, actual],
                backgroundColor: ['#007bff', '#ffc107']
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'top'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.label || '';
                            if (label) {
                                label += ': ';
                            }
                            label += context.raw;
                            return label;
                        }
                    }
                }
            }
        }
    });
}
