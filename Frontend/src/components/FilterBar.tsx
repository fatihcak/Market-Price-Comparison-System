import { ChevronDown, Filter, Loader2 } from 'lucide-react';
import { useState, useEffect } from 'react';
import { api } from '../services/api';
import { Market } from '../types';

export interface FilterState {
  sortBy: string;
  minPrice: number;
  maxPrice: number;
  selectedMarkets: string[];
}

interface FilterBarProps {
  onFilterChange: (filters: FilterState) => void;
}

export default function FilterBar({ onFilterChange }: FilterBarProps) {
  const [sortBy, setSortBy] = useState('popular');
  const [showFilters, setShowFilters] = useState(false);
  const [minPrice, setMinPrice] = useState(0);
  const [maxPrice, setMaxPrice] = useState(1000);
  const [markets, setMarkets] = useState<Market[]>([]);
  const [selectedMarkets, setSelectedMarkets] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(true);

  const min = 0;
  const max = 2000;

  // Fetch markets on mount
  useEffect(() => {
    const fetchMarkets = async () => {
      setLoading(true);
      const marketData = await api.getMarkets();
      setMarkets(marketData);
      // Select all markets by default
      setSelectedMarkets(new Set(marketData.map(m => m.name)));
      setLoading(false);
    };
    fetchMarkets();
  }, []);

  // Apply filters when sortBy changes (immediate effect for sorting)
  useEffect(() => {
    onFilterChange({
      sortBy,
      minPrice,
      maxPrice,
      selectedMarkets: Array.from(selectedMarkets)
    });
  }, [sortBy]);

  const handleMinChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = Math.min(Number(e.target.value), maxPrice - 10);
    setMinPrice(value);
  };

  const handleMaxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = Math.max(Number(e.target.value), minPrice + 10);
    setMaxPrice(value);
  };

  const toggleMarket = (marketName: string) => {
    setSelectedMarkets(prev => {
      const newSet = new Set(prev);
      if (newSet.has(marketName)) {
        newSet.delete(marketName);
      } else {
        newSet.add(marketName);
      }
      return newSet;
    });
  };

  const handleApply = () => {
    onFilterChange({
      sortBy,
      minPrice,
      maxPrice,
      selectedMarkets: Array.from(selectedMarkets)
    });
    setShowFilters(false);
  };

  const handleReset = () => {
    setMinPrice(0);
    setMaxPrice(1000);
    setSortBy('popular');
    setSelectedMarkets(new Set(markets.map(m => m.name)));
    onFilterChange({
      sortBy: 'popular',
      minPrice: 0,
      maxPrice: 1000,
      selectedMarkets: markets.map(m => m.name)
    });
  };

  const minPercent = ((minPrice - min) / (max - min)) * 100;
  const maxPercent = ((maxPrice - min) / (max - min)) * 100;

  return (
    <div className="bg-white rounded-xl border border-gray-100 p-4 mb-8">
      <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
        <div className="flex items-center gap-2">
          <Filter size={20} className="text-green-600" />
          <span className="font-semibold text-green-600">Filter</span>
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
            <label className="block text-sm font-semibold text-gray-900 mb-4">Price Range</label>
            <div className="relative w-full h-12 flex items-center px-2">
              {/* Visual Track */}
              <div className="absolute w-[calc(100%-1rem)] h-2 bg-gray-200 rounded-full left-2"></div>
              {/* Active Range */}
              <div
                className="absolute h-2 bg-green-500 rounded-full left-2"
                style={{ left: `calc(${minPercent}% + 0.5rem)`, width: `calc(${maxPercent - minPercent}% + -1rem)` }}
              ></div>

              {/* Range Inputs */}
              <input
                type="range"
                min={min}
                max={max}
                value={minPrice}
                onChange={handleMinChange}
                className="absolute w-[calc(100%-1rem)] h-2 appearance-none bg-transparent pointer-events-none [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:w-5 [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:bg-white [&::-webkit-slider-thumb]:border-2 [&::-webkit-slider-thumb]:border-green-500 [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:cursor-pointer [&::-webkit-slider-thumb]:shadow-md [&::-webkit-slider-thumb]:hover:scale-110 [&::-webkit-slider-thumb]:transition-transform z-30 left-2"
              />
              <input
                type="range"
                min={min}
                max={max}
                value={maxPrice}
                onChange={handleMaxChange}
                className="absolute w-[calc(100%-1rem)] h-2 appearance-none bg-transparent pointer-events-none [&::-webkit-slider-thumb]:pointer-events-auto [&::-webkit-slider-thumb]:w-5 [&::-webkit-slider-thumb]:h-5 [&::-webkit-slider-thumb]:appearance-none [&::-webkit-slider-thumb]:bg-white [&::-webkit-slider-thumb]:border-2 [&::-webkit-slider-thumb]:border-green-500 [&::-webkit-slider-thumb]:rounded-full [&::-webkit-slider-thumb]:cursor-pointer [&::-webkit-slider-thumb]:shadow-md [&::-webkit-slider-thumb]:hover:scale-110 [&::-webkit-slider-thumb]:transition-transform z-40 left-2"
              />
            </div>

            <div className="flex justify-between items-center mt-[-10px]">
              <div className="px-3 py-1 border border-gray-200 rounded-lg text-sm font-medium text-gray-700 bg-white shadow-sm">
                {minPrice} ₺
              </div>
              <div className="px-3 py-1 border border-gray-200 rounded-lg text-sm font-medium text-gray-700 bg-white shadow-sm">
                {maxPrice} ₺
              </div>
            </div>
          </div>

          <div>
            <label className="block text-sm font-semibold text-gray-900 mb-3">Markets</label>
            {loading ? (
              <div className="flex items-center justify-center py-4">
                <Loader2 size={24} className="animate-spin text-green-600" />
                <span className="ml-2 text-gray-500">Loading markets...</span>
              </div>
            ) : markets.length === 0 ? (
              <div className="text-gray-500 text-sm py-2">No markets available</div>
            ) : (
              <div className="grid grid-cols-2 md:grid-cols-4 gap-3">
                {markets.map((market) => (
                  <label key={market.id} className="flex items-center gap-2 cursor-pointer group">
                    <div className="relative flex items-center">
                      <input
                        type="checkbox"
                        checked={selectedMarkets.has(market.name)}
                        onChange={() => toggleMarket(market.name)}
                        className="peer w-4 h-4 rounded border-gray-300 text-green-600 focus:ring-green-500 cursor-pointer"
                      />
                    </div>
                    <span className="text-sm text-gray-700 group-hover:text-green-700 transition-colors">{market.name}</span>
                  </label>
                ))}
              </div>
            )}
          </div>

          <div className="flex gap-3 pt-4">
            <button
              onClick={handleApply}
              className="flex-1 px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium text-sm transition-colors shadow-sm hover:shadow-md"
            >
              Apply
            </button>
            <button
              onClick={handleReset}
              className="flex-1 px-4 py-2 bg-gray-100 hover:bg-gray-200 text-gray-900 rounded-lg font-medium text-sm transition-colors"
            >
              Reset
            </button>
          </div>
        </div>
      )}
    </div>
  );
}
