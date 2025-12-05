import { X, Trash2, ShoppingCart, Plus, Minus } from 'lucide-react';
import { CartItem } from '../types';

interface ProductListProps {
    isOpen: boolean;
    onClose: () => void;
    products: CartItem[];
    onRemove: (id: number) => void;
    onUpdateQuantity: (id: number, delta: number) => void;
    onCompare: () => void;
}

export default function ProductList({ isOpen, onClose, products, onRemove, onUpdateQuantity, onCompare }: ProductListProps) {
    if (!isOpen) return null;

    const totalPrice = products.reduce((sum, product) => sum + (product.price * product.quantity), 0);

    return (
        <div className="fixed inset-0 z-50 overflow-hidden">
            <div className="absolute inset-0 bg-black bg-opacity-50 transition-opacity" onClick={onClose} />

            <div className="absolute inset-y-0 right-0 max-w-md w-full bg-white shadow-xl transform transition-transform duration-300 ease-in-out flex flex-col">
                <div className="flex items-center justify-between p-6 border-b border-gray-100">
                    <div className="flex items-center gap-3">
                        <div className="bg-green-100 p-2 rounded-lg">
                            <ShoppingCart className="text-green-600" size={24} />
                        </div>
                        <h2 className="text-xl font-bold text-gray-900">Shopping List</h2>
                    </div>
                    <button
                        onClick={onClose}
                        className="p-2 hover:bg-gray-100 rounded-full transition-colors"
                    >
                        <X size={20} className="text-gray-500" />
                    </button>
                </div>

                <div className="flex-1 overflow-y-auto p-6">
                    {products.length === 0 ? (
                        <div className="h-full flex flex-col items-center justify-center text-center text-gray-500">
                            <ShoppingCart size={48} className="mb-4 text-gray-300" />
                            <p className="text-lg font-medium">Your list is empty</p>
                            <p className="text-sm">Start adding products to compare prices</p>
                        </div>
                    ) : (
                        <div className="space-y-4">
                            {products.map((product) => (
                                <div key={product.id} className="flex items-center justify-between p-4 bg-gray-50 rounded-xl border border-gray-100 hover:border-green-200 transition-colors">
                                    <div className="flex-1">
                                        <h3 className="font-medium text-gray-900">
                                            <span className="font-bold text-lg text-green-600">{product.brand}</span> {product.name} <span className="text-sm text-gray-500">{product.unit}</span>
                                        </h3>
                                        <div className="flex items-center gap-2 text-sm text-gray-500 mt-1">
                                            <span className="font-medium text-green-600">{product.market}</span>
                                            <span>•</span>
                                            <span>{product.price.toFixed(2)}₺</span>
                                        </div>
                                    </div>

                                    <div className="flex items-center gap-3">
                                        <div className="flex items-center bg-white rounded-lg border border-gray-200">
                                            <button
                                                onClick={() => onUpdateQuantity(product.id, -1)}
                                                className="p-1 hover:bg-gray-100 rounded-l-lg text-gray-600"
                                            >
                                                <Minus size={16} />
                                            </button>
                                            <span className="w-8 text-center font-medium text-sm">{product.quantity}</span>
                                            <button
                                                onClick={() => onUpdateQuantity(product.id, 1)}
                                                className="p-1 hover:bg-gray-100 rounded-r-lg text-gray-600"
                                            >
                                                <Plus size={16} />
                                            </button>
                                        </div>

                                        <button
                                            onClick={() => onRemove(product.id)}
                                            className="p-2 text-gray-400 hover:text-red-500 hover:bg-red-50 rounded-lg transition-all"
                                        >
                                            <Trash2 size={18} />
                                        </button>
                                    </div>
                                </div>
                            ))}
                        </div>
                    )}
                </div>

                {products.length > 0 && (
                    <div className="p-6 border-t border-gray-100 bg-gray-50">
                        <div className="flex items-center justify-between mb-4">
                            <span className="text-gray-600">Total Amount</span>
                            <span className="text-2xl font-bold text-green-600">{totalPrice.toFixed(2)}₺</span>
                        </div>
                        <button
                            onClick={onCompare}
                            className="w-full bg-green-600 hover:bg-green-700 text-white py-3 rounded-xl font-bold transition-colors shadow-lg shadow-green-200"
                        >
                            Compare Prices
                        </button>
                    </div>
                )}
            </div>
        </div>
    );
}
