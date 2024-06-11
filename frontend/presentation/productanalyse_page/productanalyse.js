const productBaseUrl = "http://localhost:3000/analyseProduct";

function analyseProduct() {
  getData();
}
function getData() {
  const dateFrom = document.getElementById("fromDate").value;
  const dateTo = document.getElementById("toDate").value;
  url = `${productBaseUrl}?StartTime=${dateFrom}&EndTime=${dateTo}`;
  fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .then((data) => {
      console.log(data);
    })
    .catch((error) => {
      console.error("Fetch error:", error);
    });
}

function createBarChart() {}

function createMapChart() {}

function createLineChart() {}

function createDonutChart() {}
