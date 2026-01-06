import { useState, useEffect, useRef } from 'react';
import { Routes, Route } from 'react-router-dom';
import Header from './components/Header';
//import Testimonials from './components/Testimonials';
import PriceComparison from './components/PriceComparison';
import BasketComparison from './components/BasketComparison';
import ProductList from './components/ProductList';
import { Product, CartItem } from './types';
import { api } from './services/api';
import AiChatbot from './components/AiChatbot';
import Markets from './components/Markets';
import Home from './components/Home';
import { CATEGORIES, Category } from './constants/categories'; // Import categories



function App() {
  const preloadStarted = useRef(false);
  const [comparisonOpen, setComparisonOpen] = useState(false);
  const [basketComparisonOpen, setBasketComparisonOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState('Organik Süt 1L');
  const [selectedProductId, setSelectedProductId] = useState<number | undefined>(undefined);
  const [selectedProductForComparison, setSelectedProductForComparison] = useState<Product | null>(null);
  const [listOpen, setListOpen] = useState(false);
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

    // Process one by one with a delay to prevent request waterfall
    for (const cat of currentCats) {
      // Add artificial delay between requests (1.5s) to be gentle on the server
      await new Promise(resolve => setTimeout(resolve, 1500));

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
        } catch (e) { /* ignore */ }
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
      if (!preloadStarted.current) {
        preloadStarted.current = true;
        preloadCategories();
      }
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




  return (
    <div className="min-h-screen bg-white">
      <Header onOpenList={() => setListOpen(true)} itemCount={totalItems} />

      <Routes>
        <Route path="/markets" element={<Markets />} />
        <Route path="/*" element={
          <Home
            activeCategories={activeCategories}
            onAdd={addToShoppingList}
            onCompare={openComparison}
          />
        } />
      </Routes>



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

      <AiChatbot hideOnMobile={listOpen} />
    </div>
  );
}

export default App;
