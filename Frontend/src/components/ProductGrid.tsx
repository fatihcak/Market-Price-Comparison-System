import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import ProductCard from './ProductCard';
import FilterBar, { FilterState } from './FilterBar';
import { Product } from '../types';
import { api } from '../services/api';
import { CATEGORIES, Category } from '../constants/categories';

interface ProductGridProps {
    searchQuery?: string;
    categories: Category[];
    onAdd: (product: Product) => void;
    onCompare: (product: Product) => void;
}

export default function ProductGrid({ searchQuery, categories, onAdd, onCompare }: ProductGridProps) {
    const { category, subcategory } = useParams<{ category: string; subcategory?: string }>();
    const navigate = useNavigate();
    const [currentPage, setCurrentPage] = useState(1);
    const [filters, setFilters] = useState<FilterState>({
        sortBy: 'popular',
        minPrice: 0,
        maxPrice: 1000,
        selectedMarkets: []
    });
    const itemsPerPage = 16;

    const handleNavigate = (path: string) => {
        const scrollY = window.scrollY;
        navigate(path);
        setTimeout(() => window.scrollTo(0, scrollY), 0);
    };

    // Pagination state for Load More
    const [loadedProducts, setLoadedProducts] = useState<Product[]>([]);
    const [apiPage, setApiPage] = useState(1);
    const [totalCount, setTotalCount] = useState(0);
    const [isLoading, setIsLoading] = useState(false);
    const pageSize = 48; // Fetch 3 pages worth of data (16 * 3)

    const selectedCategory = category || 'All';

    // Fetch products from API based on category OR search
    useEffect(() => {
        const fetchProducts = async () => {
            setIsLoading(true);
            setApiPage(1);

            let dataFound = false;

            // 0. Search Logic (Priority)
            if (searchQuery && searchQuery.trim().length > 0) {
                const result = await api.searchProducts(searchQuery, 1, pageSize);
                setLoadedProducts(result.products);
                setTotalCount(result.totalCount);
                dataFound = true;
            }
            // 1. URL-based routing logic
            else if (selectedCategory === 'All' && !subcategory) {
                // HOMEPAGE: Discounted Products (Pagination supported)
                const result = await api.getProductsByDiscount(1, pageSize);
                setLoadedProducts(result.products);
                setTotalCount(result.totalCount);
                dataFound = true;
            } else {
                // CATEGORY PAGE
                const catDef = categories.find(c => c.slug === selectedCategory);

                if (catDef && !subcategory) {
                    const key = `category_${catDef.id}_data`;
                    const cached = localStorage.getItem(key);
                    if (cached) {
                        try {
                            const parsed = JSON.parse(cached);
                            // Check expiry (6h)
                            if (Date.now() - parsed.timestamp < 6 * 60 * 60 * 1000) {
                                console.log(`⚡ Loaded Category ${catDef.id} from LocalStorage!`);
                                setLoadedProducts(parsed.products);
                                setTotalCount(parsed.totalCount || 50); // Use cached totalCount or fallback
                                dataFound = true;
                            }
                        } catch (e) { }
                    }
                }

                if (!dataFound) {
                    // Fallback to API
                    if (catDef) {
                        // Check if this category has subcategories with products
                        // Products are stored in subcategories, not parent categories
                        if (catDef.subCategories && catDef.subCategories.length > 0) {
                            // Find matching subcategory IDs from backend categories
                            const subCatSlugs = catDef.subCategories.map(sc => sc.slug);

                            // Use api to fetch from all subcategories
                            // Note: We need subcategory IDs, which are fetched from backend in App.tsx
                            // For now, just try the parent category first
                            const result = await api.getProductsByCategory(catDef.id, 1, pageSize);

                            if (result.products.length === 0) {
                                // Parent category is empty, this is expected since products are in subcategories
                                // Try fetching from first subcategory as a fallback
                                console.log(`[ProductGrid] Parent category ${catDef.id} is empty, trying subcategories...`);

                                // Get subcategory IDs by fetching categories from API
                                const allCats = await api.getCategories();
                                const subCatIds = allCats
                                    .filter(bc => subCatSlugs.includes(bc.categoryName))
                                    .map(bc => bc.id);

                                if (subCatIds.length > 0) {
                                    console.log(`[ProductGrid] Fetching from subcategory IDs:`, subCatIds);
                                    const subResult = await api.getProductsBySubcategories(subCatIds, 1, pageSize);
                                    setLoadedProducts(subResult.products);
                                    setTotalCount(subResult.totalCount);
                                } else {
                                    setLoadedProducts([]);
                                    setTotalCount(0);
                                }
                            } else {
                                setLoadedProducts(result.products);
                                setTotalCount(result.totalCount);
                            }
                        } else {
                            // No subcategories, fetch directly
                            const result = await api.getProductsByCategory(catDef.id, 1, pageSize);
                            setLoadedProducts(result.products);
                            setTotalCount(result.totalCount);
                        }
                    } else {
                        // Fallback for subcategories or unknown - search by category name
                        const allCats = await api.getCategories();
                        const matchingCat = allCats.find(bc => bc.categoryName === selectedCategory);

                        if (matchingCat) {
                            console.log(`[ProductGrid] Found subcategory ${selectedCategory} with ID ${matchingCat.id}`);
                            const result = await api.getProductsByCategory(matchingCat.id, 1, pageSize);
                            setLoadedProducts(result.products);
                            setTotalCount(result.totalCount);
                        } else {
                            const result = await api.getProducts(1, pageSize);
                            setLoadedProducts(result.products);
                            setTotalCount(result.totalCount);
                        }
                    }
                }
            }

            setIsLoading(false);
        };

        fetchProducts();
    }, [selectedCategory, subcategory, searchQuery]);

    // Smart Prefetching: When on middle page of a packet (2, 5, 8...), fetch next packet
    useEffect(() => {
        const isMiddleOfPacket = (currentPage % 3) === 2; // Page 2, 5, 8...
        // Calculate if we need more data for the NEXT packet (Items needed > Loaded Items)
        // Current Packet covers up to page: (Math.ceil(currentPage / 3) * 3)
        // Next Packet covers up to page: (Math.ceil(currentPage / 3) + 1) * 3
        const packetNumber = Math.ceil(currentPage / 3);
        const neededItems = (packetNumber + 1) * 3 * itemsPerPage;

        if (isMiddleOfPacket && loadedProducts.length < neededItems && loadedProducts.length < totalCount && !isLoading) {
            console.log(`🚀 Prefetching next packet from page ${currentPage}...`);
            handleLoadMore();
        }
    }, [currentPage, loadedProducts.length, totalCount, isLoading]);

    // Load more products handler (Next Packet)
    const handleLoadMore = async () => {
        if (isLoading) return;

        setIsLoading(true);
        const nextPage = apiPage + 1;
        let newProducts: Product[] = [];

        if (selectedCategory === 'All' && !subcategory) {
            // Homepage / Discount
            const result = await api.getProductsByDiscount(nextPage, pageSize);
            newProducts = result.products;
        } else {
            const catDef = CATEGORIES.find(c => c.slug === selectedCategory);
            if (catDef && !subcategory) {
                const result = await api.getProductsByCategory(catDef.id, nextPage, pageSize);
                newProducts = result.products;
            } else {
                const result = await api.getProducts(nextPage, pageSize);
                newProducts = result.products;
            }
        }

        if (newProducts.length > 0) {
            setLoadedProducts(prev => [...prev, ...newProducts]);
            setApiPage(nextPage);
        }
        setIsLoading(false);
    };

    const hasMore = loadedProducts.length < totalCount;

    useEffect(() => {
        setCurrentPage(1);
        window.scrollTo({ top: 0, behavior: 'smooth' });
    }, [selectedCategory, subcategory, filters]);

    const handleFilterChange = (newFilters: FilterState) => {
        setFilters(newFilters);
    };

    // Apply category, market, and price filters
    const filteredProducts = loadedProducts.filter(product => {
        const mainCatDef = CATEGORIES.find(c => c.slug === selectedCategory);

        if (selectedCategory !== 'All' && !mainCatDef) {
            return false;
        }

        if (subcategory) {
            if (product.category !== subcategory && product.categoryName !== subcategory) {
                return false;
            }
        }

        if (filters.selectedMarkets.length > 0) {
            if (!filters.selectedMarkets.includes(product.market)) {
                return false;
            }
        }

        if (product.price < filters.minPrice || product.price > filters.maxPrice) {
            return false;
        }

        return true;
    });

    // Apply sorting
    const sortedProducts = [...filteredProducts].sort((a, b) => {
        switch (filters.sortBy) {
            case 'price-low':
                return a.price - b.price;
            case 'price-high':
                return b.price - a.price;
            case 'discount':
                return (b.discount || 0) - (a.discount || 0);
            case 'newest':
                return (b.id || 0) - (a.id || 0);
            case 'popular':
            case 'relevance':
            default:
                // For 'relevance', logic can be added. Default to marketCount or ID
                return (b.marketCount || 1) - (a.marketCount || 1);
        }
    });

    const totalPages = Math.ceil(sortedProducts.length / itemsPerPage);
    const startIndex = (currentPage - 1) * itemsPerPage;
    const displayedProducts = sortedProducts.slice(startIndex, startIndex + itemsPerPage);

    const handlePageChange = (page: number) => {
        if (page >= 1 && page <= totalPages) {
            setCurrentPage(page);
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    };

    return (
        <section className="mt-16">
            <FilterBar onFilterChange={handleFilterChange} />

            <div className="flex items-center justify-between mb-8">
                <h2 className="text-3xl font-bold text-gray-900">
                    {subcategory ? subcategory : (selectedCategory === 'All' ? 'Popular Products' : selectedCategory)}
                </h2>
                <button
                    onClick={() => handleNavigate(`/products/${selectedCategory}`)}
                    className="text-green-600 bg-green-100 rounded-full px-4 py-2 hover:text-green-700 font-semibold text-sm transition-colors"
                >
                    See All →
                </button>
            </div>

            {displayedProducts.length > 0 ? (
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
                    {displayedProducts.map((product) => (
                        <div key={product.id}>
                            <ProductCard
                                product={product}
                                onAdd={onAdd}
                                onCompare={onCompare}
                            />
                        </div>
                    ))}
                </div>
            ) : (
                <div className="text-center py-12 text-gray-500">
                    <p>{isLoading ? 'Loading products...' : 'No products found in this category.'}</p>
                </div>
            )}

            {/* Pagination Controls */}
            {totalPages > 1 && (
                <div className="flex justify-center items-center gap-4 mt-12">
                    <button
                        onClick={() => handlePageChange(currentPage - 1)}
                        disabled={currentPage === 1}
                        className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                    >
                        <ChevronLeft size={20} />
                    </button>

                    <div className="flex gap-1 items-center">
                        {(() => {
                            const pages: (number | string)[] = [];
                            const maxMiddlePages = 7;

                            pages.push(1);

                            if (totalPages <= 10) {
                                for (let i = 2; i <= totalPages; i++) {
                                    pages.push(i);
                                }
                            } else {
                                const half = Math.floor(maxMiddlePages / 2);
                                let start = Math.max(2, currentPage - half);
                                let end = Math.min(totalPages - 1, currentPage + half);

                                if (currentPage <= half + 2) {
                                    start = 2;
                                    end = maxMiddlePages + 1;
                                } else if (currentPage >= totalPages - half - 1) {
                                    start = totalPages - maxMiddlePages;
                                    end = totalPages - 1;
                                }

                                if (start > 2) {
                                    pages.push('...');
                                }

                                for (let i = start; i <= end; i++) {
                                    pages.push(i);
                                }

                                if (end < totalPages - 1) {
                                    pages.push('...');
                                }

                                pages.push(totalPages);
                            }

                            return pages.map((page, index) => (
                                page === '...' ? (
                                    <span key={`ellipsis-${index}`} className="w-8 h-10 flex items-center justify-center text-gray-400">
                                        ...
                                    </span>
                                ) : (
                                    <button
                                        key={page}
                                        onClick={() => handlePageChange(page as number)}
                                        className={`w-10 h-10 rounded-lg font-medium transition-all ${currentPage === page
                                            ? 'bg-blue-600 text-white shadow-lg scale-110'
                                            : 'hover:bg-gray-100 text-gray-600 border border-gray-200'
                                            }`}
                                    >
                                        {page}
                                    </button>
                                )
                            ));
                        })()}
                    </div>

                    <button
                        onClick={() => handlePageChange(currentPage + 1)}
                        disabled={currentPage === totalPages}
                        className="p-2 rounded-lg border border-gray-200 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
                    >
                        <ChevronRight size={20} />
                    </button>
                </div>
            )}

            {/* Load More Button (Manual Trigger if needed) */}
            {hasMore && (
                <div className="flex justify-center mt-8">
                    {isLoading && <span className="text-gray-500 animate-pulse">Loading more products...</span>}
                </div>
            )}
        </section>
    );
}
