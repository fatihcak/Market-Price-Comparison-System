import { Apple, Milk, Wheat, Droplet, Wine, Coffee, Egg } from 'lucide-react';

export interface SubCategory {
    name: string;
    slug: string; // Used for URL matching if needed, or just use name
}

export interface Category {
    name: string;
    icon: any;
    color: string;
    slug: string;
    subCategories: SubCategory[];
}

export const CATEGORIES: Category[] = [
    {
        name: 'Fruits and Vegetables',
        slug: 'Fruits and Vegetables',
        icon: Apple,
        color: 'bg-red-50 hover:bg-red-100 text-red-600',
        subCategories: [
            { name: 'Fruits', slug: 'Fruits' },
            { name: 'Vegetables', slug: 'Vegetables' }
        ]
    },
    {
        name: 'Meat, Chicken & Fish',
        slug: 'Meat, Chicken & Fish',
        icon: Milk, // Placeholder icon from original file
        color: 'bg-blue-50 hover:bg-blue-100 text-blue-600',
        subCategories: [
            { name: 'Beef & Lamb', slug: 'Beef & Lamb' },
            { name: 'Poultry & Chicken', slug: 'Poultry & Chicken' },
            { name: 'Fish & Seafood', slug: 'Fish & Seafood' },
            { name: 'Cold Cuts & Cured Meats', slug: 'Cold Cuts & Cured Meats' },
            { name: 'Offal', slug: 'Offal' }
        ]
    },
    {
        name: 'Dairy Products & Breakfast Foods',
        slug: 'Dairy Products & Breakfast Foods',
        icon: Wheat,
        color: 'bg-orange-50 hover:bg-orange-100 text-orange-600',
        subCategories: [
            { name: 'Dairy Products', slug: 'Dairy Products' },
            { name: 'Cheese', slug: 'Cheese' },
            { name: 'Olives', slug: 'Olives' },
            { name: 'Honey & Jam', slug: 'Honey & Jam' }
        ]
    },
    {
        name: 'Staple Foods',
        slug: 'Staple Foods',
        icon: Droplet,
        color: 'bg-yellow-50 hover:bg-yellow-100 text-yellow-600',
        subCategories: [
            { name: 'Bread & Grains', slug: 'Bread & Grains' },
            { name: 'Oils', slug: 'Oils' },
            { name: 'Legumes', slug: 'Legumes' },
            { name: 'Pasta', slug: 'Pasta' }
        ]
    },
    {
        name: 'Drinks',
        slug: 'Drinks',
        icon: Wine,
        color: 'bg-purple-50 hover:bg-purple-100 text-purple-600',
        subCategories: [
            { name: 'Water', slug: 'Water' },
            { name: 'Soft Drinks', slug: 'Soft Drinks' },
            { name: 'Coffee & Tea', slug: 'Coffee & Tea' },
            { name: 'Fruit Juices', slug: 'Fruit Juices' }
        ]
    },
    {
        name: 'Snacks & Desserts',
        slug: 'Snacks & Desserts',
        icon: Coffee,
        color: 'bg-amber-50 hover:bg-amber-100 text-amber-600',
        subCategories: [
            { name: 'Chocolates', slug: 'Chocolates' },
            { name: 'Chips', slug: 'Chips' },
            { name: 'Biscuits', slug: 'Biscuits' },
            { name: 'Cakes', slug: 'Cakes' }
        ]
    },
    {
        name: 'Cleaning & Personal Care',
        slug: 'Cleaning & Personal Care',
        icon: Egg,
        color: 'bg-pink-50 hover:bg-pink-100 text-pink-600',
        subCategories: [
            { name: 'Laundry', slug: 'Laundry' },
            { name: 'Dishwashing', slug: 'Dishwashing' },
            { name: 'Hair Care', slug: 'Hair Care' },
            { name: 'Body Care', slug: 'Body Care' }
        ]
    },
];
