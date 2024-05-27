const baseUrl = 'http://localhost:5004/api/TotalNumber/FilteredStoreInfo';
const totalNumbersDiv = document.querySelector('.totalNumbers');
var storeData = new StoreData().storeData;
var StoreId;
var OrderDateFrom;
var OrderDateTo;

function getData(){
    var stores = document.getElementById("stores").value;
    OrderDateFrom = document.getElementById("fromDate").value;
    OrderDateTo = document.getElementById("toDate").value;
    var category = document.getElementById("category").value;
    StoreId = getKeysByValues(storeData, stores);
    const storeValues = [];
    console.log($('#stores').select2('data'));
    // Iterate through the selected stores
    for (const selectedStore of selectedStores) {
      // Check if the selected store exists in the storeData
      if (storeData.hasOwnProperty(selectedStore)) {
        // Retrieve the value (state-city pair) from storeData
        const storeValue = storeData[selectedStore];
  
        // Add the value to the storeValues list
        storeValues.push(storeValue);
      }
    }
    console.log(storeValues);
    const url = `${baseUrl}?StoreId=${StoreId}&OrderDateFrom=${OrderDateFrom}&OrderDateTo=${OrderDateTo}`;

    fetch(url, {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    })
    .then(response => {
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return response.json();
    })
    .then(data => {
        console.log('Success:', data[0]);
        data = data[0];
        const dataContainer = document.querySelector('.totalNumbers');
        dataContainer.style.display = "block";
        const sales = document.querySelector('.sales');
        const revenue = document.querySelector('.revenue');
        const customers = document.querySelector('.customers');
        const arpc = document.querySelector('.arpc');
    sales.textContent = data.Sales;
    revenue.textContent = data.Revenue;
    customers.textContent = data.Customers;
    arpc.textContent = data.RevenuePerCustomer;


    })    
}

function getKeysByValues(obj, values) {
    let keys = [];
    for (const value of values) {
      // Use Object.entries to iterate over key-value pairs
      for (const [key, objValue] of Object.entries(obj)) {
        if (objValue === value) {
          keys.push(key);
        }
      }
    }
    return keys;
  }

function getKeyByValue(obj, searchValue) {
    for (let [key, value] of Object.entries(obj)) {
        if (value === searchValue) {
            return key;
        }
    }
    return null; // Wenn der Wert nicht gefunden wurde
}
