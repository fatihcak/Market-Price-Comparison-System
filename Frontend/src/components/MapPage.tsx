import { useEffect, useRef } from 'react';
import L from 'leaflet';
import 'leaflet/dist/leaflet.css';
import { MapPin, Clock, Store, ExternalLink } from 'lucide-react';

// Fix Leaflet default icon issue
import markerIcon2x from 'leaflet/dist/images/marker-icon-2x.png';
import markerIcon from 'leaflet/dist/images/marker-icon.png';
import markerShadow from 'leaflet/dist/images/marker-shadow.png';

// eslint-disable-next-line @typescript-eslint/no-explicit-any
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
    iconUrl: markerIcon,
    iconRetinaUrl: markerIcon2x,
    shadowUrl: markerShadow,
});

// Market data with coordinates
const MARKET_LOCATIONS = [
    // Sarper Market - Lefkoşa
    {
        id: 1,
        name: 'Sarper Market',
        lat: 35.135910749660084,
        lng: 33.92135769838634,
        address: 'Gönyeli, Lefkoşa',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 23:00',
        website: 'https://sarpermarket.com',
        color: '#f97316' // Orange
    },
    // Kıbrıs Sanal Market - Lefkoşa
    {
        id: 2,
        name: 'Kıbrıs Sanal Market',
        lat: 35.190020601688914,
        lng: 33.35477913694478,
        address: 'Gönyeli, Lefkoşa',
        description: 'Online grocery market.',
        openHours: '10:00 - 22:00',
        website: 'https://kibrissanalmarket.com',
        color: '#3b82f6' // Blue
    },
    // Ünimar Market - Mağusa (3 locations) - GREEN
    {
        id: 3,
        name: 'Ünimar Market - Maraş',
        lat: 35.115651090630934,
        lng: 33.9404434327198,
        address: 'Salamis Yolu, Mağusa',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://unimarmarket.com',
        color: '#22c55e' // Green
    },
    {
        id: 4,
        name: 'Ünimar Market - CityMall',
        lat: 35.12702413342683,
        lng: 33.920616544814855,
        address: 'CityMall, Mağusa',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://unimarmarket.com',
        color: '#22c55e' // Green
    },
    {
        id: 5,
        name: 'Ünimar Market - Sakarya',
        lat: 35.142703146954176,
        lng: 33.904422643454524,
        address: 'Baykal, Mağusa',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://unimarmarket.com',
        color: '#22c55e' // Green
    },
    // Starling Market - Girne (3 locations) - PURPLE
    {
        id: 6,
        name: 'Starling Market - Girne Merkez',
        lat: 35.33778,
        lng: 33.31833,
        address: 'Merkez, Girne',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://starlingsupermarket.com',
        color: '#8b5cf6' // Purple
    },
    {
        id: 7,
        name: 'Starling Market - Karakum',
        lat: 35.34194,
        lng: 33.29500,
        address: 'Karakum, Girne',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://starlingsupermarket.com',
        color: '#8b5cf6' // Purple
    },
    {
        id: 8,
        name: 'Starling Market - Alsancak',
        lat: 35.33500,
        lng: 33.23361,
        address: 'Alsancak, Girne',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://starlingsupermarket.com',
        color: '#8b5cf6' // Purple
    },
    {
        id: 9,
        name: 'Sarper Market - Lefkoşa',
        lat: 35.203184365230086,
        lng: 33.34406071832413,
        address: 'Lefkoşa',
        description: 'Fresh local produce and daily essentials.',
        openHours: '08:30 - 22:00',
        website: 'https://starlingsupermarket.com',
        color: '#f97316' // Purple
    }
];

// Unique markets for the card list (one card per market chain)
const UNIQUE_MARKETS = [
    {
        id: 1,
        name: 'Sarper Market',
        address: 'Lefkoşa & Gazimağusa',
        openHours: '08:30 - 23:00',
        website: 'https://sarpermarket.com',
        color: '#f97316'
    },
    {
        id: 2,
        name: 'Kıbrıs Sanal Market',
        address: 'Lefkoşa',
        openHours: '10:00 - 22:00',
        website: 'https://kibrissanalmarket.com',
        color: '#3b82f6'
    },
    {
        id: 3,
        name: 'Ünimar Market',
        address: 'Gazimağusa',
        openHours: '08:30 - 22:00',
        website: 'https://unimarmarket.com',
        color: '#22c55e'
    },
    {
        id: 4,
        name: 'Starling Market',
        address: 'Girne',
        openHours: '08:30 - 22:00',
        website: 'https://starlingsupermarket.com',
        color: '#8b5cf6'
    }
];

