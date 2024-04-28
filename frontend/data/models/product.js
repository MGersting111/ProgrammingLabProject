class Product {
    constructor(SKU, Name, Price, Category, Size, Ingredients, Launch) {
        this.SKU = SKU;
        this.Name = Name;
        this.Price = Price;
        this.Category = Category;
        this.Size = Size;
        this.Ingredients = Ingredients;
        this.Launch = Launch;
    }

    static fromJSON(json) {
        if (!json || typeof json !== 'object' ||
            !json.hasOwnProperty('SKU') ||
            !json.hasOwnProperty('Name') ||
            !json.hasOwnProperty('Price') ||
            !json.hasOwnProperty('Category') ||
            !json.hasOwnProperty('Size') ||
            !json.hasOwnProperty('Ingredients') ||
            !json.hasOwnProperty('Launch')) {
            throw new Error('Invalid JSON data for Product');
        }

        return new Product(
            json.SKU,
            json.Name,
            json.Price,
            json.Category,
            json.Size,
            json.Ingredients,
            json.Launch
        );
    }
}