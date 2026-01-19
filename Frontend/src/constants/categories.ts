import { Apple, Milk, Wheat, Droplet, Wine, Coffee, Egg } from 'lucide-react';

export interface SubCategory {
    name: string;
    slug: string; // Must match Database CategoryName exactly
}

export interface Category {
    id: number;
    name: string;
    icon: any;
    color: string;
    slug: string;
    subCategories: SubCategory[];
}

// IMPORTANT: Names and slugs MUST exactly match the backend database CategoryName values.
// IDs are placeholders - the real IDs (15-21) are fetched from the API in App.tsx.
// Backend Categories (from https://compare-market.site/api/Category):
//   15: "Fruits & Vegetables"
//   16: "Meat, Chicken & Fish"
//   17: "Dairy Products & Breakfast Foods"
//   18: "Staple Food"
//   19: "Drink"
//   20: "Snacks & Dessert"
//   21: "Cleaning & Personal Care Products"

export const CATEGORIES: Category[] = [
    {
        id: 15, // Backend ID
        name: 'Fruits & Vegetables',
        slug: 'Fruits & Vegetables', // MUST match backend CategoryName
        icon: Apple,
        color: 'bg-emerald-50 hover:bg-emerald-100 text-emerald-600',
        subCategories: [
            { name: 'Fruits', slug: 'Fruits' },
            { name: 'Vegetables', slug: 'Vegetables' }
        ]
    },
    {
        id: 16, // Backend ID
        name: 'Meat, Chicken & Fish',
        slug: 'Meat, Chicken & Fish', // MUST match backend CategoryName
        icon: Milk,
        color: 'bg-rose-50 hover:bg-rose-100 text-rose-600',
        subCategories: [
            { name: 'Beef & Lamb', slug: 'Beef & Lamb' },
            { name: 'Poultry & Chicken', slug: 'Poultry & Chicken' },
            { name: 'Fish & Seafood', slug: 'Fish & Seafood' },
            { name: 'Cold Cuts & Cured Meats', slug: 'Cold Cuts & Cured Meats' },
            { name: 'Offal', slug: 'Offal' }
        ]
    },
    {
        id: 17, // Backend ID
        name: 'Dairy Products & Breakfast Foods',
        slug: 'Dairy Products & Breakfast Foods', // MUST match backend CategoryName
        icon: Wheat,
        color: 'bg-amber-100 hover:bg-amber-100 text-amber-600',
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
            { name: 'Halva, Tahini & Molasses', slug: 'Halva, Tahini & Molasses' }
        ]
    },
    {
        id: 18, // Backend ID
        name: 'Staple Food',
        slug: 'Staple Food', // MUST match backend CategoryName
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
            { name: 'Convenience Food', slug: 'Convenience Food' }
        ]
    },
    {
        id: 19, // Backend ID
        name: 'Drink',
        slug: 'Drink', // MUST match backend CategoryName
        icon: Wine,
        color: 'bg-blue-50 hover:bg-blue-100 text-blue-600',
        subCategories: [
            { name: 'Water', slug: 'Water' },
            { name: 'Mineral Water', slug: 'Mineral Water' },
            { name: 'Fruit Juice', slug: 'Fruit Juice' },
            { name: 'Carbonated Drinks', slug: 'Carbonated Drinks' },
            { name: 'Non-Carbonated Drinks', slug: 'Non-Carbonated Drinks' },
            { name: 'Tea & Herbal Teas', slug: 'Tea & Herbal Teas' },
            { name: 'Coffee', slug: 'Coffee' }
        ]
    },
    {
        id: 20, // Backend ID
        name: 'Snacks & Dessert',
        slug: 'Snacks & Dessert', // MUST match backend CategoryName
        icon: Coffee,
        color: 'bg-orange-50 hover:bg-orange-100 text-orange-600',
        subCategories: [
            { name: 'Chocolate', slug: 'Chocolate' },
            { name: 'Biscuits & Crackers', slug: 'Biscuits & Crackers' },
            { name: 'Cakes', slug: 'Cakes' },
            { name: 'Wafers', slug: 'Wafers' },
            { name: 'Chips', slug: 'Chips' },
            { name: 'Nuts & Dried Fruits', slug: 'Nuts & Dried Fruits' },
            { name: 'Gum & Candy', slug: 'Gum & Candy' },
            { name: 'Ice Cream', slug: 'Ice Cream' },
            { name: 'Desserts', slug: 'Desserts' }
        ]
    },
    {
        id: 21, // Backend ID
        name: 'Cleaning & Personal Care Products',
        slug: 'Cleaning & Personal Care Products', // MUST match backend CategoryName
        icon: Egg,
        color: 'bg-pink-100 hover:bg-pink-100 text-pink-600',
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
            { name: 'Fragrances, Deodorants & Colognes', slug: 'Fragrances, Deodorants & Colognes' }
        ]
    }
];
