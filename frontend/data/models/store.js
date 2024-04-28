class Store {
    constructor(storeID, zipcode, state_abbr, latitude, longitude, city, state, distance) {
        this.storeID = storeID;
        this.zipcode = zipcode;
        this.state_abbr = state_abbr;
        this.latitude = latitude;
        this.longitude = longitude;
        this.city = city;
        this.state = state;
        this.distance = distance;
    }

    static fromJSON(json) {
        if (!json || typeof json !== 'object' ||
            !json.hasOwnProperty('storeID') ||
            !json.hasOwnProperty('zipcode') ||
            !json.hasOwnProperty('state_abbr') ||
            !json.hasOwnProperty('latitude') ||
            !json.hasOwnProperty('longitude') ||
            !json.hasOwnProperty('city') ||
            !json.hasOwnProperty('state') ||
            !json.hasOwnProperty('distance')) {
            throw new Error('Invalid JSON data for Store');
        }

        return new Store(
            json.storeID,
            json.zipcode,
            json.state_abbr,
            json.latitude,
            json.longitude,
            json.city,
            json.state,
            json.distance
        );
    }
}