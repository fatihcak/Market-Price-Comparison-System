import { X, MapPin, ShoppingCart, Check, AlertTriangle } from 'lucide-react';
import { useState, useEffect } from 'react';
import { api } from '../services/api';
import { CartItem } from '../types';

interface BasketComparisonProps {
    isOpen: boolean;
    onClose: () => void;
    products: CartItem[];
}

interface MarketBasket {
    marketName: string;
    districtName: string;
    items: {
        productName: string;
        price: number;
        found: boolean;
    }[];
    totalPrice: number;
    missingCount: number;
}

export default function BasketComparison({ isOpen, onClose, products }: BasketComparisonProps) {
    const [marketBaskets, setMarketBaskets] = useState<MarketBasket[]>([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (isOpen && products.length > 0) {
            setLoading(true);

            const fetchPromises = products.map(product =>
                api.getPricesByProduct(product.id).then(prices => ({
                    productName: product.name,
                    quantity: product.quantity,
                    prices
                }))
            );

            Promise.all(fetchPromises)
                .then(results => {
                    // Group by Market
                    const marketMap = new Map<string, MarketBasket>();

                    results.forEach(({ productName, quantity, prices }) => {
                        prices.forEach(priceDto => {
                            const key = `${priceDto.marketName}-${priceDto.districtName}`;

                            if (!marketMap.has(key)) {
                                marketMap.set(key, {
                                    marketName: priceDto.marketName,
                                    districtName: priceDto.districtName,
                                    items: [],
                                    totalPrice: 0,
                                    missingCount: 0
                                });
                            }

                            const basket = marketMap.get(key)!;
                            basket.items.push({
                                productName: `${productName} (x${quantity})`,
                                price: priceDto.price * quantity,
                                found: true
                            });
                            basket.totalPrice += priceDto.price * quantity;
                        });
                    });

                    const finalBaskets: MarketBasket[] = [];

                    marketMap.forEach(basket => {
                        let currentBasketMissingCount = 0;
                        const currentBasketItems: typeof basket.items = [];
                        let currentBasketTotal = 0;

                        results.forEach(({ productName, quantity, prices }) => {

                            const priceInMarket = prices.find(p =>
                                p.marketName === basket.marketName && p.districtName === basket.districtName
                            );

                            if (priceInMarket) {
                                currentBasketItems.push({
                                    productName: `${productName} (x${quantity})`,
                                    price: priceInMarket.price * quantity,
                                    found: true
                                });
                                currentBasketTotal += priceInMarket.price * quantity;
                            } else {
                                currentBasketItems.push({
                                    productName: `${productName} (x${quantity})`,
                                    price: 0,
                                    found: false
                                });
                                currentBasketMissingCount++;
                            }
                        });

                        finalBaskets.push({
                            ...basket,
                            items: currentBasketItems,
                            totalPrice: currentBasketTotal,
                            missingCount: currentBasketMissingCount
                        });
                    });


                    finalBaskets.sort((a, b) => {
                        if (a.missingCount === 0 && b.missingCount > 0) return -1;
                        if (a.missingCount > 0 && b.missingCount === 0) return 1;
                        if (a.missingCount === 0 && b.missingCount === 0) return a.totalPrice - b.totalPrice;
                        return 0;
                    });

                    setMarketBaskets(finalBaskets);
                    setLoading(false);
                })
                .catch(err => {
                    console.error("Error comparing prices:", err);
                    setLoading(false);
                });
        }
    }, [isOpen, products]);

    if (!isOpen) return null;

    const completeBaskets = marketBaskets.filter(b => b.missingCount === 0);
    const partialBaskets = marketBaskets.filter(b => b.missingCount > 0);
    const cheapestBasket = completeBaskets.length > 0 ? completeBaskets[0] : null;

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4">
            <div className="bg-white rounded-2xl max-w-4xl w-full max-h-[90vh] overflow-y-auto animate-in fade-in zoom-in-95 duration-300 flex flex-col">
                <div className="sticky top-0 bg-white border-b border-gray-100 px-6 py-4 flex items-center justify-between z-10">
                    <div className="flex items-center gap-3">
                        <div className="bg-green-100 p-2 rounded-lg">
                            <ShoppingCart className="text-green-600" size={24} />
                        </div>
                        <div>
                            <h2 className="text-2xl font-bold text-gray-900">Basket Comparison</h2>
                            <p className="text-sm text-gray-500">Comparing {products.length} items across markets</p>
                        </div>
                    </div>
                    <button
                        onClick={onClose}
                        className="p-2 hover:bg-gray-100 rounded-lg transition-colors"
                    >
                        <X size={24} className="text-gray-600" />
                    </button>
                </div>

                <div className="p-6 space-y-8 overflow-y-auto">
                    {loading ? (
                        <div className="text-center py-12">
                            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-green-600 mx-auto mb-4"></div>
                            <p className="text-gray-500">Calculating best prices...</p>
                        </div>
                    ) : (
                        <>
                            {/* Complete Baskets Section */}
                            <section>
                                <h3 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2">
                                    <Check className="text-green-600" size={20} />
                                    Complete Baskets
                                    <span className="text-sm font-normal text-gray-500 ml-2">
                                        (Markets with all items)
                                    </span>
                                </h3>

                                {completeBaskets.length === 0 ? (
                                    <div className="text-center py-8 bg-gray-50 rounded-xl border border-dashed border-gray-300">
                                        <p className="text-gray-500">No market has all items in your list.</p>
                                    </div>
                                ) : (
                                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                        {completeBaskets.map((basket, index) => (
                                            <div
                                                key={`${basket.marketName}-${index}`}
                                                className={`border-2 rounded-xl p-4 transition-all ${basket === cheapestBasket
                                                    ? 'border-green-500 bg-green-50 shadow-md'
                                                    : 'border-gray-100 hover:border-green-200 bg-white'
                                                    }`}
                                            >
                                                <div className="flex justify-between items-start mb-4">
                                                    <div>
                                                        <h4 className="font-bold text-lg text-gray-900">{basket.marketName}</h4>
                                                        <div className="flex items-center gap-1 text-sm text-gray-500">
                                                            <MapPin size={14} />
                                                            {basket.districtName}
                                                        </div>
                                                    </div>
                                                    <div className="text-right">
                                                        <span className="block text-2xl font-bold text-green-600">
                                                            {basket.totalPrice.toFixed(2)}₺
                                                        </span>
                                                        {basket === cheapestBasket && (
                                                            <span className="inline-block px-2 py-1 bg-green-100 text-green-700 text-xs font-bold rounded-full mt-1">
                                                                Best Price
                                                            </span>
                                                        )}
                                                    </div>
                                                </div>

                                                <div className="space-y-2 border-t border-gray-200 pt-3">
                                                    {basket.items.map((item, i) => (
                                                        <div key={i} className="flex justify-between text-sm">
                                                            <span className="text-gray-700">{item.productName}</span>
                                                            <span className="font-medium text-gray-900">{item.price.toFixed(2)}₺</span>
                                                        </div>
                                                    ))}
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                )}
                            </section>

                            {/* Partial Baskets Section */}
                            {partialBaskets.length > 0 && (
                                <section>
                                    <h3 className="text-lg font-bold text-gray-900 mb-4 flex items-center gap-2 mt-8">
                                        <AlertTriangle className="text-orange-500" size={20} />
                                        Partial Baskets
                                        <span className="text-sm font-normal text-gray-500 ml-2">
                                            (Markets missing some items)
                                        </span>
                                    </h3>

                                    <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                                        {partialBaskets.map((basket, index) => (
                                            <div
                                                key={`partial-${basket.marketName}-${index}`}
                                                className="border border-gray-200 rounded-xl p-4 bg-gray-50 opacity-90 hover:opacity-100 transition-opacity"
                                            >
                                                <div className="flex justify-between items-start mb-3">
                                                    <div>
                                                        <h4 className="font-bold text-gray-900">{basket.marketName}</h4>
                                                        <div className="flex items-center gap-1 text-sm text-gray-500">
                                                            <MapPin size={14} />
                                                            {basket.districtName}
                                                        </div>
                                                    </div>
                                                    <span className="px-2 py-1 bg-orange-100 text-orange-700 text-xs font-bold rounded-full">
                                                        Missing {basket.missingCount} items
                                                    </span>
                                                </div>

                                                <div className="space-y-2 border-t border-gray-200 pt-3">
                                                    {basket.items.map((item, i) => (
                                                        <div key={i} className="flex justify-between text-sm">
                                                            <span className={`${item.found ? 'text-gray-700' : 'text-gray-400 line-through'}`}>
                                                                {item.productName}
                                                            </span>
                                                            {item.found ? (
                                                                <span className="font-medium text-gray-900">{item.price.toFixed(2)}₺</span>
                                                            ) : (
                                                                <span className="text-xs text-red-500 italic">Not available</span>
                                                            )}
                                                        </div>
                                                    ))}
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                </section>
                            )}
                        </>
                    )}
                </div>

                <div className="p-6 border-t border-gray-100 bg-gray-50 flex justify-end">
                    <button
                        onClick={onClose}
                        className="px-6 py-2 bg-gray-200 hover:bg-gray-300 text-gray-900 rounded-lg font-medium transition-colors"
                    >
                        Close
                    </button>
                </div>
            </div>
        </div>
    );
}
