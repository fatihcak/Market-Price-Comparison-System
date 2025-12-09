import { X, MapPin, AlertCircle } from 'lucide-react';
import { useState, useEffect } from 'react';
import { api } from '../services/api';
import { PriceResponseDTO, Product } from '../types';

interface PriceComparisonProps {
  isOpen: boolean;
  onClose: () => void;
  productName?: string;
  productId?: number;
  productImage?: string;
  onAdd?: (product: Product) => void;
}

export default function PriceComparison({ isOpen, onClose, productName = 'Product', productId, productImage = '📦', onAdd }: PriceComparisonProps) {
  const [selectedMarket, setSelectedMarket] = useState(0);
  const [prices, setPrices] = useState<PriceResponseDTO[]>([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (isOpen && productId) {
      setLoading(true);
      api.getPricesByProduct(productId)
        .then(data => {
          setPrices(data);
          setLoading(false);
        })
        .catch(() => setLoading(false));
    }
  }, [isOpen, productId]);

  const handleAddToList = (priceItem: PriceResponseDTO) => {
    if (onAdd && productId) {
      const productToAdd: Product = {
        id: productId,
        name: productName,
        price: priceItem.price,
        oldPrice: null,
        market: priceItem.marketName,
        discount: 0,
        category: 'General',
        image: productImage,
        // Backend fields
        marketName: priceItem.marketName,
        districtName: priceItem.districtName,
        lastUpdated: priceItem.lastUpdated
      };
      onAdd(productToAdd);
      // Optional: Show feedback or close modal
      // onClose(); 
    }
  };

  if (!isOpen) return null;

  const cheapest = prices.length > 0
    ? prices.reduce((prev, curr) => (prev.price < curr.price ? prev : curr))
    : null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto animate-in fade-in zoom-in-95 duration-300">
        <div className="sticky top-0 bg-white border-b border-gray-100 px-6 py-4 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <span className="text-2xl">{productImage}</span>
            <h2 className="text-2xl font-bold text-gray-900">{productName} Prices </h2>
          </div>
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <X size={24} className="text-gray-600" />
          </button>
        </div>

        <div className="p-6 space-y-4">
          {loading ? (
            <div className="text-center py-8 text-gray-500">Loading prices...</div>
          ) : prices.length === 0 ? (
            <div className="text-center py-8 text-gray-500">No prices found for this product.</div>
          ) : (
            prices.map((item, index) => (
              <div
                key={item.id}
                onClick={() => setSelectedMarket(index)}
                className={`p-4 rounded-xl border-2 cursor-pointer transition-all transform hover:scale-102 ${selectedMarket === index
                  ? 'border-green-500 bg-green-50'
                  : 'border-gray-200 hover:border-green-200 bg-white'
                  }`}
              >
                <div className="flex items-center justify-between mb-3">
                  <div className="flex-1">
                    <h3 className="font-bold text-lg text-gray-900">{item.marketName}</h3>
                    <div className="flex items-center gap-2 mt-1">
                      <MapPin size={16} className="text-gray-500" />
                      <span className="text-sm text-gray-600">{item.districtName}</span>
                    </div>
                  </div>
                  <div className="text-right">
                    <div className="flex items-center gap-2 justify-end mb-2">
                      <span className="text-3xl font-bold text-green-600">{item.price.toFixed(2)}₺</span>
                    </div>
                    <div className="text-xs text-gray-400">
                      Updated: {new Date(item.lastUpdated).toLocaleDateString()}
                    </div>
                  </div>
                </div>

                <div className="flex items-center gap-4 pt-3 border-t border-gray-100">
                  <button
                    onClick={(e) => {
                      e.stopPropagation();
                      handleAddToList(item);
                    }}
                    className="ml-auto px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium text-sm transition-colors"
                  >
                    Add to List
                  </button>
                </div>
              </div>
            ))
          )}
        </div>

        {cheapest && (
          <div className="bg-blue-50 border-t border-blue-100 px-6 py-4 flex items-gap-3">
            <AlertCircle size={20} className="text-blue-600 flex-shrink-0 mt-0.5" />
            <p className="text-sm text-blue-700">
              <strong>Tip:</strong> Cheapest Price on {cheapest.marketName} {cheapest.price.toFixed(2)}₺.
            </p>
          </div>
        )}

        <div className="bg-gray-50 border-t border-gray-100 px-6 py-4 flex gap-3">
          <button
            onClick={onClose}
            className="flex-1 px-4 py-3 bg-gray-200 hover:bg-gray-300 text-gray-900 rounded-lg font-medium transition-colors"
          >
            Close
          </button>
          <button
            onClick={onClose}
            className="flex-1 px-4 py-3 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium transition-colors"
          >
            Okay
          </button>
        </div>
      </div>
    </div>
  );
}
