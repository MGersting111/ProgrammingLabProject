const urlTotalNumbers = 'http://localhost:3000/totalnumbers';
const totalNumbersDiv = document.querySelector('.totalNumbers')

function getData(){
    var store = document.getElementById("stores").value;
    var time = document.getElementById("fromDate").value;
    var time = document.getElementById("toDate").value;
    var category = document.getElementById("category").value;
    var data = {
        store: store,
      time: time,
      category: category,
    }

    console.log(data);

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