import { useNavigate, useParams } from 'react-router-dom';
import { Category } from '../constants/categories';

interface Props {
  categories: Category[];
}

export default function CategorySection({ categories }: Props) {
  const navigate = useNavigate();
  const { category } = useParams<{ category: string }>();
  // If category is undefined, it defaults to 'All'
  const selectedCategory = category || 'All';

  // We filter out 'All' if we were treating it as a category, 
  // but here it seems we are just displaying the blocks.

  const handleSelectCategory = (categorySlug: string) => {
    // Save current scroll position
    const scrollY = window.scrollY;

    if (selectedCategory === categorySlug) {
      navigate('/products/All');
    } else {
      navigate(`/products/${categorySlug}`);
    }

    // Restore scroll position after navigation
    setTimeout(() => {
      window.scrollTo(0, scrollY);
    }, 0);
  };

  return (
    <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-7 gap-4 group">
      {categories.map((cat) => {
        const Icon = cat.icon;
        const isSelected = selectedCategory === cat.slug;

        return (
          <button
            key={cat.slug}
            onClick={() => handleSelectCategory(cat.slug)}
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
