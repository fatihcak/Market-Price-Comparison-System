import { MapPin, ExternalLink } from 'lucide-react';
import { Link } from 'react-router-dom';
import { Map } from 'lucide-react'

const MARKETS = [
    {
        id: 1,
        name: 'Sarper Market',
        description: 'Fresh local produce and daily essentials in Cyprus.',
        image: 'https://cyprus-faq.com/site/assets/files/8333/image.webp',
        address: 'Girne Mahallesi, Lefkoşa',
        color: 'from-orange-500 to-red-600',
        openHours: '08:30 - 23:00',
        url: 'https://www.sarpermarket.com'
    },
    {
        id: 2,
        name: 'Kıbrıs Sanal Market',
        description: 'Online grocery market with fresh produce and daily essentials in Cyprus',
        image: 'https://images.unsplash.com/photo-1578916171728-46686eac8d58?auto=format&fit=crop&q=80&w=1000',
        address: 'Online / Distribution Center: Gönyeli',
        color: 'from-blue-500 to-indigo-600',
        openHours: '10:00 - 22:00',
        url: 'https://www.kibrissanalmarket.com/'
    },
    {
        id: 3,
        name: 'Ünimar Market',
        description: 'Fresh local produce and daily essentials in Cyprus.',
        image: 'https://lh3.googleusercontent.com/p/AF1QipMNRGpWvyj0ulwydewe5JGxyhJRTs6yNXBMQeYZ=s680-w680-h510-rw',
        address: 'Girne Mahallesi, Lefkoşa',
        color: 'from-orange-500 to-red-600',
        openHours: '08:30 - 22:00',
        url: 'https://www.unimarmarket.com/'
    },
    {
        id: 4,
        name: 'Starling Market',
        description: 'Fresh local produce and daily essentials in Cyprus.',
        image: 'https://lh3.googleusercontent.com/gps-cs-s/AG0ilSz9BTeDk837pPTQzJoxO36AQlGSNAF7OB6rcOVunJf7yFna6xkZaGvo3Z3sUYriQFOJGxpVOXDCXiw2-RWiuGLsrdaYTU3uJkXj_7T0Qajt3sRHT8D2iyEScGDrnNXIQS0Db6VbCA=s680-w680-h510-rw',
        address: 'Girne Mahallesi, Lefkoşa',
        color: 'from-orange-500 to-red-600',
        openHours: '08:30 - 22:00',
        url: 'https://www.starlingsupermarket.com/'
    }
];

export default function Markets() {
    return (
        <div className="min-h-screen bg-gray-50 pt-20 pb-12">
            <div className="max-w-7xl mx-auto px-4">
                {/* Header Section */}
                <div className="text-center mb-16 space-y-4">
                    <h1 className="text-4xl md:text-5xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-gray-900 to-gray-700">
                        Markets
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
                            className="group bg-white rounded-3xl overflow-hidden shadow-sm transition-all duration-500 border border-gray-100 flex flex-col"
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
                                        <span className="px-4 py-1.5 bg-green-50 text-green-700 rounded-lg text-sm font-medium border border-green-100">
                                            Open: {market.openHours}
                                        </span>
                                    </div>
                                </div>

                                <div className="mt-8 flex gap-4">
                                    <button onClick={() => window.open(market.url, '_blank')} className="flex-1 flex items-center justify-center gap-2 bg-gray-900 text-white py-4 rounded-xl font-bold transition-transform active:scale-95 group/btn">
                                        Visit Market
                                        <ExternalLink size={18} className="group-hover/btn:translate-x-1 transition-transform" />
                                    </button>
                                    <Link to="/map">
                                        <button className="flex-1 flex items-center justify-center gap-2 bg-gray-900 text-white py-4 px-5 rounded-xl font-bold transition-transform active:scale-95 group/btn">
                                            View on Map
                                            <Map size={18} className="group-hover/btn:translate-x-1 transition-transform" />
                                        </button>
                                    </Link>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>

                {/* Coming Soon Section */}
                <div className="mt-16">
                    <h2 className="text-2xl font-bold text-gray-800 mb-6 text-center">Coming Soon</h2>
                    <div className="grid grid-cols-1 lg:grid-cols-2 gap-10">
                        {/*  Market 1 */}
                        <div className="group bg-white rounded-3xl overflow-hidden shadow-sm border-2 border-dashed border-gray-300 flex flex-col">
                            <div className="relative h-64 bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center">
                                <div className="text-center">
                                    <div className="w-20 h-20 bg-gray-300 rounded-full mx-auto mb-4 flex items-center justify-center">

                                    </div>
                                    <span className="text-gray-500 font-medium">New Market</span>
                                </div>
                            </div>
                            <div className="p-8 flex-1 flex flex-col justify-center items-center">
                                <h3 className="text-xl font-bold text-gray-400 mb-2">Coming Soon</h3>
                                <p className="text-gray-400 text-center">
                                    Coming Soon
                                </p>
                            </div>
                        </div>

                        {/*  Market 2 */}
                        <div className="group bg-white rounded-3xl overflow-hidden shadow-sm border-2 border-dashed border-gray-300 flex flex-col">
                            <div className="relative h-64 bg-gradient-to-br from-gray-100 to-gray-200 flex items-center justify-center">
                                <div className="text-center">
                                    <div className="w-20 h-20 bg-gray-300 rounded-full mx-auto mb-4 flex items-center justify-center">

                                    </div>
                                    <span className="text-gray-500 font-medium">New Market</span>
                                </div>
                            </div>
                            <div className="p-8 flex-1 flex flex-col justify-center items-center">
                                <h3 className="text-xl font-bold text-gray-400 mb-2">Coming Soon</h3>
                                <p className="text-gray-400 text-center">
                                    Coming Soon
                                </p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}
