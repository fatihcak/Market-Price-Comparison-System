import { Search, Mic, MapPin } from 'lucide-react';
import { useState } from 'react';

interface SearchBarProps {
  onSearch: (query: string) => void;
}

export default function SearchBar({ onSearch }: SearchBarProps) {
  const [searchFocus, setSearchFocus] = useState(false);
  const [query, setQuery] = useState('');

  const handleSearch = (value: string) => {
    setQuery(value);
    onSearch(value);
  };

  const handleQuickSearch = (term: string) => {
    setQuery(term);
    onSearch(term);
  };

  return (
    <div className="relative">
      <div className="bg-gradient-to-r from-green-50 to-emerald-50 rounded-2xl p-8">
        <h1 className="text-4xl font-bold text-gray-900 mb-8">
          Find the Best Prices Now Easier
        </h1>

        <div className="grid grid-cols-1 lg:grid-cols-2 gap-4">
          <div className={`relative transition-all ${searchFocus ? 'ring-2 ring-green-500' : ''}`}>
            <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
            <input
              type="text"
              value={query}
              onChange={(e) => handleSearch(e.target.value)}
              placeholder="Enter the name of the product you are looking for..."
              onFocus={() => setSearchFocus(true)}
              onBlur={() => setSearchFocus(false)}
              className="w-full pl-12 pr-12 py-4 rounded-xl border border-gray-200 focus:outline-none bg-white text-gray-900 placeholder:text-gray-400"
            />
            {query && (
              <button
                onClick={() => handleSearch('')}
                className="absolute right-12 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600"
              >
                ✕
              </button>
            )}
            <Mic className="absolute right-4 top-1/2 transform -translate-y-1/2 text-gray-400 cursor-pointer hover:text-gray-600" size={20} />
          </div>

          <div className="relative">
            <MapPin className="absolute left-4 top-1/2 transform -translate-y-1/2 text-gray-400" size={20} />
            <select className="w-full pl-12 pr-4 py-4 rounded-xl border border-gray-200 focus:outline-none focus:ring-2 focus:ring-green-500 bg-white text-gray-900 appearance-none cursor-pointer">
              <option>All Cities</option>
              <option>Lefkoşa</option>
              <option>Gazimağusa</option>
            </select>
          </div>
        </div>

        <div className="mt-6 flex flex-wrap gap-3">
          <span className="text-sm text-gray-600">Popular searches:</span>
          <button
            onClick={() => handleQuickSearch('Süt')}
            className="px-4 py-2 bg-white hover:bg-gray-100 text-gray-700 text-sm rounded-full border border-gray-200 transition-colors"
          >
            Milk
          </button>
          <button
            onClick={() => handleQuickSearch('Ekmek')}
            className="px-4 py-2 bg-white hover:bg-gray-100 text-gray-700 text-sm rounded-full border border-gray-200 transition-colors"
          >
            Bread
          </button>
          <button
            onClick={() => handleQuickSearch('Yumurta')}
            className="px-4 py-2 bg-white hover:bg-gray-100 text-gray-700 text-sm rounded-full border border-gray-200 transition-colors"
          >
            Egg
          </button>
          <button
            onClick={() => handleQuickSearch('Zeytinyağı')}
            className="px-4 py-2 bg-white hover:bg-gray-100 text-gray-700 text-sm rounded-full border border-gray-200 transition-colors"
          >
            Olive Oil
          </button>
        </div>
      </div>
    </div>
  );
}
