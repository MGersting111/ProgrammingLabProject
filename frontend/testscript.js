
const mainChartView = document.querySelector('.mainChartView');
const fetchButton = document.querySelector('.fetchDataButton');
const ctx = document.getElementById('myChart');
const urlOrders = 'http://localhost:3000/orders';
const urlTotalNumbers = 'http://localhost:3000/totalnumbers';
const canvaHeight = "150";


fetchButton.addEventListener('click', () =>{
    fetch(urlTotalNumbers)
    .then(res => res.json())
    .then(data => {
      const totalNumbers = data[0]; // Alle Daten in einer Variable speichern

      // Zugriff auf die einzelnen Kennzahlen
      const totalRevenue = totalNumbers.Gesamtumsatz;
      const totalOrders = totalNumbers.Gesamtanzahl_der_Bestellungen;
      const totalCustomers = totalNumbers.Anzahl_der_Kunden;

      // Zugriff auf die Kategoriedaten
      const categoryOrders = totalNumbers.Anzahl_der_Bestellungen_pro_Kategorie;
      const categoryProducts = totalNumbers.Anzahl_der_Produkte_pro_Kategorie;
      const storeOrders = totalNumbers.Anzahl_der_Bestellungen_pro_Laden;
      const canvaCatOrd = document.getElementById('categoryOrders');
      const canvaCatPro = document.getElementById('categoryProducts');
      const canvaStoOrd = document.getElementById('storeOrders');
     const chartDiv = document.getElementById("chartDiv");
     chartDiv.style.display = "block";

        new Chart(canvaCatOrd, {
          type: 'bar',
          data: {
            labels: ["Elektronik", "Kleidung", "Haushalt", "Lebensmittel"],
            datasets: [{
              label: 'Anzahl der Bestellungen Kategorie',
              data: categoryOrders,
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

        new Chart(canvaCatPro, {
          type: 'bar',
          data: {
            labels: ["Elektronik", "Kleidung", "Haushalt", "Lebensmittel"],
            datasets: [{
              label: 'Anzahl der Produkte pro Kategorien',
              data: categoryProducts,
              borderWidth: 1
            }]
          },
          options: {
            scales: {
              y: {
                beginAtZero: true
              }
            },
          }
        });

        new Chart(canvaStoOrd, {
          type: 'bar',
          data: {
            labels: ["Store1", "Store2", "Store3", "Store4"],
            datasets: [{
              label: 'Anzahl der Bestellungen pro Laden',
              data: storeOrders,
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




//fetch list of data and create a table with it
/*
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
*/
});
