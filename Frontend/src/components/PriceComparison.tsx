import { X, MapPin, AlertCircle, ChevronDown, ChevronUp } from 'lucide-react';
import { useState, useEffect, useMemo } from 'react';
import { api } from '../services/api';
import { PriceResponseDTO, Product } from '../types';

interface PriceComparisonProps {
  isOpen: boolean;
  onClose: () => void;
  productName?: string;
  productId?: number;
  productImage?: string;
  variantIds?: number[];
  onAdd?: (product: Product) => void;
}

// Group prices by market, then by price within each market
interface GroupedPrice {
  price: number;
  districts: { id: number; districtName: string; lastUpdated: string }[];
}

interface MarketGroup {
  marketId: number;
  marketName: string;
  priceGroups: GroupedPrice[];
  lowestPrice: number;
}

export default function PriceComparison({ isOpen, onClose, productName = 'Product', productId, productImage = '📦', variantIds, onAdd }: PriceComparisonProps) {
  const [prices, setPrices] = useState<PriceResponseDTO[]>([]);
  const [loading, setLoading] = useState(false);
  const [expandedMarkets, setExpandedMarkets] = useState<Set<number>>(new Set());

  useEffect(() => {
    if (isOpen && (variantIds?.length || productId)) {
      setLoading(true);

      // Use variantIds if available (for consolidated products), otherwise use single productId
      const fetchPrices = variantIds && variantIds.length > 0
        ? api.getPricesByProductIds(variantIds)
        : api.getPricesByProduct(productId!);

      fetchPrices
        .then(data => {
          setPrices(data);
          // Expand all markets by default
          const marketIds = [...new Set(data.map(p => p.marketId))];
          setExpandedMarkets(new Set(marketIds));
          setLoading(false);
        })
        .catch(() => setLoading(false));
    }
  }, [isOpen, productId, variantIds]);

  // Group prices by market, then group districts by same price
  const marketGroups = useMemo<MarketGroup[]>(() => {
    const marketMap = new Map<number, PriceResponseDTO[]>();

    // Group by market
    prices.forEach(price => {
      const existing = marketMap.get(price.marketId) || [];
      marketMap.set(price.marketId, [...existing, price]);
    });

    // For each market, group by price
    const groups: MarketGroup[] = [];
    marketMap.forEach((marketPrices, marketId) => {
      const priceMap = new Map<number, GroupedPrice>();

      marketPrices.forEach(p => {
        const existing = priceMap.get(p.price);
        if (existing) {
          existing.districts.push({
            id: p.id,
            districtName: p.districtName,
            lastUpdated: p.lastUpdated
          });
        } else {
          priceMap.set(p.price, {
            price: p.price,
            districts: [{
              id: p.id,
              districtName: p.districtName,
              lastUpdated: p.lastUpdated
            }]
          });
        }
      });

      const priceGroups = Array.from(priceMap.values()).sort((a, b) => a.price - b.price);
      const lowestPrice = priceGroups[0]?.price ?? 0;

      groups.push({
        marketId,
        marketName: marketPrices[0].marketName,
        priceGroups,
        lowestPrice
      });
    });

    // Sort markets by their lowest price
    return groups.sort((a, b) => a.lowestPrice - b.lowestPrice);
  }, [prices]);

  const toggleMarket = (marketId: number) => {
    setExpandedMarkets(prev => {
      const newSet = new Set(prev);
      if (newSet.has(marketId)) {
        newSet.delete(marketId);
      } else {
        newSet.add(marketId);
      }
      return newSet;
    });
  };

  const handleAddToList = (marketName: string, price: number, districtName: string) => {
    if (onAdd && productId) {
      const productToAdd: Product = {
        id: productId,
        name: productName,
        price: price,
        oldPrice: null,
        market: marketName,
        discount: 0,
        category: 'General',
        image: productImage,
        marketName: marketName,
        districtName: districtName,
      };
      onAdd(productToAdd);
    }
  };

  if (!isOpen) return null;

  const cheapest = marketGroups.length > 0 ? marketGroups[0] : null;

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 z-50 flex items-center justify-center p-4">
      <div className="bg-white rounded-2xl max-w-2xl w-full max-h-[90vh] overflow-y-auto animate-in fade-in zoom-in-95 duration-300">
        <div className="sticky top-0 bg-white border-b border-gray-100 px-6 py-4 flex items-center justify-between z-10">
          <div className="flex items-center gap-3">
            <span className="text-2xl">{productImage}</span>
            <h2 className="text-2xl font-bold text-gray-900">{productName} Prices</h2>
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
          ) : marketGroups.length === 0 ? (
            <div className="text-center py-8 text-gray-500">No prices found for this product.</div>
          ) : (
            marketGroups.map((market) => (
              <div
                key={market.marketId}
                className="rounded-xl border-2 border-gray-200 overflow-hidden transition-all"
              >
                {/* Market Header */}
                <div
                  onClick={() => toggleMarket(market.marketId)}
                  className="flex items-center justify-between p-4 bg-gray-50 cursor-pointer hover:bg-gray-100 transition-colors"
                >
                  <div className="flex items-center gap-3">
                    <h3 className="font-bold text-lg text-gray-900">{market.marketName}</h3>
                    <span className="text-sm text-gray-500">
                      ({market.priceGroups.reduce((sum, g) => sum + g.districts.length, 0)} locations)
                    </span>
                  </div>
                  <div className="flex items-center gap-3">
                    <span className="text-xl font-bold text-green-600">
                      from {market.lowestPrice.toFixed(2)}₺
                    </span>
                    {expandedMarkets.has(market.marketId) ? (
                      <ChevronUp size={20} className="text-gray-500" />
                    ) : (
                      <ChevronDown size={20} className="text-gray-500" />
                    )}
                  </div>
                </div>

                {/* Expanded Price Groups */}
                {expandedMarkets.has(market.marketId) && (
                  <div className="divide-y divide-gray-100">
                    {market.priceGroups.map((priceGroup, idx) => (
                      <div key={idx} className="p-4 bg-white">
                        <div className="flex items-start justify-between">
                          <div className="flex-1">
                            <div className="flex items-center gap-2 mb-2">
                              <span className="text-2xl font-bold text-green-600">
                                {priceGroup.price.toFixed(2)}₺
                              </span>
                              {priceGroup.price === market.lowestPrice && (
                                <span className="px-2 py-0.5 bg-green-100 text-green-700 text-xs font-medium rounded-full">
                                  Lowest
                                </span>
                              )}
                            </div>
                            <div className="flex flex-wrap gap-2">
                              {priceGroup.districts.map((district) => (
                                <div
                                  key={district.id}
                                  className="flex items-center gap-1 px-3 py-1 bg-gray-100 rounded-full text-sm"
                                >
                                  <MapPin size={12} className="text-gray-500" />
                                  <span className="text-gray-700">{district.districtName}</span>
                                </div>
                              ))}
                            </div>
                          </div>
                          <button
                            onClick={() => handleAddToList(
                              market.marketName,
                              priceGroup.price,
                              priceGroup.districts[0].districtName
                            )}
                            className="ml-4 px-4 py-2 bg-green-600 hover:bg-green-700 text-white rounded-lg font-medium text-sm transition-colors whitespace-nowrap"
                          >
                            Add to List
                          </button>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            ))
          )}
        </div>

        {cheapest && (
          <div className="bg-blue-50 border-t border-blue-100 px-6 py-4 flex items-center gap-3">
            <AlertCircle size={20} className="text-blue-600 flex-shrink-0" />
            <p className="text-sm text-blue-700">
              <strong>Tip:</strong> Cheapest price at {cheapest.marketName} - {cheapest.lowestPrice.toFixed(2)}₺
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
