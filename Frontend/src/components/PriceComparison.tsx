import { X, MapPin, AlertCircle } from 'lucide-react';
import { useState } from 'react';

interface PriceComparisonProps {
  isOpen: boolean;
  onClose: () => void;
  productName?: string;
}

export default function PriceComparison({ isOpen, onClose, productName = 'Organik Süt 1L' }: PriceComparisonProps) {
  const [selectedMarket, setSelectedMarket] = useState(0);

  const prices = [
    { market: 'A101', price: 18.50, originalPrice: 22.0, discount: 16, distance: '0.5 km', rating: 4.5 },
    { market: 'Migros', price: 19.99, originalPrice: 25.0, discount: 20, distance: '1.2 km', rating: 4.8 },
    { market: 'Carrefour', price: 20.50, originalPrice: 26.0, discount: 21, distance: '2.1 km', rating: 4.6 },
    { market: 'Bim', price: 27.80, originalPrice: 21.0, discount: 15, distance: '0.8 km', rating: 4.3 },
    { market: 'Tesco', price: 21.00, originalPrice: 27.0, discount: 22, distance: '3.5 km', rating: 4.4 },
  ];
  const cheapest = prices.reduce((prev, curr) => (prev.price < curr.price ? prev : curr));
  if (!isOpen) return null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto animate-in fade-in zoom-in-95 duration-300">
        <div className="sticky top-0 bg-white border-b border-gray-100 px-6 py-4 flex items-center justify-between">
          <h2 className="text-2xl font-bold text-gray-900">{productName} Fiyatları</h2>
          <button
            onClick={onClose}
            className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
          >
            <X size={24} className="text-gray-600" />
          </button>
        </div>

        <div className="p-6 space-y-4">
          {prices.map((item, index) => (
            <div
              key={index}
              onClick={() => setSelectedMarket(index)}
              className={`p-4 rounded-xl border-2 cursor-pointer transition-all transform hover:scale-102 ${selectedMarket === index
                ? 'border-green-500 bg-green-50'
                : 'border-gray-200 hover:border-green-200 bg-white'
                }`}
            >
              <div className="flex items-center justify-between mb-3">
                <div className="flex-1">
                  <h3 className="font-bold text-lg text-gray-900">{item.market}</h3>
                  <div className="flex items-center gap-2 mt-1">
                    <MapPin size={16} className="text-gray-500" />
                    <span className="text-sm text-gray-600">{item.distance} uzaklıkta</span>
                  </div>
                </div>
                <div className="text-right">
                  <div className="flex items-center gap-2 justify-end mb-2">
                    <span className="text-3xl font-bold text-green-600">{item.price.toFixed(2)}₺</span>
                    <span className="text-sm text-gray-400 line-through">{item.originalPrice.toFixed(2)}₺</span>
                  </div>
                  <div className="bg-red-100 text-red-700 px-3 py-1 rounded-full text-sm font-semibold inline-block">
                    -{item.discount}%
                  </div>
                </div>
              </div>

              <div className="flex items-center gap-4 pt-3 border-t border-gray-100">
                <div className="flex items-center gap-1">
                  <span className="text-sm font-medium text-gray-700">Rating:</span>
                  <span className="text-sm font-bold text-gray-900">{item.rating}</span>
                  <span className="text-yellow-400 text-lg">★</span>
                </div>
                {/* BUTON FONKSİYONU EKLENECEK*/}
                <button className="ml-auto px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium text-sm transition-colors">
                  Add to List
                </button>
              </div>
            </div>
          ))}
        </div>

        <div className="bg-blue-50 border-t border-blue-100 px-6 py-4 flex items-gap-3">
          <AlertCircle size={20} className="text-blue-600 flex-shrink-0 mt-0.5" />
          <p className="text-sm text-blue-700">
            <strong>Tip:</strong> Cheapest Price on {cheapest.market} {cheapest.price.toFixed(2)}₺. Prices may vary in your city.
          </p>
        </div>

        <div className="bg-gray-50 border-t border-gray-100 px-6 py-4 flex gap-3">
          <button
            onClick={onClose}
            className="flex-1 px-4 py-3 bg-gray-200 hover:bg-gray-300 text-gray-900 rounded-lg font-medium transition-colors"
          >
            Close
          </button>
          {/* BUTON ADI DEĞİŞİLECEK FONKSİYON ?*/}
          <button className="flex-1 px-4 py-3 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium transition-colors">
            Okay
          </button>
        </div>
      </div>
    </div>
  );
}
