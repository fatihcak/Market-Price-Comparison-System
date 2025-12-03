import { Apple, Milk, Wheat, Droplet, Wine, Coffee, Citrus, Egg } from 'lucide-react';
import { useNavigate, useParams } from 'react-router-dom';

export default function CategorySection() {
  const navigate = useNavigate();
  const { category } = useParams<{ category: string }>();
  const selectedCategory = category || 'All';

  const categories = [
    { name: 'Fruits', icon: Apple, color: 'bg-red-50 hover:bg-red-100 text-red-600' },
    { name: 'Dairy Products', icon: Milk, color: 'bg-blue-50 hover:bg-blue-100 text-blue-600' },
    { name: 'Bread & Grains', icon: Wheat, color: 'bg-orange-50 hover:bg-orange-100 text-orange-600' },
    { name: 'Oils', icon: Droplet, color: 'bg-yellow-50 hover:bg-yellow-100 text-yellow-600' },
    { name: 'Beverages', icon: Wine, color: 'bg-purple-50 hover:bg-purple-100 text-purple-600' },
    { name: 'Coffee & Tea', icon: Coffee, color: 'bg-amber-50 hover:bg-amber-100 text-amber-600' },
    { name: 'Vegetables', icon: Citrus, color: 'bg-green-50 hover:bg-green-100 text-green-600' },
    { name: 'Eggs & Fish', icon: Egg, color: 'bg-pink-50 hover:bg-pink-100 text-pink-600' },
  ];

  const handleSelectCategory = (categoryName: string) => {
    if (selectedCategory === categoryName) {
      navigate('/products/All');
    } else {
      navigate(`/products/${categoryName}`);
    }
  };

  return (
    <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-8 gap-4 group">
      {categories.map((cat) => {
        const Icon = cat.icon;
        const isSelected = selectedCategory === cat.name;

        return (
          <button
            key={cat.name}
            onClick={() => handleSelectCategory(cat.name)}
            className={`${cat.color} 
            p-4 rounded-xl 
            flex 
            flex-col 
            items-center 
            justify-center 
            gap-2
            transition-all
            duration-300 
            origin-top
            group-hover:scale-y-100
            hover:scale-x-125
            hover:shadow-lg
            ${isSelected ? 'ring-2 ring-offset-2 ring-green-500 scale-105 shadow-md' : ''}`}
          >
            <Icon size={28} />
            <span className="text-xs font-semibold text-center">{cat.name}</span>
          </button>
        );
      })}
    </div>
  );
}
