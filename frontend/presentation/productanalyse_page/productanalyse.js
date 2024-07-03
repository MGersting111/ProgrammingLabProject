const productBaseUrl = "http://localhost:5004/api/ProductSales";
const pieChartContainer = document.querySelector("#pieChartContainer");
const lineChartContainer = document.querySelector(".lineChartsContainer");

let typeChart;
let categoryChart;
let totalRevenueChart;
let cumulativeSalesChart;
let heatmapChart;

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

    updateCharts(productSize, productCategory);
  })
  .catch(error => {
    console.error("Fetch error:", error);
  });
}

function updateCharts(productSize, productCategory) {
  updatePieCharts(productSize, productCategory);
  updateHeatmap(productSize, productCategory);
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

function updateHeatmap(productSize, productCategory) {
  if (heatmapChart) {
    heatmapChart.destroy();
  }

  const ctx = document.getElementById('heatmapChart').getContext('2d');
  const dateFrom = new Date(document.getElementById("fromDate").value);
  const dateTo = new Date(document.getElementById("toDate").value);
  const months = [];

  // Generate month labels based on the selected date range
  let currentDate = new Date(dateFrom);
  while (currentDate <= dateTo) {
    months.push(currentDate.toLocaleString('default', { month: 'short' }));
    currentDate.setMonth(currentDate.getMonth() + 1);
  }

  const sizes = ['Small', 'Medium', 'Large', 'Extra Large'];
  const categories = ['Classic', 'Vegetarian', 'Specialty'];

  const heatmapData = {
    labels: sizes,
    datasets: categories.map((category, index) => {
      const data = sizes.map(size => {
        const metrics = productCategory[category] ? productCategory[category][dateFrom.getFullYear()] : {};
        return months.map((month, idx) => {
          const monthNumber = new Date(dateFrom.getFullYear(), idx).toLocaleString('default', { month: 'long' }).substring(0, 3);
          return metrics[monthNumber] || 0;
        });
      });

      return {
        label: category,
        data: data,
        backgroundColor: `rgba(${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, ${Math.floor(Math.random() * 255)}, 0.6)`,
        borderColor: 'rgba(255, 255, 255, 0.8)',
        borderWidth: 1
      };
    })
  };

  heatmapChart = new Chart(ctx, {
    type: 'heatmap',
    data: heatmapData,
    options: {
      responsive: true,
      maintainAspectRatio: true,
      plugins: {
        title: {
          display: true,
          text: 'Sales Volume Heatmap'
        },
        legend: {
          display: true,
          position: 'top'
        },
        tooltip: {
          callbacks: {
            label: function(context) {
              const value = context.raw;
              return `Sales: ${value}`;
            }
          }
        }
      },
      scales: {
        x: {
          title: {
            display: true,
            text: 'Sizes'
          },
          ticks: {
            autoSkip: false
          }
        },
        y: {
          title: {
            display: true,
            text: 'Categories'
          }
        }
      }
    }
  });
}

document.addEventListener('DOMContentLoaded', () => {
  document.getElementById('analyseButton').addEventListener('click', analyseProduct);
});
