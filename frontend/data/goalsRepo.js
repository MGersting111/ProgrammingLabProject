const goalApiBaseUrl = "http://localhost:5004/api/Goal";

// Fetch all goals
async function fetchGoals() {
    try {
        const response = await fetch(goalApiBaseUrl);
        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const goals = await response.json();
        return goals;
    } catch (error) {
        console.error("Fetch error:", error);
        return [];
    }
}

// Add a new goal
async function addGoal(goal) {
    try {
        const response = await fetch(goalApiBaseUrl, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(goal)
        });
        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const newGoal = await response.json();
        return newGoal;
    } catch (error) {
        console.error("Add goal error:", error);
        return null;
    }
}

// Update an existing goal
async function updateGoal(goalId, updatedGoal) {
    try {
        const response = await fetch(`${goalApiBaseUrl}/${goalId}`, {
            method: "PUT",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(updatedGoal)
        });
        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        const updatedData = await response.json();
        return updatedData;
    } catch (error) {
        console.error("Update goal error:", error);
        return null;
    }
}

// Delete a goal
async function deleteGoal(goalId) {
    try {
        const response = await fetch(`${goalApiBaseUrl}/${goalId}`, {
            method: "DELETE"
        });
        if (!response.ok) {
            throw new Error("Network response was not ok");
        }
        return true;
    } catch (error) {
        console.error("Delete goal error:", error);
        return false;
    }
}

export { fetchGoals, addGoal, updateGoal, deleteGoal };
