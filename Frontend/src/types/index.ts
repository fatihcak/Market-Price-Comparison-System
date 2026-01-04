export interface PriceResponseDTO {
    id: number;
    marketId: number;
    marketName: string;
    productId: number;
    productName: string;
    districtId: number;
    districtName: string;
    price: number;
    lastUpdated: string;
}

export interface MarketResponseDTO {
    id: number;
    marketName: string;
    logoUrl?: string;
    website?: string;
    createdAt: string;
}

export interface CategoryResponseDTO {
    id: number;
    categoryName: string;
}

export interface Market {
    id: number;
    name: string;
    logoUrl?: string;
}

export interface Product {
    id: number;
    categoryId?: number;
    categoryName?: string;
    productName?: string;
    marketName?: string;
    districtName?: string;
    brand?: string;
    unit?: string;
    lastUpdated?: string;

    // UI
    name: string;
    price: number;
    oldPrice: number | null;
    market: string;
    discount: number;
    category: string;
    image: string;

    // For consolidated products (same product from different markets)
    variantIds?: number[];
    marketCount?: number;
}

export interface CartItem extends Product {
    quantity: number;
}

export interface ProductResponseDTO {
    id: number;
    categoryId: number;
    categoryName: string;
    productName: string;
    brand?: string;
    unit: string;
    lastUpdated: string;
    createdAt: string;
    price: number;
    oldPrice?: number;
    discount: number;
    marketName: string;
    marketCount: number;
    imageUrl: string;
}

export interface ProductPriceHistoryDTO {
    date: string;
    minPrice: number;
    maxPrice: number;
    averagePrice: number;
}
