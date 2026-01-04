import { MapPin, TrendingDown, ShoppingCart, ChevronLeft, ChevronRight } from 'lucide-react';
import { useState, useEffect } from 'react';
import { Routes, Route, useParams, useNavigate, Navigate } from 'react-router-dom';
import Header from './components/Header';
import SearchBar from './components/SearchBar';
import ProductCard from './components/ProductCard';
import CategorySection from './components/CategorySection';
import FilterBar, { FilterState } from './components/FilterBar';
//import Testimonials from './components/Testimonials';
import PriceComparison from './components/PriceComparison';
import BasketComparison from './components/BasketComparison';
import ProductList from './components/ProductList';
import { Product, CartItem } from './types';
import { api } from './services/api';
import AiChatbot from './components/AiChatbot';
import SubCategoryNavbar from './components/SubCategoryNavbar';
import { CATEGORIES, Category } from './constants/categories'; // Import categories



function App() {
  const [comparisonOpen, setComparisonOpen] = useState(false);
  const [basketComparisonOpen, setBasketComparisonOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState('Organik Süt 1L');
  const [selectedProductId, setSelectedProductId] = useState<number | undefined>(undefined);
  const [selectedProductForComparison, setSelectedProductForComparison] = useState<Product | null>(null);
  const [listOpen, setListOpen] = useState(false);
  const [searchQuery, setSearchQuery] = useState('');
  const [shoppingList, setShoppingList] = useState<CartItem[]>(() => {
    const saved = localStorage.getItem('market_basket');
    if (saved) {
      try {
        return JSON.parse(saved);
      } catch (error) {
        console.error('Failed to parse basket:', error);
      }
    }
    return [];
  });

  const [activeCategories, setActiveCategories] = useState<Category[]>(CATEGORIES);

  // Preload Categories & Fetch Real IDs
  const preloadCategories = async () => {
      let updatedCategories: Category[] = [];
      // 1. Fetch proper IDs from Backend (Fix for "No products found")
      try {
          const backendCats = await api.getCategories();
          if (backendCats && backendCats.length > 0) {
              updatedCategories = CATEGORIES.map(frontendCat => {
                  // Find matching backend category by loose name matching
                  const match = backendCats.find(bc => 
                      bc.categoryName.toLowerCase().trim() === frontendCat.name.toLowerCase().trim() ||
                      bc.categoryName.toLowerCase().trim() === frontendCat.slug.toLowerCase().trim() || 
                      // Handle potential singular/plural mismatch
                       // e.g. "Drinks" vs "Drink"
                      (frontendCat.name.includes(bc.categoryName) || bc.categoryName.includes(frontendCat.name)) 
                  );

                  if (match) {
                      console.log(`✅ Mapped Category: ${frontendCat.name} (ID: ${frontendCat.id} -> ${match.id})`);
                      return { ...frontendCat, id: match.id };
                  } else {
                      return frontendCat;
                  }
              });
              setActiveCategories(updatedCategories);
          }
      } catch (e) {
          console.error("Failed to sync category IDs", e);
      }

    // Use mapped IDs for preloading, or fallback to default
    const currentCats = updatedCategories.length > 0 ? updatedCategories : CATEGORIES;
    console.log("🚀 Starting background category preload with IDs:", currentCats.map(c => c.id));
    
    // Process one by one with a small delay to be gentle
    for (const cat of currentCats) {
      const id = cat.id;
      // Check if already cached and valid (6 hours)
      const key = `category_${id}_data`;
      const cached = localStorage.getItem(key);
      let isValid = false;
      if (cached) {
        try {
            const parsed = JSON.parse(cached);
            if (Date.now() - parsed.timestamp < 6 * 60 * 60 * 1000) {
                isValid = true;
            }
        } catch(e) { /* ignore */ }
      }

      if (!isValid) {
          try {
            // Fetch page 1, size 50
            const result = await api.getProductsByCategory(id, 1, 50);
            if (result && result.products.length > 0) {
                const cacheData = {
                    timestamp: Date.now(),
                    products: result.products,
                    totalCount: result.totalCount
                };
                localStorage.setItem(key, JSON.stringify(cacheData));
                console.log(`✅ Preloaded Category ${id} (${cat.name})`);
            }
          } catch (err) {
              console.error(`Failed to preload category ${id}`, err);
          }
      } else {
          console.log(`⚡ Category ${id} already cached and valid in LocalStorage.`);
      }
    }
  };

  useEffect(() => {
    // Start preloading categories after a short delay to ensure UI is interactive first
    const timer = setTimeout(() => {
        preloadCategories();
    }, 3000);
    return () => clearTimeout(timer);
  }, []);

  useEffect(() => {
    localStorage.setItem('market_basket', JSON.stringify(shoppingList));
  }, [shoppingList]);

  const openComparison = (product: Product) => {
    setSelectedProduct(product.name);
    setSelectedProductId(product.id);
    setSelectedProductForComparison(product);
    setComparisonOpen(true);
  };

  const addToShoppingList = (product: Product) => {
    setShoppingList(prevList => {
      const existingItem = prevList.find(item => item.id === product.id);
      if (existingItem) {
        return prevList.map(item =>
          item.id === product.id
            ? { ...item, quantity: item.quantity + 1 }
            : item
        );
      }
      return [...prevList, { ...product, quantity: 1 }];
    });
    // setListOpen(true); // Disabled auto-open
  };

  const updateQuantity = (productId: number, delta: number) => {
    setShoppingList(prevList => {
      return prevList.map(item => {
        if (item.id === productId) {
          return { ...item, quantity: Math.max(0, item.quantity + delta) };
        }
        return item;
      }).filter(item => item.quantity > 0);
    });
  };

  const removeFromShoppingList = (productId: number) => {
    setShoppingList(prevList => prevList.filter(item => item.id !== productId));
  };

  const totalItems = shoppingList.reduce((sum, item) => sum + item.quantity, 0);


  const handleSearch = (query: string) => {
    setSearchQuery(query);
  };

  return (
    <div className="min-h-screen bg-white">
      <Header onOpenList={() => setListOpen(true)} itemCount={totalItems} />

      <main className="max-w-7xl mx-auto px-4 py-8">
        <SearchBar onSearch={handleSearch} />

        <section className="mt-12">
          <div className="flex items-center justify-between mb-8">
            <h2 className="text-3xl font-bold text-gray-900">Categories</h2>
          </div>
          <CategorySection categories={activeCategories} />
          <SubCategoryNavbar categories={activeCategories} />
        </section>

        <Routes>
          <Route path="/" element={<Navigate to="/products/All" replace />} />
          <Route
            path="/products/:category/:subcategory?"
            element={
              <ProductGrid
                searchQuery={searchQuery}
                categories={activeCategories}
                onAdd={addToShoppingList}
                onCompare={openComparison}
              />
            }
          />
        </Routes>

        <section className="mt-20 bg-gradient-to-r from-green-50 to-emerald-50 rounded-2xl p-12">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center hover:scale-105 transition-transform duration-300">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 shadow-md">
                <TrendingDown className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">Price Tracking</h3>
              <p className="text-gray-600">Track price changes of your favorite products</p>
            </div>

            <div className="text-center hover:scale-105 transition-transform duration-300">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 shadow-md">
                <MapPin className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">Closest Market</h3>
              <p className="text-gray-600">Find the most affordable markets around you</p>
            </div>

            <div className="text-center hover:scale-105 transition-transform duration-300">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 shadow-md">
                <ShoppingCart className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">Smart List</h3>
              <p className="text-gray-600">Create shopping lists and save money</p>
            </div>
          </div>
        </section>
      </main>



      <footer className="bg-gray-900 text-gray-300 py-12">
        <div className="max-w-7xl mx-auto px-4">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8 mb-8">
            <div>
              <h4 className="text-white font-bold mb-4">Market Price</h4>
              <p className="text-sm">Find and compare the best prices</p>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Quick Links</h4>
              <ul className="text-sm space-y-2">
                <li><a href="#" className="hover:text-white transition-colors">About Us</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Markets</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Blog</a></li>
              </ul>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Support</h4>
              <ul className="text-sm space-y-2">
                <li><a href="#" className="hover:text-white transition-colors">Contact</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Privacy</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Terms</a></li>
              </ul>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Follow Us</h4>
              <p className="text-sm">Follow Us</p>
            </div>
          </div>
          <div className="border-t border-gray-700 pt-8 text-sm text-center">
            <p>&copy; Market Price. All rights reserved.</p>
          </div>
        </div>
      </footer>

      <PriceComparison
        isOpen={comparisonOpen}
        onClose={() => setComparisonOpen(false)}
        productName={selectedProduct}
        productId={selectedProductId}
        productImage={selectedProductForComparison?.image}
        variantIds={selectedProductForComparison?.variantIds}
        onAdd={addToShoppingList}
      />

      <BasketComparison
        isOpen={basketComparisonOpen}
        onClose={() => setBasketComparisonOpen(false)}
        products={shoppingList}
      />

      <ProductList
        isOpen={listOpen}
        onClose={() => setListOpen(false)}
        products={shoppingList}
        onRemove={removeFromShoppingList}
        onUpdateQuantity={updateQuantity}
        onCompare={() => {
          setListOpen(false);
          setBasketComparisonOpen(true);
        }}
      />

      <AiChatbot />
    </div>
  );
}

interface ProductGridProps {
  // products prop removed as it was unused and caused confusion
  searchQuery?: string;
  categories: Category[];
  onAdd: (product: Product) => void;
  onCompare: (product: Product) => void;
}

function ProductGrid({ searchQuery, categories, onAdd, onCompare }: ProductGridProps) {
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
                 } catch(e) {}
             }
         }

         if (!dataFound) {
             // Fallback to API
             if (catDef) {
                 const result = await api.getProductsByCategory(catDef.id, 1, pageSize);
                 setLoadedProducts(result.products);
                 setTotalCount(result.totalCount); 
             } else {
                 // Fallback for subcategories or unknown
                 const result = await api.getProducts(1, pageSize);
                 setLoadedProducts(result.products);
                 setTotalCount(result.totalCount);
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
          {subcategory ? subcategory : (selectedCategory === 'All' ? 'All Products' : selectedCategory)}
        </h2>
        <button
          onClick={() => navigate('/products/All')}
          className="text-green-600 hover:text-green-700 font-semibold text-sm transition-colors"
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
          {isLoading ? 'Loading products...' : 'No products found in this category.'}
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

export default App;
