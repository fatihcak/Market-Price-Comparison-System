import { MapPin, TrendingDown, ShoppingCart, ChevronLeft, ChevronRight } from 'lucide-react';
import { useState, useEffect } from 'react';
import { Routes, Route, useParams, useNavigate, Navigate } from 'react-router-dom';
import Header from './components/Header';
import SearchBar from './components/SearchBar';
import ProductCard from './components/ProductCard';
import CategorySection from './components/CategorySection';
import FilterBar from './components/FilterBar';
//import Testimonials from './components/Testimonials';
import PriceComparison from './components/PriceComparison';
import BasketComparison from './components/BasketComparison';
import ProductList from './components/ProductList';
import { Product, CartItem } from './types';
import { api } from './services/api';
import AiChatbot from './components/AiChatbot';
import SubCategoryNavbar from './components/SubCategoryNavbar';
import { CATEGORIES } from './constants/categories'; // Import categories



function App() {
  const [comparisonOpen, setComparisonOpen] = useState(false);
  const [basketComparisonOpen, setBasketComparisonOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState('Organik Süt 1L');
  const [selectedProductId, setSelectedProductId] = useState<number | undefined>(undefined);
  const [selectedProductForComparison, setSelectedProductForComparison] = useState<Product | null>(null);
  const [listOpen, setListOpen] = useState(false);
  const [products, setProducts] = useState<Product[]>([]);
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

  useEffect(() => {
    api.getProducts().then(setProducts);
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

  return (
    <div className="min-h-screen bg-white">
      <Header onOpenList={() => setListOpen(true)} itemCount={totalItems} />

      <main className="max-w-7xl mx-auto px-4 py-8">
        <SearchBar />

        <section className="mt-12">
          <div className="flex items-center justify-between mb-8">
            <h2 className="text-3xl font-bold text-gray-900">Categories</h2>
          </div>
          <CategorySection />
          <SubCategoryNavbar />
        </section>

        <Routes>
          <Route path="/" element={<Navigate to="/products/All" replace />} />
          <Route
            path="/products/:category/:subcategory?"
            element={
              <ProductGrid
                products={products}
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
  products: Product[];
  onAdd: (product: Product) => void;
  onCompare: (product: Product) => void;
}

function ProductGrid({ products, onAdd, onCompare }: ProductGridProps) {
  const { category, subcategory } = useParams<{ category: string; subcategory?: string }>();
  const navigate = useNavigate();
  const [currentPage, setCurrentPage] = useState(1);
  const itemsPerPage = 16;

  const selectedCategory = category || 'All';

  useEffect(() => {
    setCurrentPage(1);
  }, [selectedCategory, subcategory]);

  const filteredProducts = products.filter(product => {
    if (selectedCategory === 'All') return true;

    // 1. Find the Main Category definition
    const mainCatDef = CATEGORIES.find(c => c.slug === selectedCategory);
    if (!mainCatDef) return false; // Unknown main category

    // 2. If a specific subcategory is selected, filter by that EXACT subcategory match
    if (subcategory) {
      // Backend 'CategoryName' (e.g. 'Fruits') should match subcategory slug/name
      // Assuming product.category (or product.categoryName) matches the subcategory name
      return product.category === subcategory || product.categoryName === subcategory;
    }

    // 3. If only Main Category is selected, valid products must match ANY of its subcategories
    // E.g. Main='Fruits and Vegetables', valid subs=['Fruits', 'Vegetables']
    // Product category must be one of those.
    const validSubNames = mainCatDef.subCategories.map(s => s.name);
    return validSubNames.includes(product.category) || (product.categoryName && validSubNames.includes(product.categoryName));
  });

  const totalPages = Math.ceil(filteredProducts.length / itemsPerPage);
  const startIndex = (currentPage - 1) * itemsPerPage;
  const displayedProducts = filteredProducts.slice(startIndex, startIndex + itemsPerPage);

  const handlePageChange = (page: number) => {
    if (page >= 1 && page <= totalPages) {
      setCurrentPage(page);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  };

  return (
    <section className="mt-16">
      <FilterBar />

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
          No products found in this category.
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

          <div className="flex gap-2 items-center">
            {(() => {
              const pages: (number | string)[] = [];
              const showEllipsisStart = currentPage > 4;
              const showEllipsisEnd = currentPage < totalPages - 3;

              // Always show first page
              pages.push(1);

              // Show ellipsis after first page if needed
              if (showEllipsisStart) {
                pages.push('...');
              }

              // Show pages around current page
              for (let i = Math.max(2, currentPage - 1); i <= Math.min(totalPages - 1, currentPage + 1); i++) {
                if (!pages.includes(i)) {
                  pages.push(i);
                }
              }

              // Show ellipsis before last page if needed
              if (showEllipsisEnd && !pages.includes('...') || (showEllipsisEnd && pages.filter(p => p === '...').length < 2)) {
                if (currentPage + 1 < totalPages - 1) {
                  pages.push('...');
                }
              }

              // Always show last page
              if (totalPages > 1 && !pages.includes(totalPages)) {
                pages.push(totalPages);
              }

              return pages.map((page, index) => (
                page === '...' ? (
                  <span key={`ellipsis-${index}`} className="w-10 h-10 flex items-center justify-center text-gray-400">
                    ...
                  </span>
                ) : (
                  <button
                    key={page}
                    onClick={() => handlePageChange(page as number)}
                    className={`w-10 h-10 rounded-lg font-medium transition-colors ${currentPage === page
                      ? 'bg-green-600 text-white'
                      : 'hover:bg-gray-50 text-gray-600'
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
    </section>
  );
}

export default App;
