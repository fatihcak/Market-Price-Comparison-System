import { useNavigate, useLocation } from 'react-router-dom';
import { CATEGORIES } from '../constants/categories';


export default function SubCategoryNavbar() {
    const location = useLocation();
    const navigate = useNavigate();

    // Parse path manually since we are outside the <Routes>
    const pathParts = location.pathname.split('/').filter(Boolean);
    const categoryParam = pathParts[1];
    const subCategoryParam = pathParts[2];

    const category = categoryParam ? decodeURIComponent(categoryParam) : undefined;
    const subcategory = subCategoryParam ? decodeURIComponent(subCategoryParam) : undefined;

    const currentCategory = CATEGORIES.find(c => c.slug === category);

    // If no valid category found or no subcategories, render nothing
    if (!currentCategory || !currentCategory.subCategories.length) {
        return null;
    }

    return (
        <div className="relative group mb-8">
            <div className="flex flex-wrap gap-3 py-2 px-1 items-center">
                <button
                    onClick={() => navigate(`/products/${currentCategory.slug}`)}
                    className={`px-4 py-2 rounded-full text-sm font-medium transition-all duration-200 whitespace-nowrap
            ${!subcategory
                            ? 'bg-green-600 text-white shadow-md transform scale-105'
                            : 'bg-white text-gray-600 border border-gray-200 hover:border-green-500 hover:text-green-600'
                        }`}
                >
                    All
                </button>

                {currentCategory.subCategories.map((sub) => {
                    const isActive = subcategory === sub.slug;
                    return (
                        <button
                            key={sub.slug}
                            onClick={() => navigate(`/products/${currentCategory.slug}/${sub.slug}`)}
                            className={`px-4 py-2 rounded-full text-sm font-medium transition-all duration-200 whitespace-nowrap
                ${isActive
                                    ? 'bg-green-600 text-white shadow-md transform scale-105'
                                    : 'bg-white text-gray-600 border border-gray-200 hover:border-green-500 hover:text-green-600'
                                }`}
                        >
                            {sub.name}
                        </button>
                    );
                })}
            </div>
        </div>
    );
}
