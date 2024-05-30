document.getElementById('openModal').addEventListener('click', function() {
    document.getElementById('inputWrapper').style.display = 'flex'; // Show the wrapper
});

function closeWrapper() {
    document.getElementById('inputWrapper').style.display = 'none'; // Hide the wrapper
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

    // Create a new list item
    var newGoal = document.createElement('li');
    newGoal.textContent = `Topic: ${topicSelect}, Period: ${periodSelect}, Number: ${dataInput}`;

    // Append the new goal to the goals list
    document.getElementById('goals-list').appendChild(newGoal);

    // Optionally clear the form fields
    document.getElementById('dataForm').reset();

    // Hide the wrapper
    closeWrapper();
});

