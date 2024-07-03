const productBaseUrl = "http://localhost:5004/api/ProductSales";
const pieChartContainer = document.querySelector("#pieChartContainer");
const lineChartContainer = document.querySelector(".lineChartsContainer");

let typeChart;
let categoryChart;
let totalRevenueChart;
let cumulativeSalesChart;
let radarChart;
let growthRateChart;

function analyseProduct() {
  getData();
}

function getData() {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);

  const url = `${productBaseUrl}?fromDate=${dateFrom.toISOString()}&toDate=${dateTo.toISOString()}`;
  console.log(url);

  fetch(url, {
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

    if (!data.totalSize || Object.keys(data.totalSize).length === 0) {
      console.error("totalSize data is missing or empty");
      return;
    }

    if (!data.totalCategory || Object.keys(data.totalCategory).length === 0) {
      console.error("totalCategory data is missing or empty");
      return;
    }

    const productSize = {
      Small: data.totalSize.Small || 0,
      Large: data.totalSize.Large || 0,
      Medium: data.totalSize.Medium || 0,
      "Extra Large": data.totalSize["Extra Large"] || 0
    };

    const productCategory = {
      Classic: data.totalCategory.Classic || 0,
      Vegetarian: data.totalCategory.Vegetarian || 0,
      Specialty: data.totalCategory.Specialty || 0
    };

    updatePieCharts(productSize, productCategory);
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updatePieCharts(productSize, productCategory) {
  if (typeChart) {
    typeChart.destroy();
  }

  if (categoryChart) {
    categoryChart.destroy();
  }

  const typeCtx = document.getElementById('typeChart').getContext('2d');
  typeChart = createPieChart(typeCtx, productSize, 'Product Sizes', [
    'rgba(173, 216, 230, 0.8)', // Light Blue for Small
    'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
    'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
    'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
  ]);

  const categoryCtx = document.getElementById('categoryChart').getContext('2d');
  categoryChart = createPieChart(categoryCtx, productCategory, 'Product Categories', [
    'rgba(0, 123, 255, 0.8)',   // Blue for Classic
    'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
    'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
  ]);
}

function createPieChart(ctx, data, title, colors) {
  const total = Object.values(data).reduce((sum, value) => sum + value, 0);
  const defaultColors = ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0', '#9966FF', '#FF9F40'];

  return new Chart(ctx, {
    type: 'pie',
    data: {
      labels: Object.keys(data).map(key => `${key} (${data[key]} - ${(data[key] / total * 100).toFixed(2)}%)`),
      datasets: [{
        data: Object.values(data),
        backgroundColor: colors || defaultColors,
      }]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: title
        }
      },
      onClick: (e, elements) => {
        if (elements.length > 0) {
          const clickedElementIndex = elements[0].index;
          const label = Object.keys(data)[clickedElementIndex];
          fetchLineChartData(label, title === 'Product Sizes' ? 'size' : 'category');
        }
      }
    }
  });
}

