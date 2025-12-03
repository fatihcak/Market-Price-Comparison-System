import { ChevronDown, Filter } from 'lucide-react';
import { useState } from 'react';

export default function FilterBar() {
  const [sortBy, setSortBy] = useState('popular');
  const [showFilters, setShowFilters] = useState(false);
  const [priceRange, setPriceRange] = useState([0, 100]);

  return (
    <div className="bg-white rounded-xl border border-gray-100 p-4 mb-8">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div className="flex items-center gap-2">
          <Filter size={20} className="text-gray-600" />
          <span className="font-semibold text-gray-900">Filter</span>
        </div>

        <div className="flex flex-col md:flex-row gap-3 md:gap-4">
          <div className="relative">
            <select
              value={sortBy}
              onChange={(e) => setSortBy(e.target.value)}
              className="appearance-none bg-white border border-gray-200 rounded-lg px-4 py-2 text-sm font-medium text-gray-700 cursor-pointer focus:outline-none focus:ring-2 focus:ring-green-500"
            >
              <option value="popular">Popular Products</option>
              <option value="price-low">Cheapest Price</option>
              <option value="price-high">Higher Price</option>
              <option value="discount">Most Discount</option>
              <option value="newest">Newest</option>
            </select>
            <ChevronDown size={16} className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 pointer-events-none" />
          </div>

          <button
            onClick={() => setShowFilters(!showFilters)}
            className="px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-900 rounded-lg font-medium text-sm transition-colors flex items-center justify-center gap-2"
          >
            <Filter size={16} />
            Detailed
          </button>
        </div>
      </div>

      {showFilters && (
        <div className="mt-6 pt-6 border-t border-gray-100 space-y-6 animate-in fade-in slide-in-from-top-2">
          <div>
            <label className="block text-sm font-semibold text-gray-900 mb-3">Price Range</label>
            <div className="flex items-center gap-4">
              <input
                type="range"
                min="0"
                max="100"
                value={priceRange[0]}
                onChange={(e) => setPriceRange([Number(e.target.value), priceRange[1]])}
                className="flex-1"
              />
              <input
                type="range"
                min="0"
                max="100"
                value={priceRange[1]}
                onChange={(e) => setPriceRange([priceRange[0], Number(e.target.value)])}
                className="flex-1"
              />
              <span className="text-sm font-medium text-gray-600 min-w-fit">{priceRange[0]}₺ - {priceRange[1]}₺</span>
            </div>
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-900 mb-3">Markets</label>
            <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
              {['A101', 'Migros', 'Carrefour', 'Bim', 'Tesco', 'Metro'].map((market) => (
                <label key={market} className="flex items-center gap-2 cursor-pointer">
                  <input
                    type="checkbox"
                    defaultChecked
                    className="w-4 h-4 rounded border-gray-300 text-green-600 cursor-pointer"
                  />
                  <span className="text-sm text-gray-700">{market}</span>
                </label>
              ))}
            </div>
          </div>

          <div className="flex gap-3 pt-4">
            <button className="flex-1 px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium text-sm transition-colors">
              Apply
            </button>
            <button className="flex-1 px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-900 rounded-lg font-medium text-sm transition-colors">
              Reset
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
