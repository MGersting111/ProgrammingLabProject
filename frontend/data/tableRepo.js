const baseUrl = 'http://localhost:5004/api/Models/';

const endpoint = 'SortedPagedProducts';
const page = 1;
const pageSize = 5;
const sortColumn = 'Category';
const sortOrder = 'desc';

const url = `${baseUrl}${endpoint}?page=${page}&pageSize=${pageSize}&sortColumn=${sortColumn}&sortOrder=${sortOrder}`;

  const modelAttributes = {
    customers: ["customerID", "latitude", "longitude"],
    stores: ["city", "state", "zipcode", "distance"],
    products: ["SKU", "Name", "Price", "Category", "Size", "Launch"],
    orders: ["orderId", "customerId", "storeId", "orderDate", "nItems", "total"]
};

// Funktion zum Aktualisieren der sortBy-Optionen
function updateSortByOptions() {
    const modelSelect = document.getElementById("modelSelect");
    const sortBySelect = document.getElementById("sortSelect");
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

document.addEventListener("DOMContentLoaded", () => {
    const modelSelect = document.getElementById("modelSelect");
    modelSelect.addEventListener("change", updateSortByOptions);

    // Initial load of options
    updateSortByOptions();
});

// Event Listener für die Änderung des Model-Dropdowns
document.addEventListener("DOMContentLoaded", () => {
    const modelSelect = document.getElementById("model");
    modelSelect.addEventListener("change", updateSortByOptions);

    // Initiales Laden der Optionen
    updateSortByOptions();
});



function getData(){
    var model = document.getElementById("modelSelect").value;
    var orderBy = document.getElementById("orderSelect").value;
    var sortBy = document.getElementById("sortSelect").value;
    const queryParams = {
        page: 1,
        pageSize: 5,
        sortColumn: orderBy,
        sortOrder: sortBy
    };
    if(model=='products'){
        urlTotalNumbers += "SortedPagedProducts"
    }

    fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        }
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
