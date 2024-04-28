class OrderItem {
    constructor(SKU, orderID) {
        this.SKU = SKU;
        this.orderID = orderID;
    }

    static fromJSON(json) {
        if (!json || typeof json !== 'object' ||
            !json.hasOwnProperty('SKU') ||
            !json.hasOwnProperty('orderID')) {
            throw new Error('Invalid JSON data for OrderItem');
        }

        return new OrderItem(
            json.SKU,
            json.orderID
        );
    }
}