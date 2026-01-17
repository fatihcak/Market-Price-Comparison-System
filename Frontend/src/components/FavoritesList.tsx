import { X, Heart, MapPin, Plus } from 'lucide-react';
import { Product } from '../types';

interface FavoritesListProps {
    isOpen: boolean;
    onClose: () => void;
    favorites: Product[];
    onRemove: (id: number) => void;
    onAddToList: (product: Product) => void;
}

export default function FavoritesList({ isOpen, onClose, favorites, onRemove, onAddToList }: FavoritesListProps) {
    if (!isOpen) return null;

    return (
        <div className="fixed inset-0 z-50 overflow-hidden">
            <div className="absolute inset-0 bg-black bg-opacity-50 transition-opacity" onClick={onClose} />

            <div className="absolute inset-y-0 right-0 max-w-md w-full bg-white shadow-xl transform transition-transform duration-300 ease-in-out flex flex-col">
                <div className="flex items-center justify-between p-6 border-b border-gray-100">
                    <div className="flex items-center gap-3">
                        <div className="bg-red-100 p-2 rounded-lg">
                            <Heart className="text-red-500 fill-red-500" size={24} />
                        </div>
                        <h2 className="text-xl font-bold text-gray-900">Favorites</h2>
                    </div>
                    <button
                        onClick={onClose}
                        className="p-2 hover:bg-gray-100 rounded-full transition-colors"
                    >
                        <X size={20} className="text-gray-500" />
                    </button>
                </div>

                <div className="flex-1 overflow-y-auto p-6">
                    {favorites.length === 0 ? (
                        <div className="h-full flex flex-col items-center justify-center text-center text-gray-500">
                            <Heart size={48} className="mb-4 text-gray-300" />
                            <p className="text-lg font-medium">No favorites yet</p>
                            <p className="text-sm">Click the heart icon on products to add them here</p>
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {favorites.map((product) => (
                                <div key={product.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-xl border border-gray-100 hover:border-red-200 transition-colors">
                                    <div className="flex-1">
                                        <h3 className="font-medium text-gray-900 text-sm line-clamp-2">
                                            {product.name}
                                        </h3>
                                        <div className="flex items-center gap-2 text-sm text-gray-500 mt-1">
                                            <span className="font-bold text-green-600">{product.price.toFixed(2)}₺</span>
                                            <span>•</span>
                                            <MapPin size={12} />
                                            <span>{product.market}</span>
                                        </div>
                                    </div>

                                    <div className="flex items-center gap-2">
                                        <button
                                            onClick={() => onAddToList(product)}
                                            className="p-2 text-green-600 hover:bg-green-50 rounded-lg transition-all"
                                            title="Add to shopping list"
                                        >
                                            <Plus size={18} />
                                            List
                                        </button>
                                        <button
                                            onClick={() => onRemove(product.id)}
                                            className="p-2 text-red-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all"
                                            title="Remove from favorites"
                                        >
                                            <Heart size={18} className="fill-current" />
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>
            </div>
        </div>
    );
}
