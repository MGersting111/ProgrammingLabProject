class Customer {
    constructor(customerID, latitude, longitude) {
        this.customerID = customerID;
        this.latitude = latitude;
        this.longitude = longitude;
    }

    static fromJSON(json) {
        if (!json || typeof json !== 'object' ||
            !json.hasOwnProperty('customerID') ||
            !json.hasOwnProperty('latitude') ||
            !json.hasOwnProperty('longitude')) {
            throw new Error('Invalid JSON data for Customer');
        }

        return new Customer(
            json.customerID,
            json.latitude,
            json.longitude
        );
    }
}