export default function MapPage() {
    const mapRef = useRef<L.Map | null>(null);
    const mapContainerRef = useRef<HTMLDivElement>(null);

    useEffect(() => {
        // Only initialize once
        if (mapRef.current || !mapContainerRef.current) return;

        // Center of Northern Cyprus
        const cyprusCenter: L.LatLngExpression = [35.17, 33.6];

        // Create map
        const map = L.map(mapContainerRef.current).setView(cyprusCenter, 10);
        mapRef.current = map;

        // Add tile layer
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(map);

        // Add markers for each market
        MARKET_LOCATIONS.forEach(market => {
            const customIcon = L.divIcon({
                className: 'custom-marker',
                html: `
                    <div style="
                        background-color: ${market.color};
                        width: 30px;
                        height: 30px;
                        border-radius: 50% 50% 50% 0;
                        transform: rotate(-45deg);
                        border: 3px solid white;
                        box-shadow: 0 2px 5px rgba(0,0,0,0.3);
                    "></div>
                `,
                iconSize: [30, 30],
                iconAnchor: [15, 30],
                popupAnchor: [0, -30]
            });

            const marker = L.marker([market.lat, market.lng], { icon: customIcon }).addTo(map);

            marker.bindPopup(`
                <div style="min-width: 200px; font-family: system-ui, sans-serif;">
                    <h3 style="font-weight: bold; font-size: 16px; margin: 0 0 8px 0; color: #111;">
                        ${market.name}
                    </h3>
                    <p style="font-size: 13px; color: #666; margin: 0 0 8px 0;">
                        ${market.description}
                    </p>
                    <p style="font-size: 12px; color: #444; margin: 4px 0;">
                         ${market.address}
                    </p>
                    <p style="font-size: 12px; color: #444; margin: 4px 0;">
                         ${market.openHours}
                    </p>
                    <a href="${market.website}" target="_blank" rel="noopener" style="
                        display: inline-block;
                        margin-top: 8px;
                        color: #16a34a;
                        font-size: 13px;
                        text-decoration: none;
                    ">
                        Visit Website →
                    </a>
                </div>
            `);
        });

        // Cleanup on unmount
        return () => {
            if (mapRef.current) {
                mapRef.current.remove();
                mapRef.current = null;
            }
        };
    }, []);

    return (
        <div className="min-h-screen bg-gradient-to-b from-gray-50 to-gray-100">
            <div className="max-w-7xl mx-auto px-4 py-8">
                {/* Header */}
                <div className="text-center mb-8">
                    <h1 className="text-4xl md:text-5xl font-extrabold text-transparent bg-clip-text bg-gradient-to-r from-green-600 to-emerald-600 mb-4">
                        Market Locations
                    </h1>
                    <p className="text-gray-600 max-w-2xl mx-auto">

                    </p>
                </div>

                {/* Map Container */}
                <div className="bg-white rounded-2xl shadow-xl overflow-hidden border border-gray-100">
                    <div
                        ref={mapContainerRef}
                        style={{ height: '500px', width: '100%' }}
                        className="z-0"
                    />
                </div>

                {/* Market List Below Map */}
                <div className="mt-8 grid md:grid-cols-2 gap-4">
                    {UNIQUE_MARKETS.map((market) => (
                        <div
                            key={market.id}
                            className="bg-white rounded-xl p-4 border border-gray-100"
                        >
                            <div className="flex items-start gap-4">
                                <div
                                    className="w-12 h-12 rounded-lg flex items-center justify-center"
                                    style={{ backgroundColor: market.color + '20' }}
                                >
                                    <Store size={24} style={{ color: market.color }} />
                                </div>
                                <div className="flex-1">
                                    <h3 className="font-bold text-gray-900">{market.name}</h3>
                                    <p className="text-sm text-gray-500 flex items-center gap-1 mt-1">
                                        <MapPin size={12} /> {market.address}
                                    </p>
                                    <p className="text-sm text-gray-500 flex items-center gap-1">
                                        <Clock size={12} /> {market.openHours}
                                    </p>
                                    <a
                                        href={market.website}
                                        target="_blank"
                                        rel="noopener noreferrer"
                                        className="text-sm text-green-600 hover:text-green-700 flex items-center gap-1 mt-2"
                                    >
                                        <ExternalLink size={12} /> Visit Website
                                    </a>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
}
