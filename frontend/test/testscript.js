
const mainChartView = document.querySelector('.mainChartView');
const fetchButton = document.querySelector('.fetchDataButton');
const ctx = document.getElementById('myChart');
const urlTotalNumbers = 'http://localhost:3000/totalnumbers';
const canvaHeight = "100";

function postData() {
  // Daten aus den Dropdown-Menüs erhalten
  var store = document.getElementById("stores").value;
  var time = document.getElementById("fromDate").value;
  var time = document.getElementById("toDate").value;
  var category = document.getElementById("category").value;
  var orderBy = document.getElementById("orderBy").value;
  var sortBy = document.getElementById("sortBy").value;

  // Objekt mit den ausgewählten Daten erstellen
  var data = {
      store: store,
      time: time,
      category: category,
      orderBy: orderBy,
      sortBy: sortBy
  };
console.log(data);
  // POST-Anfrage senden
  fetch(urlTotalNumbers, {
      method: 'POST',
      headers: {
          'Content-Type': 'application/json'
      },
      body: JSON.stringify(data)
  })
  .then(response => {
      if (!response.ok) {
          throw new Error('Network response was not ok');
      }
      return response.json();
  })
  .then(data => {
      console.log('Success:', data);
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
    if(Chart.getChart(canvaCatOrd) != null){
      Chart.getChart(canvaCatOrd).destroy();
      Chart.getChart(canvaCatPro).destroy();
      Chart.getChart(canvaStoOrd).destroy();
    }
 

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
  .catch(error => {
      console.error('Error:', error);
  });

  const stores = data[1];
  console.log('Data!!:' + data[0].stores);
stores.forEach(store => {
  const storeName = store.storeName;
  const orders = store.orders;

  // Erstellen der Tabelle für den aktuellen Laden
  const table = document.createElement('table');
  table.classList.add('store-table');

  // Erstellen des Tabellenkopfs
  const thead = document.createElement('thead');
  const headerRow = document.createElement('tr');
  headerRow.innerHTML = `
    <th>Bestellnummer</th>
    <th>Datum</th>
    <th>Anzahl der Artikel</th>
    <th>Gesamtpreis</th>
  `;
  thead.appendChild(headerRow);
  table.appendChild(thead);

  // Hinzufügen der Bestellungen in die Tabelle
  const tbody = document.createElement('tbody');
  orders.forEach(order => {
    const row = document.createElement('tr');
    row.innerHTML = `
      <td>${order.orderID}</td>
      <td>${order.date}</td>
      <td>${order.nItems}</td>
      <td>${order.totalPrice}</td>
    `;
    tbody.appendChild(row);
  });
  table.appendChild(tbody);

  // Anzeigen der Tabelle im HTML-Dokument
  const storeContainer = document.getElementById('store-container');
  const storeTableContainer = document.createElement('div');
  storeTableContainer.classList.add('store-table-container');
  const storeTitle = document.createElement('h2');
  storeTitle.textContent = storeName;
  storeTableContainer.appendChild(storeTitle);
  storeTableContainer.appendChild(table);
  storeContainer.appendChild(storeTableContainer);
})};