function fetchLineChartData(label, chartType) {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);

  const url = `${productBaseUrl}?fromDate=${dateFrom.toISOString()}&toDate=${dateTo.toISOString()}`;
  console.log(`Fetching line chart data from URL: ${url}`);

  fetch(url, {
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
    console.log("Fetched line chart data:", data);
    if (chartType === 'size') {
      updateLineCharts(data.productSalesBySize, label, 'Size', [
        'rgba(173, 216, 230, 0.8)', // Light Blue for Small
        'rgba(100, 149, 237, 0.8)', // Medium Blue for Medium
        'rgba(65, 105, 225, 0.8)',  // Royal Blue for Large
        'rgba(0, 0, 139, 0.8)'      // Dark Blue for Extra Large
      ]);
    } else if (chartType === 'category') {
      updateLineCharts(data.productSalesByCategory, label, 'Category', [
        'rgba(0, 123, 255, 0.8)',   // Blue for Classic
        'rgba(123, 0, 255, 0.8)',   // Purple for Vegetarian
        'rgba(64, 224, 208, 0.8)'   // Turquise for Specialty
      ]);
    }
    // Show additional buttons after rendering the line charts
    document.getElementById('radarChartButton').style.display = 'inline-block';
    document.getElementById('growthRateChartButton').style.display = 'inline-block';
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updateLineCharts(data, label, chartType, colors) {
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);
  const year = dateFrom.getFullYear();
  const months = [];

  // Generate month labels based on the selected date range
  let currentDate = new Date(dateFrom);
  while (currentDate <= dateTo) {
    months.push(currentDate.toLocaleString('default', { month: 'short' }));
    currentDate.setMonth(currentDate.getMonth() + 1);
  }

  const datasets = Object.keys(data).map((key, index) => {
    const metrics = data[key][year];
    const values = months.map((month, idx) => {
      const monthNumber = new Date(dateFrom.getFullYear(), idx).toLocaleString('default', { month: 'long' }).substring(0, 3);
      return metrics[monthNumber] || 0;
    });
    const cumulativeValues = values.reduce((acc, val) => {
      if (acc.length > 0) {
        acc.push(acc[acc.length - 1] + val);
      } else {
        acc.push(val);
      }
      return acc;
    }, []);

    return {
      label: key,
      data: values,
      borderColor: key === label ? 'rgba(255, 0, 0, 1)' : colors[index % colors.length],
      borderWidth: key === label ? 2 : 1,
      fill: false,
      pointBackgroundColor: key === label ? 'rgba(255, 0, 0, 1)' : colors[index % colors.length]
    };
  });

  const cumulativeDatasets = Object.keys(data).map((key, index) => {
    const metrics = data[key][year];
    const values = months.map((month, idx) => {
      const monthNumber = new Date(dateFrom.getFullYear(), idx).toLocaleString('default', { month: 'long' }).substring(0, 3);
      return metrics[monthNumber] || 0;
    });
    const cumulativeValues = values.reduce((acc, val) => {
      if (acc.length > 0) {
        acc.push(acc[acc.length - 1] + val);
      } else {
        acc.push(val);
      }
      return acc;
    }, []);

    return {
      label: key,
      data: cumulativeValues,
      borderColor: key === label ? 'rgba(255, 0, 0, 1)' : colors[index % colors.length],
      borderWidth: key === label ? 2 : 1,
      fill: false,
      pointBackgroundColor: key === label ? 'rgba(255, 0, 0, 1)' : colors[index % colors.length]
    };
  });

  const totalRevenueData = {
    labels: months,
    datasets: datasets
  };

  const cumulativeSalesData = {
    labels: months,
    datasets: cumulativeDatasets
  };

  const ctx1 = document.getElementById('totalRevenueChart').getContext('2d');
  const ctx2 = document.getElementById('cumulativeSalesChart').getContext('2d');

  if (totalRevenueChart) totalRevenueChart.destroy();
  if (cumulativeSalesChart) cumulativeSalesChart.destroy();

  totalRevenueChart = new Chart(ctx1, {
    type: 'line',
    data: totalRevenueData,
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        title: {
          display: true,
          text: `Monthly Sales for ${label} (${chartType})`
        }
      },
      scales: {
        x: {
          title: {
            display: true,
            text: 'Month'
          }
        },
        y: {
          title: {
            display: true,
            text: 'Sales'
          },
          beginAtZero: true
        }
      }
    }
  });

  cumulativeSalesChart = new Chart(ctx2, {
    type: 'line',
    data: cumulativeSalesData,
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        title: {
          display: true,
          text: `Cumulative Sales for ${label} (${chartType})`
        }
      },
      scales: {
        x: {
          title: {
            display: true,
            text: 'Month'
          }
        },
        y: {
          title: {
            display: true,
            text: 'Cumulative Sales'
          },
          beginAtZero: true
        }
      }
    }
  });
}

