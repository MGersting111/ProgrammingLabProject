const baseUrl = "http://localhost:5000/api/Goal"; // Adjust the base URL and port as needed

// Function to add a new goal
async function addGoal(goal) {
    try {
        const response = await fetch(`${baseUrl}`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(goal)
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();
        return result;
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}

// Function to fetch all goals (if needed)
async function fetchGoals() {
    try {
        const response = await fetch(`${baseUrl}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json"
            }
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const result = await response.json();
        return result;
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
    }
}
