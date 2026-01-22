import { Menu, X, ShoppingBasket, Store, Map, Heart } from 'lucide-react';
import { useState } from 'react';
import { Link } from 'react-router-dom';

interface HeaderProps {
  onOpenList?: () => void;
  itemCount?: number;
  onOpenFavorites?: () => void;
  favoritesCount?: number;
}

export default function Header({ onOpenList, itemCount = 0, onOpenFavorites, favoritesCount = 0 }: HeaderProps) {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <header className="bg-white border-b border-gray-100 sticky top-0 z-50 backdrop-blur-sm bg-opacity-95">
      <div className="max-w-7xl mx-auto px-4 py-4 flex items-center justify-between">
        <Link
          to="/"
          onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}
          className="flex items-center gap-2 cursor-pointer "
        >
          <img
            src="/MarketComparisonLogo1.png"
            alt="Market Comparison System Logo"
            className="w-14 h-12 object-contain"
          />
          <div>
            <span className="text-xl font-bold text-gray-900">Market Comparison System</span>
            <p className="text-xs text-green-700">Find and Compare the Best Prices</p>
          </div>
        </Link>

        <div className="flex items-center gap-2 md:gap-4">
          <nav className="hidden md:flex items-center gap-6 mr-4">
            <Link to="/markets" className="flex items-center text-gray-600 hover:text-green-600 font-medium text-sm transition-colors">
              <Store size={18} className="mr-1.5" />
              Markets
            </Link>
            <Link to="/map" className="flex items-center text-gray-600 hover:text-green-600 font-medium text-sm transition-colors">
              <Map size={18} className="mr-1.5" />
              Map
            </Link>
          </nav>
          <button
            onClick={onOpenFavorites}
            className="hidden md:block bg-red-50 hover:bg-red-100 text-red-600 px-4 py-2 rounded-lg font-medium text-sm transition-all hover:shadow-lg hover:scale-105 transform duration-200 relative"
          >
            <Heart size={16} className="inline-block mr-2 fill-red-500" />
            Favorites
            {favoritesCount > 0 && (
              <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold w-5 h-5 flex items-center justify-center rounded-full border-2 border-white">
                {favoritesCount}
              </span>
            )}
          </button>
          <button
            onClick={onOpenList}
            className="hidden md:block bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg font-medium text-sm transition-all hover:shadow-lg hover:scale-105 transform duration-200 relative"
          >
            <ShoppingBasket size={16} className="inline-block mr-2" />
            List
            {itemCount > 0 && (
              <span className="absolute -top-2 -right-2 bg-red-500 text-white text-xs font-bold w-5 h-5 flex items-center justify-center rounded-full border-2 border-white">
                {itemCount}
              </span>
            )}
          </button>
          <button
            onClick={() => setMobileMenuOpen(!mobileMenuOpen)}
            className="md:hidden p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            {mobileMenuOpen ? <X size={20} /> : <Menu size={20} />}
          </button>
        </div>
      </div>

      {mobileMenuOpen && (
        <div className="md:hidden border-t border-gray-100 p-4 space-y-3 animate-in fade-in slide-in-from-top">
          <Link to="/markets" className="block flex items-center text-gray-600 hover:text-green-600 font-medium text-sm p-2">
            <Store size={18} className="mr-2 inline" /> Markets
          </Link>
          <Link to="/map" className="block flex items-center text-gray-600 hover:text-green-600 font-medium text-sm p-2">
            <Map size={18} className="mr-2 inline" /> Map
          </Link>
          <button
            onClick={onOpenFavorites}
            className="w-full bg-red-50 hover:bg-red-100 text-red-600 px-4 py-2 rounded-lg font-medium text-sm transition-colors relative"
          >
            <Heart size={16} className="inline-block mr-2 fill-red-500" />
            Favorites
            {favoritesCount > 0 && (
              <span className="ml-2 bg-red-500 text-white text-xs font-bold px-2 py-0.5 rounded-full">
                {favoritesCount}
              </span>
            )}
          </button>
          <button
            onClick={onOpenList}
            className="w-full bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded-lg font-medium text-sm transition-colors mt-2 relative"
          >
            <ShoppingBasket size={16} className="inline-block mr-2" />
            List
            {itemCount > 0 && (
              <span className="ml-2 bg-red-500 text-white text-xs font-bold px-2 py-0.5 rounded-full">
                {itemCount}
              </span>
            )}
          </button>
        </div>
      )}
    </header>
  );
}

