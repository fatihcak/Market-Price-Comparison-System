import { useState } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { TrendingDown, MapPin, ShoppingCart } from 'lucide-react';
import SearchBar from './SearchBar';
import CategorySection from './CategorySection';
import SubCategoryNavbar from './SubCategoryNavbar';
import ProductGrid from './ProductGrid';
import { Category } from '../constants/categories';
import { Product } from '../types';

interface HomeProps {
    activeCategories: Category[];
    onAdd: (product: Product) => void;
    onCompare: (product: Product) => void;
}

export default function Home({ activeCategories, onAdd, onCompare }: HomeProps) {
    const [searchQuery, setSearchQuery] = useState('');

    const handleSearch = (query: string) => {
        setSearchQuery(query);
    };

    return (
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
                            onAdd={onAdd}
                            onCompare={onCompare}
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
    );
}
