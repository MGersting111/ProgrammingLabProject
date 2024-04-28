
const mainChartView = document.querySelector('.mainChartView');
const fetchButton = document.querySelector('.fetchDataButton');
const ctx = document.getElementById('myChart');
const urlOrders = 'http://localhost:3000/orders';
const urlTotalNumbers = 'http://localhost:3000/totalnumbers';
let totalRevenue = 0;
let totalOrders = 0;
let totalCustomers = 0;


fetchButton.addEventListener('click', () =>{
    fetch(urlTotalNumbers)
    .then(res => res.json())
    .then(data => {
        totalRevenue = data[0].totalRevenue;
        totalOrders = data[0].totalOrders;
        totalCustomers = data[0].totalCustomers;
        const ctx = document.getElementById('myChart');

        new Chart(ctx, {
          type: 'bar',
          data: {
            labels: ["Revenue", "Orders", "Customers"],
            datasets: [{
              label: 'Total',
              data: [totalRevenue, totalOrders, totalCustomers],
              borderWidth: 1
            }]
          },
          options: {
            scales: {
              y: {
                beginAtZero: true
              }
            }
          }
        });
    })





    fetch(urlOrders)
    .then(res => res.json())
    .then(data => {
        // Erstelle eine Tabelle
        const table = document.createElement('table');
        table.classList.add('table');
        
        // Erstelle die Tabellenüberschrift
        const thead = document.createElement('thead');
        const headerRow = document.createElement('tr');
        headerRow.innerHTML = `
            <th>Order ID</th>
            <th>Customer ID</th>
            <th>Store ID</th>
            <th>Order Date</th>
            <th>Number of Items</th>
            <th>Total<th>
        `;
        thead.appendChild(headerRow);
        table.appendChild(thead);

        // Erstelle die Tabellenzeilen für jedes Element
        const tbody = document.createElement('tbody');
        data.forEach(element => {
            const order = Order.fromJSON(element);
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${order.orderID}</td>
                <td>${order.customerID}</td>
                <td>${order.storeID}</td>
                <td>${order.orderDate}</td>
                <td>${order.nItems}</td>
                <td>${order.total}</td>
            `;
            tbody.appendChild(row);
        });
        table.appendChild(tbody);

        // Füge die Tabelle dem Hauptansichtscontainer hinzu
        mainChartView.appendChild(table);
    });

});
