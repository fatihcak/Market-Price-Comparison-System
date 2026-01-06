import { MapPin, Navigation, ExternalLink } from 'lucide-react';

const MARKETS = [
    {
        id: 1,
        name: 'Sarper Market',
        description: 'Fresh local produce and daily essentials at competitive prices. Known for their quality meats and organic vegetable selection.',
        image: 'https://images.unsplash.com/photo-1542838132-92c53300491e?auto=format&fit=crop&q=80&w=1000',
        address: 'Girne Mahallesi, Lefkoşa',
        color: 'from-orange-500 to-red-600',
        rating: 4.5,
        openHours: '08:00 - 22:00'
    },
    {
        id: 2,
        name: 'Kıbrıs Sanal Market',
        description: 'Your premier online grocery destination. Largest selection of international and local brands delivered straight to your door.',
        image: 'https://images.unsplash.com/photo-1578916171728-46686eac8d58?auto=format&fit=crop&q=80&w=1000',
        address: 'Online / Distribution Center: Gönyeli',
        color: 'from-blue-500 to-indigo-600',
        rating: 4.8,
        openHours: '24/7'
    }
];

export default function Markets() {
    return (
        <div className="min-h-screen bg-gray-50 pt-20 pb-12">
            <div className="max-w-7xl mx-auto px-4">
                {/* Header Section */}
                <div className="text-center mb-16 space-y-4">
                    <h1 className="text-4xl md:text-5xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-gray-900 to-gray-700">
                        Our Partner Markets
                    </h1>
                    <p className="text-lg text-gray-600 max-w-2xl mx-auto">
                        Compare prices across top local markets. We verify prices daily to ensure you get the best deals available in Cyprus.
                    </p>
                </div>

                {/* Markets Grid */}
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-10">
                    {MARKETS.map((market) => (
                        <div
                            key={market.id}
                            className="group bg-white rounded-3xl overflow-hidden shadow-sm hover:shadow-2xl transition-all duration-500 border border-gray-100 flex flex-col"
                        >
                            {/* Image Section */}
                            <div className="relative h-64 overflow-hidden">
                                <div className={`absolute inset-0 bg-gradient-to-t ${market.color} opacity-40 mix-blend-multiply transition-opacity group-hover:opacity-30`} />
                                <img
                                    src={market.image}
                                    alt={market.name}
                                    className="w-full h-full object-cover transform group-hover:scale-110 transition-transform duration-700"
                                />
                                <div className="absolute bottom-0 left-0 p-8 w-full bg-gradient-to-t from-black/80 to-transparent">
                                    <div className="flex justify-between items-end">
                                        <div>
                                            <h2 className="text-3xl font-bold text-white mb-2">{market.name}</h2>
                                            <div className="flex items-center gap-4 text-white/90 text-sm font-medium">
                                                <span className="flex items-center gap-1 bg-white/20 backdrop-blur-md px-3 py-1 rounded-full">
                                                    <MapPin size={14} /> {market.address}
                                                </span>
                                                <span className="flex items-center gap-1 bg-green-500/80 backdrop-blur-md px-3 py-1 rounded-full">
                                                    ★ {market.rating}
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            {/* Content Section */}
                            <div className="p-8 flex-1 flex flex-col justify-between relative">
                                <div className="space-y-6">
                                    <p className="text-gray-600 leading-relaxed text-lg">
                                        {market.description}
                                    </p>

                                    <div className="flex flex-wrap gap-3">
                                        <span className="px-4 py-1.5 bg-gray-50 text-gray-600 rounded-lg text-sm font-medium border border-gray-200">
                                            Daily Fresh
                                        </span>
                                        <span className="px-4 py-1.5 bg-gray-50 text-gray-600 rounded-lg text-sm font-medium border border-gray-200">
                                            Best Prices
                                        </span>
                                        <span className="px-4 py-1.5 bg-green-50 text-green-700 rounded-lg text-sm font-medium border border-green-100">
                                            Open: {market.openHours}
                                        </span>
                                    </div>
                                </div>

                                <div className="mt-8 flex gap-4">
                                    <button className="flex-1 flex items-center justify-center gap-2 bg-gray-900 text-white py-4 rounded-xl font-bold transition-transform active:scale-95 group/btn">
                                        Visit Market
                                        <ExternalLink size={18} className="group-hover/btn:translate-x-1 transition-transform" />
                                    </button>
                                    <button className="flex-1 flex items-center justify-center gap-2 bg-gray-100 text-gray-900 py-4 rounded-xl font-bold hover:bg-gray-200 transition-colors active:scale-95">
                                        <Navigation size={18} />
                                        Get Directions
                                    </button>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}
