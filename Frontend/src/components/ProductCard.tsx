import { Heart, MapPin, TrendingDown, Plus } from 'lucide-react';
import { useState } from 'react';
import { Product } from '../types';

interface ProductCardProps {
  product: Product;
  onAdd?: (product: Product) => void;
  onCompare?: (product: Product) => void;
}

export default function ProductCard({ product, onAdd, onCompare }: ProductCardProps) {
  const [isFavorite, setIsFavorite] = useState(false);

  const handleAdd = (e: React.MouseEvent) => {
    e.stopPropagation();
    if (onAdd) {
      onAdd(product);
    }
  };

  return (
    <div
      className="group bg-white rounded-xl border border-gray-100 overflow-hidden hover:border-green-200 hover:shadow-lg transition-all duration-300 cursor-default"
    >
      <div className="relative bg-gray-100 h-48 overflow-hidden">
        {product.image ? (
          <img
            src={product.image}
            alt={product.name}
            className="w-full h-full object-contain p-4"
            onError={(e) => {
              const target = e.target as HTMLImageElement;
              target.style.display = 'none';
              target.parentElement!.innerHTML = `<div class="w-full h-full bg-gradient-to-br from-gray-200 to-gray-300 flex items-center justify-center"><div class="text-center"><div class="text-4xl mb-2">📦</div><p class="text-sm text-gray-600">${product.name}</p></div></div>`;
            }}
          />
        ) : (
          <div className="w-full h-full bg-gradient-to-br from-gray-200 to-gray-300 flex items-center justify-center">
            <div className="text-center">
              <div className="text-4xl mb-2">📦</div>
              <p className="text-sm text-gray-600">{product.name}</p>
            </div>
          </div>
        )}

        <div className="absolute top-3 right-3 flex gap-2">
          <div className="bg-red-500 text-white px-3 py-1 rounded-full text-sm font-bold">
            -{product.discount}%
          </div>
          <button
            onClick={(e) => {
              e.stopPropagation();
              setIsFavorite(!isFavorite);
            }}
            className="bg-white rounded-full p-2 hover:bg-gray-100 transition-colors cursor-pointer"
          >
            <Heart
              size={18}
              className={isFavorite ? 'fill-red-500 text-red-500' : 'text-gray-400'}
            />
          </button>
        </div>

        {/* Market count badge */}
        {product.marketCount && product.marketCount > 1 && (
          <div className="absolute top-3 left-3">
            <div className="bg-blue-500 text-white px-2 py-1 rounded-full text-xs font-medium">
              {product.marketCount} markets
            </div>
          </div>
        )}
      </div>

      <div className="p-4">
        <h3 className="font-semibold text-gray-900 text-sm mb-3 line-clamp-3 min-h-14">
          <span className="font-bold text-lg text-green-600">{product.brand}</span> {product.name} <span className="text-sm text-gray-500">{product.unit}</span>
        </h3>

        <div className="flex items-center gap-2 mb-4">
          {product.marketCount && product.marketCount > 1 && (
            <span className="text-sm text-gray-500">from</span>
          )}
          <span className="text-2xl font-bold text-green-600">{product.price.toFixed(2)}₺</span>
          {product.oldPrice && <span className="text-sm text-gray-400 line-through">{product.oldPrice.toFixed(2)}₺</span>}
        </div>

        <div className="flex items-center gap-2 text-xs text-gray-600 mb-4 pb-4 border-b border-gray-100">
          <MapPin size={14} />
          <span className="font-medium">{product.market}</span>
          {product.marketCount && product.marketCount > 1 && (
            <span className="text-blue-500">+{product.marketCount - 1} more</span>
          )}
          <TrendingDown size={14} className="ml-auto text-green-600" />
        </div>

        <button
          onClick={handleAdd}
          className="w-full bg-green-600 hover:bg-green-700 text-white py-2 rounded-lg font-medium text-sm transition-colors flex items-center justify-center gap-2 cursor-pointer"
        >
          <Plus size={16} />
          Add to List
        </button>

        <button
          onClick={() => onCompare && onCompare(product)}
          className="w-full mt-2 bg-gray-100 hover:bg-gray-200 text-gray-900 py-2 rounded-lg font-medium text-sm transition-colors cursor-pointer"
        >
          Other Markets
        </button>
      </div>
    </div>
  );
}