function showRadarChart(data) {
  const ctx = document.getElementById('radarChart').getContext('2d');

  const months = ['Jan', 'Feb', 'Mär', 'Apr', 'Mai', 'Jun'];
  const sizes = ['Small', 'Medium', 'Large', 'Extra Large'];
  const categories = ['Specialty', 'Classic', 'Vegetarian'];

  const sizeDatasets = sizes.map((size, index) => {
    const sizeData = data.productSalesBySize[size]['2022'];
    return {
      label: size,
      data: months.map(month => sizeData[month] || 0),
      backgroundColor: `rgba(${index * 50}, ${index * 80}, ${index * 150}, 0.4)`,
      borderColor: `rgba(${index * 50}, ${index * 80}, ${index * 150}, 0.8)`,
      fill: true
    };
  });

  const categoryDatasets = categories.map((category, index) => {
    const categoryData = data.productSalesByCategory[category]['2022'];
    return {
      label: category,
      data: months.map(month => categoryData[month] || 0),
      backgroundColor: `rgba(${index * 100}, ${index * 60}, ${index * 20}, 0.4)`,
      borderColor: `rgba(${index * 100}, ${index * 60}, ${index * 20}, 0.8)`,
      fill: true
    };
  });

  if (radarChart) {
    radarChart.destroy();
  }

  radarChart = new Chart(ctx, {
    type: 'radar',
    data: {
      labels: months,
      datasets: [...sizeDatasets, ...categoryDatasets]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: 'Sales Comparison Across Sizes and Categories'
        }
      }
    }
  });

  document.getElementById('radarChart').style.display = 'block';
}

function showGrowthRateChart(data) {
  const ctx = document.getElementById('growthRateChart').getContext('2d');

  const months = ['Jan', 'Feb', 'Mär', 'Apr', 'Mai', 'Jun'];
  const sizes = ['Small', 'Medium', 'Large', 'Extra Large'];
  const categories = ['Specialty', 'Classic', 'Vegetarian'];

  const calculateGrowthRate = (values) => {
    return values.map((value, index) => {
      if (index === 0) return 0;
      return ((value - values[index - 1]) / values[index - 1]) * 100;
    });
  };

  const sizeDatasets = sizes.map((size, index) => {
    const sizeData = data.productSalesBySize[size]['2022'];
    const values = months.map(month => sizeData[month] || 0);
    return {
      label: size,
      data: calculateGrowthRate(values),
      borderColor: `rgba(${index * 50}, ${index * 80}, ${index * 150}, 0.8)`,
      fill: false
    };
  });

  const categoryDatasets = categories.map((category, index) => {
    const categoryData = data.productSalesByCategory[category]['2022'];
    const values = months.map(month => categoryData[month] || 0);
    return {
      label: category,
      data: calculateGrowthRate(values),
      borderColor: `rgba(${index * 100}, ${index * 60}, ${index * 20}, 0.8)`,
      fill: false
    };
  });

  if (growthRateChart) {
    growthRateChart.destroy();
  }

  growthRateChart = new Chart(ctx, {
    type: 'line',
    data: {
      labels: months,
      datasets: [...sizeDatasets, ...categoryDatasets]
    },
    options: {
      responsive: true,
      maintainAspectRatio: false,
      plugins: {
        title: {
          display: true,
          text: 'Monthly Sales Growth Rate'
        }
      },
      scales: {
        y: {
          beginAtZero: true,
          title: {
            display: true,
            text: 'Growth Rate (%)'
          }
        }
      }
    }
  });

  document.getElementById('growthRateChart').style.display = 'block';
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('analyseButton').addEventListener('click', analyseProduct);
  document.getElementById('radarChartButton').addEventListener('click', () => showRadarChart(data));
  document.getElementById('growthRateChartButton').addEventListener('click', () => showGrowthRateChart(data));
});
