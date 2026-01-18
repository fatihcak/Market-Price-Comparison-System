import { useState, useRef, useEffect } from 'react';
import { Routes, Route } from 'react-router-dom';
import { TrendingDown, MapPin, ShoppingCart, Search } from 'lucide-react';
import SearchBar from './SearchBar';
import CategorySection from './CategorySection';
import SubCategoryNavbar from './SubCategoryNavbar';
import ProductGrid from './ProductGrid';
import NotFound from './NotFound';
import { Category } from '../constants/categories';
import { Product } from '../types';

interface HomeProps {
    activeCategories: Category[];
    onAdd: (product: Product) => void;
    onCompare: (product: Product) => void;
}

export default function Home({ activeCategories, onAdd, onCompare }: HomeProps) {
    const [searchQuery, setSearchQuery] = useState('');
    const [showStickySearch, setShowStickySearch] = useState(false);
    const [stickyQuery, setStickyQuery] = useState('');
    const mainSearchRef = useRef<HTMLDivElement>(null);

    const handleSearch = (query: string) => {
        setSearchQuery(query);
        setStickyQuery(query);
    };

    const handleStickySearch = (e: React.FormEvent) => {
        e.preventDefault();
        setSearchQuery(stickyQuery);
    };

    // Observe main search bar visibility
    useEffect(() => {
        const observer = new IntersectionObserver(
            ([entry]) => {
                // Show sticky when main search is out of view
                setShowStickySearch(!entry.isIntersecting);
            },
            { threshold: 0, rootMargin: '-80px 0px 0px 0px' }
        );

        if (mainSearchRef.current) {
            observer.observe(mainSearchRef.current);
        }

        return () => observer.disconnect();
    }, []);

    return (
        <main className="max-w-7xl mx-auto px-4 py-8">
            {/* Main Search Bar */}
            <div ref={mainSearchRef}>
                <SearchBar onSearch={handleSearch} />
            </div>

            <section className="mt-12">
                <div className="flex items-center justify-between mb-8">
                    <h2 className="text-3xl font-bold text-gray-900">Categories</h2>
                </div>
                <CategorySection categories={activeCategories} />
                <SubCategoryNavbar categories={activeCategories} />
            </section>

            {/* Sticky Mini Search Bar */}
            <div
                className={`sticky top-16 z-40 py-3 bg-gray-50/80 backdrop-blur-sm transition-all duration-300 ${showStickySearch
                    ? 'opacity-100 translate-y-0'
                    : 'opacity-0 -translate-y-4 pointer-events-none'
                    }`}
            >
                <form
                    onSubmit={handleStickySearch}
                    className="bg-white rounded-xl shadow-lg border border-gray-200 p-3 flex items-center gap-3"
                >
                    <div className="flex-1 flex items-center gap-2 bg-gray-50 rounded-lg px-3 py-2">
                        <Search size={18} className="text-gray-400" />
                        <input
                            type="text"
                            value={stickyQuery}
                            onChange={(e) => setStickyQuery(e.target.value)}
                            placeholder="Search products..."
                            className="flex-1 bg-transparent outline-none text-sm text-gray-700"
                        />
                    </div>
                    <button
                        type="submit"
                        className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg text-sm font-medium transition-colors"
                    >
                        Search
                    </button>
                </form>
            </div>

            <Routes>
                <Route
                    path="/"
                    element={
                        <ProductGrid
                            searchQuery={searchQuery}
                            categories={activeCategories}
                            onAdd={onAdd}
                            onCompare={onCompare}
                        />
                    }
                />
                <Route
                    path="/products/:category/:subcategory?"
                    element={
                        <ProductGrid
                            searchQuery={searchQuery}
                            categories={activeCategories}
                            onAdd={onAdd}
                            onCompare={onCompare}
                        />
                    }
                />
                <Route path="*" element={<NotFound />} />
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
    );
}

