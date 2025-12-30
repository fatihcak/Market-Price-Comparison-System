import { PriceResponseDTO, Product, ProductResponseDTO, ProductPriceHistoryDTO, Market, MarketResponseDTO } from '../types';

const API_BASE_URL = import.meta.env.PROD
    ? 'INSERT_AWS_API_URL_HERE'
    : 'http://localhost:5000/api';

export const api = {
    getMarkets: async (): Promise<Market[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Market`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            // Handle ApiResponse wrapper
            const data: MarketResponseDTO[] = result.data || result;
            return data.map(m => ({
                id: m.id,
                name: m.marketName,
                logoUrl: m.logoUrl
            }));
        } catch (error) {
            console.error('Error fetching markets:', error);
            return [];
        }
    },

    getPricesByProduct: async (productId: number): Promise<PriceResponseDTO[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Price/product/${productId}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            return result.data || result;
        } catch (error) {
            console.error('Error fetching prices:', error);
            return [];
        }
    },

    // Get prices for multiple product IDs (for consolidated products)
    getPricesByProductIds: async (productIds: number[]): Promise<PriceResponseDTO[]> => {
        try {
            const allPrices: PriceResponseDTO[] = [];

            // Fetch prices for all product IDs in parallel
            const results = await Promise.all(
                productIds.map(id =>
                    fetch(`${API_BASE_URL}/Price/product/${id}`)
                        .then(async res => {
                            if (!res.ok) return [];
                            const json = await res.json();
                            // Handle ApiResponse wrapper
                            if (json.success && Array.isArray(json.data)) {
                                return json.data;
                            }
                            // Fallback if data is directly returned (legacy) or invalid
                            return Array.isArray(json) ? json : [];
                        })
                        .catch(() => [])
                )
            );

            results.forEach(prices => {
                allPrices.push(...prices);
            });

            return allPrices;
        } catch (error) {
            console.error('Error fetching prices for multiple products:', error);
            return [];
        }
    },


    getProducts: async (): Promise<Product[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            // Handle ApiResponse wrapper
            const data: ProductResponseDTO[] = result.data || result;

            // Map Backend DTO to Frontend UI Model
            // Backend now returns canonical products with MarketCount
            const products = data.map(item => ({
                id: item.id,
                name: item.productName,
                price: item.price,
                oldPrice: item.oldPrice || null,
                market: item.marketName,
                discount: item.discount,
                category: item.categoryName,
                image: item.imageUrl || 'https://placehold.co/200x200?text=No+Image',
                brand: item.brand,
                unit: item.unit,
                marketCount: item.marketCount || 1,
                variantIds: [item.id]
            }));

            // Sort by marketCount descending so products with multiple markets appear first
            products.sort((a, b) => (b.marketCount || 1) - (a.marketCount || 1));

            // [ANTIGRAVITY] Grouping Logic for "Same Product Different IDs"
            const uniqueMap = new Map<string, Product>();

            for (const p of products) {
                // Create a unique key based on Brand, Name, and Unit
                // Use normalized values to avoid case/whitespace issues
                const brand = (p.brand || '').trim().toUpperCase();
                const name = (p.name || '').trim().toUpperCase();
                const unit = (p.unit || '').trim().toUpperCase();

                // If distinct products like "MARUL 1 ADET" are normalized to have empty name, 
                // the key relies on brand + unit. 
                const key = `${brand}|${name}|${unit}`;

                if (uniqueMap.has(key)) {
                    const existing = uniqueMap.get(key)!;

                    // Merge logic:
                    // 1. Keep the one with Lower Price as the "Main" display
                    if (p.price < existing.price) {
                        existing.price = p.price;
                        existing.oldPrice = p.oldPrice; // Assume old price follows current price
                        existing.market = p.market; // Update market to the cheaper one
                        existing.discount = p.discount;
                        // Keep image of the cheaper one or existing? existing usually fine.
                        existing.image = p.image || existing.image;
                    }

                    // 2. Accumulate Market Count
                    existing.marketCount = (existing.marketCount || 1) + (p.marketCount || 1);

                    // 3. Accumulate Variant IDs
                    if (p.variantIds) {
                        existing.variantIds = [...(existing.variantIds || []), ...p.variantIds];
                    }
                    if (!existing.variantIds?.includes(p.id)) {
                        existing.variantIds?.push(p.id);
                    }

                } else {
                    // Initialize variantIds if not present
                    if (!p.variantIds || p.variantIds.length === 0) {
                        p.variantIds = [p.id];
                    }
                    uniqueMap.set(key, p);
                }
            }

            return Array.from(uniqueMap.values());
        } catch (error) {
            console.error('Error fetching products:', error);
            return [];
        }
    },


    getProductHistory: async (productId: number, days: number = 30): Promise<ProductPriceHistoryDTO[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product/${productId}/history?days=${days}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            return result.data || result;
        } catch (error) {
            console.error('Error fetching product history:', error);
            return [];
        }
    },

    searchProducts: async (query: string): Promise<Product[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product/search?name=${encodeURIComponent(query)}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            const data: ProductResponseDTO[] = result.data || result;
            return data.map(item => ({
                id: item.id,
                name: item.productName,
                price: item.price,
                oldPrice: item.oldPrice || null,
                market: item.marketName,
                discount: item.discount,
                category: item.categoryName,
                image: item.imageUrl || 'https://placehold.co/200x200?text=No+Image',
                brand: item.brand,
                unit: item.unit
            }));
        } catch (error) {
            console.error('Error searching products:', error);
            return [];
        }
    },

    getProductsByCategory: async (categoryId: number): Promise<Product[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product/category/${categoryId}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            const data: ProductResponseDTO[] = result.data || result;
            return data.map(item => ({
                id: item.id,
                name: item.productName,
                price: item.price,
                oldPrice: item.oldPrice || null,
                market: item.marketName,
                discount: item.discount,
                category: item.categoryName,
                image: item.imageUrl || 'https://placehold.co/200x200?text=No+Image',
                brand: item.brand,
                unit: item.unit
            }));
        } catch (error) {
            console.error('Error fetching products by category:', error);
            return [];
        }
    },

    sendMessage: async (message: string): Promise<any> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Chat`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ message }),
            });
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            return result.data || result;
        } catch (error) {
            console.error('Error sending message:', error);
            throw error;
        }
    }
};
