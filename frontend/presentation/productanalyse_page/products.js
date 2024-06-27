const pieCharts = {
    typeChart: null,
    categoryChart: null,
  };
  
  function analyseProduct() {
    getData();
  }
  
  function getData() {
    // Hardcoded data
    const data = {
      productSalesBySize: {
        Small: 120,
        Medium: 80,
        Large: 50,
      },
      productSalesByCategory: {
        Special: 100,
        Veggie: 70,
        Classic: 80,
      },
    };
  
    console.log(data);
    updatePieCharts(data);
  }
  
  function updatePieCharts(data) {
    const productTypes = data.productSalesBySize;
    const productCategories = data.productSalesByCategory;
  
    if (pieCharts.typeChart) {
      pieCharts.typeChart.destroy();
    }
    if (pieCharts.categoryChart) {
      pieCharts.categoryChart.destroy();
    }
  
    const typeCtx = document.getElementById("typeChart").getContext("2d");
    pieCharts.typeChart = createPieChart(typeCtx, productTypes, "Product Types");
  
    const categoryCtx = document.getElementById("categoryChart").getContext("2d");
    pieCharts.categoryChart = createPieChart(
      categoryCtx,
      productCategories,
      "Product Categories"
    );
  }
  
  function createPieChart(ctx, data, title) {
    const total = Object.values(data).reduce((sum, value) => sum + value, 0);
  
    return new Chart(ctx, {
      type: "pie",
      data: {
        labels: Object.keys(data).map(
          (key) => `${key} (${data[key]} - ${(data[key] / total) * 100}%)`
        ),
        datasets: [
          {
            data: Object.values(data),
            backgroundColor: ["#FF6384", "#36A2EB", "#FFCE56", "#4BC0C0"],
          },
        ],
      },
      options: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          title: {
            display: true,
            text: title,
          },
        },
      },
    });
  }
  
  document.addEventListener("DOMContentLoaded", () => {
    document
      .getElementById("analyseButton")
      .addEventListener("click", analyseProduct);
  });
  