import { PriceResponseDTO, Product, ProductResponseDTO, ProductPriceHistoryDTO } from '../types';

const API_BASE_URL = import.meta.env.PROD
    ? 'INSERT_AWS_API_URL_HERE'
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
            const allPrices: PriceResponseDTO[] = [];

            // Fetch prices for all product IDs in parallel
            const results = await Promise.all(
                productIds.map(id =>
                    fetch(`${API_BASE_URL}/Price/product/${id}`)
                        .then(res => res.ok ? res.json() : [])
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
            const data: ProductResponseDTO[] = await response.json();

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

            return products;
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
