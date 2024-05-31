const baseUrl = "http://localhost:5004/api/TotalNumber/FilteredStoreInfo";
const totalNumbersDiv = document.querySelector(".totalNumbers");
var storeData = new StoreData().storeData;
var StoreId;
var OrderDateFrom;
var OrderDateTo;

function getData() {
  var stores = document.getElementById("stores");
  OrderDateFrom = document.getElementById("fromDate").value;
  OrderDateTo = document.getElementById("toDate").value;
  var category = document.getElementById("category").value;

  const selectedOptions = Array.from(stores.selectedOptions);
  const selectedValues = selectedOptions.map((option) => option.value);
  const selectedStoreId = selectedValues.join(",");

  const url = `${baseUrl}?StoreId=${selectedStoreId}&OrderDateFrom=${OrderDateFrom}&OrderDateTo=${OrderDateTo}`;

  fetch(url, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
    },
  })
    .then((response) => {
      if (!response.ok) {
        throw new Error("Network response was not ok");
      }
      return response.json();
    })
    .then((data) => {
      const tableBody = document
        .getElementById("totalNumbersTable")
        .getElementsByTagName("tbody")[0];
      data.forEach((item) => {
        const row = document.createElement("tr");

        const storeIdCell = document.createElement("td");
        storeIdCell.textContent = item.storeId;
        row.appendChild(storeIdCell);

        const orderCountCell = document.createElement("td");
        orderCountCell.textContent = item.orderCount;
        row.appendChild(orderCountCell);

        const totalRevenueCell = document.createElement("td");
        totalRevenueCell.textContent = item.totalRevenue.toFixed(2);
        row.appendChild(totalRevenueCell);

        const customerCountCell = document.createElement("td");
        customerCountCell.textContent = item.customerCount;
        row.appendChild(customerCountCell);

        const revenuePerCustomerCell = document.createElement("td");
        revenuePerCustomerCell.textContent = item.revenuePerCustomer.toFixed(2);
        row.appendChild(revenuePerCustomerCell);

        tableBody.appendChild(row);
      });
    });
}
