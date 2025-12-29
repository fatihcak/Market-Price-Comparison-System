import { Apple, Milk, Wheat, Droplet, Wine, Coffee, Egg } from 'lucide-react';

export interface SubCategory {
    name: string;
    slug: string; // Must match Database CategoryName exactly
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
            { name: 'Vegetables', slug: 'Vegetables' },
            { name: 'Fruits & Vegetables', slug: 'Fruits & Vegetables' }
        ]
    },
    {
        name: 'Meat, Chicken & Fish',
        slug: 'Meat, Chicken & Fish',
        icon: Milk,
        color: 'bg-blue-50 hover:bg-blue-100 text-blue-600',
        subCategories: [
            { name: 'Beef & Lamb', slug: 'Beef & Lamb' },
            { name: 'Poultry & Chicken', slug: 'Poultry & Chicken' },
            { name: 'Fish & Seafood', slug: 'Fish & Seafood' },
            { name: 'Cold Cuts & Cured Meats', slug: 'Cold Cuts & Cured Meats' },
            { name: 'Offal', slug: 'Offal' },
            { name: 'Meat, Chicken & Fish', slug: 'Meat, Chicken & Fish' }
        ]
    },
    {
        name: 'Dairy Products & Breakfast Foods',
        slug: 'Dairy Products & Breakfast Foods',
        icon: Wheat,
        color: 'bg-orange-50 hover:bg-orange-100 text-orange-600',
        subCategories: [
            { name: 'Cheese', slug: 'Cheese' },
            { name: 'Milk', slug: 'Milk' },
            { name: 'Yogurt', slug: 'Yogurt' },
            { name: 'Butter & Margarine', slug: 'Butter & Margarine' },
            { name: 'Eggs', slug: 'Eggs' },
            { name: 'Olives', slug: 'Olives' },
            { name: 'Honey & Jam', slug: 'Honey & Jam' },
            { name: 'Clotted Cream & Cream', slug: 'Clotted Cream & Cream' },
            { name: 'Ayran & Kefir', slug: 'Ayran & Kefir' },
            { name: 'Breakfast Cereals, Bars & Granola', slug: 'Breakfast Cereals, Bars & Granola' },
            { name: 'Spreads & Breakfast Sauces', slug: 'Spreads & Breakfast Sauces' },
            { name: 'Dairy Products & Breakfast Foods', slug: 'Dairy Products & Breakfast Foods' }
        ]
    },
    {
        name: 'Staple Foods',
        slug: 'Staple Food', // DB name is 'Staple Food' (singular)
        icon: Droplet,
        color: 'bg-yellow-50 hover:bg-yellow-100 text-yellow-600',
        subCategories: [
            { name: 'Bread & Bakery Products', slug: 'Bread & Bakery Products' },
            { name: 'Oils', slug: 'Oils' },
            { name: 'Pulses', slug: 'Pulses' },
            { name: 'Dumplings, Pasta & Noodles', slug: 'Dumplings, Pasta & Noodles' },
            { name: 'Flour & Semolina', slug: 'Flour & Semolina' },
            { name: 'Baking Ingredients', slug: 'Baking Ingredients' },
            { name: 'Sugar & Sweeteners', slug: 'Sugar & Sweeteners' },
            { name: 'Salt & Spices', slug: 'Salt & Spices' },
            { name: 'Sauces & Vinegar', slug: 'Sauces & Vinegar' },
            { name: 'Tomato Paste', slug: 'Tomato Paste' },
            { name: 'Canned Goods', slug: 'Canned Goods' },
            { name: 'Pickles', slug: 'Pickles' },
            { name: 'Baby Food', slug: 'Baby Food' },
            { name: 'Convenience Food', slug: 'Convenience Food' },
            { name: 'Staple Food', slug: 'Staple Food' }
        ]
    },
    {
        name: 'Drinks',
        slug: 'Drink', // DB has 'Drink' (singular)
        icon: Wine,
        color: 'bg-purple-50 hover:bg-purple-100 text-purple-600',
        subCategories: [
            { name: 'Water', slug: 'Water' },
            { name: 'Mineral Water', slug: 'Mineral Water' },
            { name: 'Fruit Juice', slug: 'Fruit Juice' },
            { name: 'Carbonated Drinks', slug: 'Carbonated Drinks' },
            { name: 'Non-Carbonated Drinks', slug: 'Non-Carbonated Drinks' },
            { name: 'Tea & Herbal Teas', slug: 'Tea & Herbal Teas' },
            { name: 'Coffee', slug: 'Coffee' },
            { name: 'Drink', slug: 'Drink' }
        ]
    },
    {
        name: 'Snacks & Desserts',
        slug: 'Snacks & Dessert', // DB singular 'Dessert'
        icon: Coffee,
        color: 'bg-amber-50 hover:bg-amber-100 text-amber-600',
        subCategories: [
            { name: 'Chocolate', slug: 'Chocolate' },
            { name: 'Biscuits & Crackers', slug: 'Biscuits & Crackers' },
            { name: 'Cakes', slug: 'Cakes' },
            { name: 'Wafers', slug: 'Wafers' },
            { name: 'Chips', slug: 'Chips' },
            { name: 'Nuts & Dried Fruits', slug: 'Nuts & Dried Fruits' },
            { name: 'Gum & Candy', slug: 'Gum & Candy' },
            { name: 'Ice Cream', slug: 'Ice Cream' },
            { name: 'Desserts', slug: 'Desserts' },
            { name: 'Snacks & Dessert', slug: 'Snacks & Dessert' }
        ]
    },
    {
        name: 'Cleaning & Personal Care',
        slug: 'Cleaning & Personal Care Products', // DB Name
        icon: Egg,
        color: 'bg-pink-50 hover:bg-pink-100 text-pink-600',
        subCategories: [
            { name: 'Laundry Products', slug: 'Laundry Products' },
            { name: 'Dishwashing & Cleaning Products', slug: 'Dishwashing & Cleaning Products' },
            { name: 'General Purpose Cleaners', slug: 'General Purpose Cleaners' },
            { name: 'Kitchen Supplies', slug: 'Kitchen Supplies' },
            { name: 'Paper Towels', slug: 'Paper Towels' },
            { name: 'Toilet Paper', slug: 'Toilet Paper' },
            { name: 'Paper Napkins & Tissues', slug: 'Paper Napkins & Tissues' },
            { name: 'Wet Wipes', slug: 'Wet Wipes' },
            { name: 'Other Cleaning', slug: 'Other Cleaning' },
            { name: 'Hair Care', slug: 'Hair Care' },
            { name: 'Shower, Bath & Soap', slug: 'Shower, Bath & Soap' },
            { name: 'Oral Care', slug: 'Oral Care' },
            { name: 'Skin Care', slug: 'Skin Care' },
            { name: 'Makeup', slug: 'Makeup' },
            { name: 'Feminine Hygiene', slug: 'Feminine Hygiene' },
            { name: 'Diapers & Adult Incontinence', slug: 'Diapers & Adult Incontinence' },
            { name: 'Fragrances, Deodorants & Colognes', slug: 'Fragrances, Deodorants & Colognes' },
            { name: 'Cleaning & Personal Care Products', slug: 'Cleaning & Personal Care Products' }
        ]
    }
];
