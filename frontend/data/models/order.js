class Order{
    constructor(orderID, customerID, storeID, orderDate, nItems, total) {
        this.orderID = orderID;
        this.customerID = customerID;
        this.storeID = storeID;
        this.orderDate = orderDate;
        this.nItems = nItems;
        this.total = total;
    }

    
    static fromJSON(json) {
        
        if (!json || typeof json !== 'object' ||
            !json.hasOwnProperty('orderID') ||
            !json.hasOwnProperty('customerID') ||
            !json.hasOwnProperty('storeID') ||
            !json.hasOwnProperty('orderDate') ||
            !json.hasOwnProperty('nItems') ||
            !json.hasOwnProperty('total')) {
            throw new Error('Invalid JSON data for Order');
        }

        return new Order(
            json.orderID,
            json.customerID,
            json.storeID,
            json.orderDate,
            json.nItems,
            json.total
        );
    }
}