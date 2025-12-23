import { PriceResponseDTO, Product, ProductResponseDTO, ProductPriceHistoryDTO } from '../types';

const API_BASE_URL = import.meta.env.PROD
    ? 'https://marketcomparisonsystem.azurewebsites.net/api'
    : 'http://localhost:5000/api';

export const api = {
    getPricesByProduct: async (productId: number): Promise<PriceResponseDTO[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Price/product/${productId}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching prices:', error);
            return [];
        }
    },

    // Get prices for multiple product IDs (for consolidated products)
    getPricesByProductIds: async (productIds: number[]): Promise<PriceResponseDTO[]> => {
        try {
            console.log('[DEBUG] Fetching prices for productIds:', productIds);
            const allPrices: PriceResponseDTO[] = [];

            // Fetch prices for all product IDs in parallel
            const results = await Promise.all(
                productIds.map(id =>
                    fetch(`${API_BASE_URL}/Price/product/${id}`)
                        .then(res => res.ok ? res.json() : [])
                        .catch(() => [])
                )
            );

            results.forEach((prices, index) => {
                console.log(`[DEBUG] Prices for productId ${productIds[index]}:`, prices);
                allPrices.push(...prices);
            });
            console.log('[DEBUG] All prices combined:', allPrices);
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
            const data: ProductResponseDTO[] = await response.json();

            // Map Backend DTO to Frontend UI Model
            const allProducts = data.map(item => ({
                id: item.id,
                name: item.productName, // Map productName to name
                price: item.price,
                oldPrice: item.oldPrice || null,
                market: item.marketName, // Map marketName to market
                discount: item.discount,
                category: item.categoryName, // Map categoryName to category
                image: item.imageUrl || 'https://placehold.co/200x200?text=No+Image', // Fallback image
                brand: item.brand,
                unit: item.unit
            }));
            // ==========================================
            // SIMPLE EXACT MATCH GROUPING
            // Groups only products with identical normalized brand+name+size
            // ==========================================

            // Normalize text for grouping key
            const normalizeText = (text?: string): string => {
                if (!text) return '';
                return text
                    .toLowerCase()
                    .replace(/\s+/g, '')  // Remove all spaces
                    .replace(/[^a-z0-9ğüşıöç]/g, ''); // Keep only letters and numbers
            };

            // Extract size/quantity from text for grouping key
            const extractSizeKey = (text: string): string => {
                const lower = text.toLowerCase();

                // Match patterns like "400gr", "1kg", "200ml", "1.5lt", "1adet"
                const match = lower.match(/(\d+(?:[.,]\d+)?)\s*(kg|gr|gram|g|lt|ml|litre|liter|cl|adet|tane|paket)\b/);
                if (match) {
                    const value = parseFloat(match[1].replace(',', '.'));
                    const unit = match[2];

                    // Normalize to base unit
                    if (unit === 'kg') return `${value * 1000}g`;
                    if (unit === 'gr' || unit === 'gram' || unit === 'g') return `${value}g`;
                    if (unit === 'lt' || unit === 'litre' || unit === 'liter') return `${value * 1000}ml`;
                    if (unit === 'ml') return `${value}ml`;
                    if (unit === 'cl') return `${value * 10}ml`;
                    return `${value}${unit}`;
                }
                return '';
            };

            // Create a unique grouping key for a product
            const getGroupKey = (product: Product): string => {
                const fullText = `${product.brand || ''} ${product.name || ''} ${product.unit || ''}`;
                const brand = normalizeText(product.brand);
                const name = normalizeText(product.name);
                const size = extractSizeKey(fullText);

                // Key format: brand_name_size
                return `${brand}_${name}_${size}`;
            };

            // Group products by exact key match
            const productGroups = new Map<string, Product[]>();

            allProducts.forEach(product => {
                const key = getGroupKey(product);
                const existing = productGroups.get(key) || [];
                productGroups.set(key, [...existing, product]);
            });

            // For each group, return the product with the lowest price
            const consolidatedProducts: Product[] = [];

            productGroups.forEach((variants, key) => {
                // Sort by price to get the cheapest one
                variants.sort((a, b) => a.price - b.price);
                const cheapest = { ...variants[0] };

                // Store all variant IDs so we can fetch prices for all of them
                (cheapest as any).variantIds = variants.map(v => v.id);

                // Count UNIQUE markets, not total products
                const uniqueMarkets = new Set(variants.map(v => v.market));
                (cheapest as any).marketCount = uniqueMarkets.size;

                // Debug log for grouped products with multiple unique markets
                if (uniqueMarkets.size > 1) {
                    console.log(`[DEBUG] Grouped ${variants.length} products from ${uniqueMarkets.size} UNIQUE markets with key "${key}":`);
                    variants.forEach(v => console.log(`  - ID: ${v.id}, Market: ${v.market}, Name: ${v.name}`));
                }

                consolidatedProducts.push(cheapest);
            });

            return consolidatedProducts;
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
            return await response.json();
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
            const data: ProductResponseDTO[] = await response.json();
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
            const data: ProductResponseDTO[] = await response.json();
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
            return await response.json();
        } catch (error) {
            console.error('Error sending message:', error);
            throw error;
        }
    }
};
