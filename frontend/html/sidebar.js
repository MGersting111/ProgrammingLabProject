document.addEventListener('DOMContentLoaded', function() {
    document.getElementById("sidebar").style.display = "none"; // Hide the sidebar initially
});

function toggleSidebar() {
    document.getElementById("sidebar").style.display === "none" ? 
        document.getElementById("sidebar").style.display = "block" :
        document.getElementById("sidebar").style.display = "none";
}