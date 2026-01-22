// City to Markets mapping for location-based filtering
// When a user selects a city, only products from these markets are shown

export const CITY_MARKETS: Record<string, string[]> = {
    'All Cities': [], // Empty array means show all markets
    'Girne': ['Starling Supermarket', 'Kıbrıs Sanal Market'],
    'Gazimağusa': ['Unimar Market', 'Sarper Market'],
    'Lefkoşa': ['Kıbrıs Sanal Market']
};

export const CITIES = Object.keys(CITY_MARKETS);

// Helper function to get markets for a city
export function getMarketsForCity(city: string): string[] {
    return CITY_MARKETS[city] || [];
}

// Helper function to check if a product is available in a city
export function isProductInCity(productMarkets: string[], city: string): boolean {
    if (city === 'All Cities' || !city) {
        return true; // Show all products when no city filter
    }
    const cityMarkets = CITY_MARKETS[city] || [];
    return productMarkets.some(market => cityMarkets.includes(market));
}

// Helper function to get the best market name for a product in a specific city
// If the product's current market is not in the city, return the first matching city market
export function getBestMarketForCity(productMarket: string, productAllMarkets: string[], city: string): string {
    if (city === 'All Cities' || !city) {
        return productMarket; // No city filter, return original market
    }

    const cityMarkets = CITY_MARKETS[city] || [];

    // If the current market is in the city, keep it
    if (cityMarkets.includes(productMarket)) {
        return productMarket;
    }

    // Otherwise, find the first matching market from the city
    const matchingMarket = productAllMarkets.find(m => cityMarkets.includes(m));
    return matchingMarket || productMarket;
}
