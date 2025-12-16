import { PriceResponseDTO, Product, ProductResponseDTO, ProductPriceHistoryDTO } from '../types';

const API_BASE_URL = import.meta.env.PROD
    ? '/api'
    : 'http://localhost:5000/api';

export const api = {
    getPricesByProduct: async (productId: number): Promise<PriceResponseDTO[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Prices/product/${productId}`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return await response.json();
        } catch (error) {
            console.error('Error fetching prices:', error);
            return [];
        }
    },

    getProducts: async (): Promise<Product[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Products`);
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            const data: ProductResponseDTO[] = await response.json();

            // Map Backend DTO to Frontend UI Model
            return data.map(item => ({
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
        } catch (error) {
            console.error('Error fetching products:', error);
            return [];
        }
    },

    getProductHistory: async (productId: number, days: number = 30): Promise<ProductPriceHistoryDTO[]> => {
        try {
            const response = await fetch(`${API_BASE_URL}/Products/${productId}/history?days=${days}`);
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
            const response = await fetch(`${API_BASE_URL}/Products/search?name=${encodeURIComponent(query)}`);
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
            const response = await fetch(`${API_BASE_URL}/Products/category/${categoryId}`);
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
