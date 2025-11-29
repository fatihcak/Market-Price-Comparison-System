import { MapPin, TrendingDown, ShoppingCart } from 'lucide-react';
import { useState } from 'react';
import Header from './components/Header';
import SearchBar from './components/SearchBar';
import ProductCard from './components/ProductCard';
import CategorySection from './components/CategorySection';
import FilterBar from './components/FilterBar';
import Testimonials from './components/Testimonials';
import PriceComparison from './components/PriceComparison';

function App() {
  const [comparisonOpen, setComparisonOpen] = useState(false);
  const [selectedProduct, setSelectedProduct] = useState('Organik Süt 1L');

  const openComparison = (productName: string) => {
    setSelectedProduct(productName);
    setComparisonOpen(true);
  };

  return (
    <div className="min-h-screen bg-white">
      <Header />

      <main className="max-w-7xl mx-auto px-4 py-8">
        <SearchBar />

        <section className="mt-12">
          <div className="flex items-center justify-between mb-8">
            <h2 className="text-3xl font-bold text-gray-900">Kategoriler</h2>
          </div>
          <CategorySection />
        </section>

        <section className="mt-16">
          <FilterBar />

          <div className="flex items-center justify-between mb-8">
            <h2 className="text-3xl font-bold text-gray-900">Popüler Ürünler</h2>
            <button className="text-green-600 hover:text-green-700 font-semibold text-sm transition-colors">
              Tümünü Gör →
            </button>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            {[1, 2, 3, 4, 5, 6, 7, 8].map((item) => (
              <div key={item} onClick={() => openComparison(`Ürün ${item}`)}>
                <ProductCard id={item} />
              </div>
            ))}
          </div>
        </section>

        <section className="mt-20 bg-gradient-to-r from-green-50 to-emerald-50 rounded-2xl p-12">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center hover:scale-105 transition-transform duration-300">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 shadow-md">
                <TrendingDown className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">Fiyat Takibi</h3>
              <p className="text-gray-600">Favori ürünlerinizin fiyat değişimlerini takip edin</p>
            </div>

            <div className="text-center hover:scale-105 transition-transform duration-300">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 shadow-md">
                <MapPin className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">Yakın Market</h3>
              <p className="text-gray-600">Etrafınızdaki en uygun fiyatlı marketleri bulun</p>
            </div>

            <div className="text-center hover:scale-105 transition-transform duration-300">
              <div className="bg-green-100 w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 shadow-md">
                <ShoppingCart className="text-green-600" size={32} />
              </div>
              <h3 className="text-xl font-bold text-gray-900 mb-2">Akıllı Liste</h3>
              <p className="text-gray-600">Alışveriş listelerinizi oluşturun ve tasarruf edin</p>
            </div>
          </div>
        </section>
      </main>

      <Testimonials />

      <footer className="bg-gray-900 text-gray-300 py-12">
        <div className="max-w-7xl mx-auto px-4">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-8 mb-8">
            <div>
              <h4 className="text-white font-bold mb-4">Market Fiyatı</h4>
              <p className="text-sm">En iyi fiyatları bulmak artık daha kolay</p>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Hızlı Linkler</h4>
              <ul className="text-sm space-y-2">
                <li><a href="#" className="hover:text-white transition-colors">Hakkımızda</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Marketler</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Blog</a></li>
              </ul>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Destek</h4>
              <ul className="text-sm space-y-2">
                <li><a href="#" className="hover:text-white transition-colors">İletişim</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Gizlilik</a></li>
                <li><a href="#" className="hover:text-white transition-colors">Kullanım Şartları</a></li>
              </ul>
            </div>
            <div>
              <h4 className="text-white font-bold mb-4">Biz'e Takip Edin</h4>
              <p className="text-sm">Sosyal medya kanallarımızı takip edin</p>
            </div>
          </div>
          <div className="border-t border-gray-700 pt-8 text-sm text-center">
            <p>&copy; 2024 Market Fiyatı. Tüm hakları saklıdır.</p>
          </div>
        </div>
      </footer>

      <PriceComparison isOpen={comparisonOpen} onClose={() => setComparisonOpen(false)} productName={selectedProduct} />
    </div>
  );
}

export default App;
