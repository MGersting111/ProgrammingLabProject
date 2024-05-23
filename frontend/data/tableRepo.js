var urlTotalNumbers = 'http://localhost:3000/';

  const modelAttributes = {
    customers: ["customerID", "latitude", "longitude"],
    stores: ["city", "state", "zipcode", "distance"],
    products: ["SKU", "Name", "Price", "Category", "Size", "Launch"],
    orders: ["orderId", "customerId", "storeId", "orderDate", "nItems", "total"]
};

// Funktion zum Aktualisieren der sortBy-Optionen
function updateSortByOptions() {
    const modelSelect = document.getElementById("model");
    const sortBySelect = document.getElementById("sortBy");
    const selectedModel = modelSelect.value;

    // Lösche alle aktuellen Optionen im sortBy-Select
    sortBySelect.innerHTML = "";

    // Hole die Attribute des ausgewählten Modells
    const attributes = modelAttributes[selectedModel] || [];

    // Erstelle neue Optionen für jedes Attribut
    attributes.forEach(attribute => {
        const option = document.createElement("option");
        option.value = attribute.toLowerCase();
        option.textContent = attribute;
        sortBySelect.appendChild(option);
    });
}

// Event Listener für die Änderung des Model-Dropdowns
document.addEventListener("DOMContentLoaded", () => {
    const modelSelect = document.getElementById("model");
    modelSelect.addEventListener("change", updateSortByOptions);

    // Initiales Laden der Optionen
    updateSortByOptions();
});



function getData(){
    var model = document.getElementById("categorySelect").value;
    var orderBy = document.getElementById("orderSelect").value;
    var sortBy = document.getElementById("sortSelect").value;
    var data = {
        orderBy: orderBy,
        sortBy: sortBy
    }

    console.log(data);
    fetch(urlTotalNumbers+model, {
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
        console.log('Success:', data[0]);
        const tableContainer = document.querySelector('.tableContainer');
        const tableHead = tableContainer.querySelector('thead');
        const tableBody = tableContainer.querySelector('tbody');
        tableContainer.style.display = "block";
        tableHead.innerHTML = '<tr id="header-row"></tr>';
        tableBody.innerHTML = '';
        if (data.length > 0) {
            // Header generieren
            const headerRow = tableHead.querySelector('#header-row');
            const firstItem = data[0];
            Object.keys(firstItem).forEach(key => {
                const th = document.createElement('th');
                th.textContent = key;
                headerRow.appendChild(th);
            });

            // Datenreihen generieren
            data.forEach(item => {
                const row = document.createElement('tr');
                Object.values(item).forEach(value => {
                    const cell = document.createElement('td');
                    cell.textContent = value;
                    row.appendChild(cell);
                });
                tableBody.appendChild(row);
            });
        }



    })

    

}
