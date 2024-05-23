function getData() {
    // Dummy data for demonstration
    const data = [
        { storeId: 1, city: 'New York', state: 'NY' },
        { storeId: 2, city: 'Los Angeles', state: 'CA' },
        { storeId: 3, city: 'Chicago', state: 'IL' },
        { storeId: 4, city: 'Houston', state: 'TX' },
        { storeId: 5, city: 'Phoenix', state: 'AZ' }
    ];

    const tableBody = document.getElementById('table-body');
    tableBody.innerHTML = ''; // Clear previous data

    data.forEach(row => {
        const tr = document.createElement('tr');
        tr.innerHTML = `<td>${row.storeId}</td><td>${row.city}</td><td>${row.state}</td>`;
        tableBody.appendChild(tr);
    });
}

// Call getData on page load
window.onload = getData;
