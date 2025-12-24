import { ShoppingBag, Bell, Menu, X, ShoppingBasket } from 'lucide-react';
import { useState } from 'react';

interface HeaderProps {
  onOpenList?: () => void;
  itemCount?: number;
}

export default function Header({ onOpenList, itemCount = 0 }: HeaderProps) {
  const [mobileMenuOpen, setMobileMenuOpen] = useState(false);

  return (
    <header className="bg-white border-b border-gray-100 sticky top-0 z-50 backdrop-blur-sm bg-opacity-95">
      <div className="max-w-7xl mx-auto px-4 py-4 flex items-center justify-between">
        <div
          onClick={() => window.scrollTo({ top: 0, behavior: 'smooth' })}
          className="flex items-center gap-3 cursor-pointer hover:opacity-80 transition-opacity"
        >
          <div className="w-10 h-10 bg-gradient-to-br from-green-500 to-emerald-600 rounded-lg flex items-center justify-center shadow-md">
            <ShoppingBag className="text-white" size={24} />
          </div>
          <div>
            <span className="text-xl font-bold text-gray-900">Market Comparison System</span>
            <p className="text-xs text-gray-500">Find and Compare the Best Prices</p>
          </div>
        </div>

        <nav className="hidden md:flex items-center gap-8">
          <a href="#" className="text-gray-600 hover:text-green-600 font-medium text-sm transition-colors relative group">
            Markets
            <span className="absolute bottom-0 left-0 w-0 h-0.5 bg-green-600 transition-all group-hover:w-full"></span>
          </a>
          <a href="#" className="text-gray-600 hover:text-green-600 font-medium text-sm transition-colors relative group">
            Products
            <span className="absolute bottom-0 left-0 w-0 h-0.5 bg-green-600 transition-all group-hover:w-full"></span>
          </a>
          <a href="#" className="text-gray-600 hover:text-green-600 font-medium text-sm transition-colors relative group">
            About
            <span className="absolute bottom-0 left-0 w-0 h-0.5 bg-green-600 transition-all group-hover:w-full"></span>
          </a>
          <a href="#" className="text-gray-600 hover:text-green-600 font-medium text-sm transition-colors relative group">
            Blog
            <span className="absolute bottom-0 left-0 w-0 h-0.5 bg-green-600 transition-all group-hover:w-full"></span>
          </a>
        </nav>

        <div className="flex items-center gap-2 md:gap-4">
          <button className="p-2 hover:bg-gray-100 rounded-lg transition-colors hover:scale-110 transform duration-200">
            <Bell size={20} className="text-gray-600" />
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
          <a href="#" className="block text-gray-600 hover:text-green-600 font-medium text-sm p-2">Markets</a>
          <a href="#" className="block text-gray-600 hover:text-green-600 font-medium text-sm p-2">Products</a>
          <a href="#" className="block text-gray-600 hover:text-green-600 font-medium text-sm p-2">Map</a>
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

