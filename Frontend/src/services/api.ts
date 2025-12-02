import { PriceResponseDTO, Product, ProductResponseDTO } from '../types';

const API_BASE_URL = 'http://localhost:5000/api';

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
                image: item.imageUrl // Map imageUrl to image
            }));
        } catch (error) {
            console.error('Error fetching products:', error);
            return [];
        }
    }
};
