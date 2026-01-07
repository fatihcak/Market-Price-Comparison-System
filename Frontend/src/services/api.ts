import { PriceResponseDTO, Product, ProductResponseDTO, ProductPriceHistoryDTO, Market, MarketResponseDTO, CategoryResponseDTO } from '../types';

const API_BASE_URL = import.meta.env.DEV
    ? 'http://localhost:5000/api'
    : 'https://compare-market.site/api';
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
    getCategories: async (): Promise<CategoryResponseDTO[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Category`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const result = await response.json();
            return result.data || result;
        } catch (error) {
            console.error('Error fetching categories:', error);
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


    getProducts: async (page: number = 1, pageSize: number = 50): Promise<{ products: Product[]; totalCount: number }> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product?page=${page}&pageSize=${pageSize}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const totalCount = parseInt(response.headers.get('X-Total-Count') || '0', 10);
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

            return { products: Array.from(uniqueMap.values()), totalCount };
        } catch (error) {
            console.error('Error fetching products:', error);
            return { products: [], totalCount: 0 };
        }
    },

    getProductsByDiscount: async (page: number = 1, pageSize: number = 20): Promise<{ products: Product[]; totalCount: number }> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product/by-discount?page=${page}&pageSize=${pageSize}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const totalCount = parseInt(response.headers.get('X-Total-Count') || '0', 10);
            const result = await response.json();
            const data: ProductResponseDTO[] = result.data || result;

            const products: Product[] = data.map(item => ({
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

            return { products, totalCount };
        } catch (error) {
            console.error('Error fetching discounted products:', error);
            return { products: [], totalCount: 0 };
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

    searchProducts: async (query: string, page: number = 1, pageSize: number = 50): Promise<{ products: Product[]; totalCount: number }> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Product/search?name=${encodeURIComponent(query)}&page=${page}&pageSize=${pageSize}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const totalCount = parseInt(response.headers.get('X-Total-Count') || '0', 10);
            const result = await response.json();
            const data: ProductResponseDTO[] = result.data || result;
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
            return { products, totalCount };
        } catch (error) {
            console.error('Error searching products:', error);
            return { products: [], totalCount: 0 };
        }
    },

    getProductsByCategory: async (categoryId: number, page: number = 1, pageSize: number = 50): Promise<{ products: Product[]; totalCount: number }> => {
        try {
            console.log(`[API] Fetching category ${categoryId} page ${page}`);
            const response = await fetch(`${API_BASE_URL}/Product/category/${categoryId}?page=${page}&pageSize=${pageSize}`);
            if (!response.ok) {
                console.error(`[API] Category fetch failed: ${response.status} ${response.statusText}`);
                throw new Error('Network response was not ok');
            }
            const totalCount = parseInt(response.headers.get('X-Total-Count') || '0', 10);
            const result = await response.json();

            // Helpful logging
            if (!result.data && !Array.isArray(result)) {
                console.warn(`[API] Category ${categoryId} returned null/empty data. Result:`, result);
            }

            const data: ProductResponseDTO[] = result.data || result || []; // Handle null data safely

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

            return { products, totalCount };
        } catch (error) {
            console.error('Error fetching products by category:', error);
            return { products: [], totalCount: 0 };
        }
    },

    // Fetch products from multiple subcategory IDs in parallel
    // Used when a parent category is clicked but products are stored in subcategories
    getProductsBySubcategories: async (subcategoryIds: number[], page: number = 1, pageSize: number = 50): Promise<{ products: Product[]; totalCount: number }> => {
        try {
            if (!subcategoryIds || subcategoryIds.length === 0) {
                console.warn('[API] No subcategory IDs provided');
                return { products: [], totalCount: 0 };
            }

            console.log(`[API] Fetching from ${subcategoryIds.length} subcategories:`, subcategoryIds);

            // Fetch from all subcategories in parallel
            const results = await Promise.all(
                subcategoryIds.map(id =>
                    fetch(`${API_BASE_URL}/Product/category/${id}?page=${page}&pageSize=${pageSize}`)
                        .then(async res => {
                            if (!res.ok) return { products: [], totalCount: 0 };
                            const totalCount = parseInt(res.headers.get('X-Total-Count') || '0', 10);
                            const json = await res.json();
                            const data: ProductResponseDTO[] = json.data || json || [];
                            return { products: data, totalCount };
                        })
                        .catch(() => ({ products: [], totalCount: 0 }))
                )
            );

            // Combine all products
            const allProducts: Product[] = [];
            let totalCountSum = 0;

            for (const result of results) {
                totalCountSum += result.totalCount;
                for (const item of result.products) {
                    allProducts.push({
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
                    });
                }
            }

            console.log(`[API] Combined ${allProducts.length} products from subcategories`);
            return { products: allProducts, totalCount: totalCountSum };
        } catch (error) {
            console.error('Error fetching products by subcategories:', error);
            return { products: [], totalCount: 0 };
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
    },

    sendMessageStream: async (
        message: string,
        sessionId: string,
        onChunk: (chunk: string) => void,
        onComplete: () => void,
        onError: (error: Error) => void
    ): Promise<void> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Chat/stream`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ message, sessionId }),
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const reader = response.body?.getReader();
            const decoder = new TextDecoder();

            if (!reader) {
                throw new Error('Response body is null');
            }

            while (true) {
                const { done, value } = await reader.read();

                if (done) {
                    onComplete();
                    break;
                }

                const chunk = decoder.decode(value, { stream: true });
                const lines = chunk.split('\n');

                for (const line of lines) {
                    if (line.startsWith('data: ')) {
                        const data = line.substring(6); // Remove "data: " prefix

                        try {
                            const json = JSON.parse(data);
                            if (json.chunk) {
                                onChunk(json.chunk);
                            } else if (json.error) {
                                onError(new Error(json.error));
                                break;
                            }
                        } catch (parseError) {
                            // Ignore parse errors for incomplete chunks
                            console.warn('Failed to parse SSE data:', data);
                        }
                    }
                }
            }
        } catch (error) {
            console.error('Error in stream:', error);
            onError(error instanceof Error ? error : new Error('Unknown streaming error'));
        }
    }
};
