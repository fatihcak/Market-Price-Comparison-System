export interface Product {
    id: number;
    name: string;
    price: number;
    oldPrice: number;
    market: string;
    discount: number;
    category: string;
    image: string;
}

export const products: Product[] = [
    // Fruits
    { id: 1, name: 'Organik Süt 1L', price: 18.5, oldPrice: 22.0, market: 'A101', discount: 16, category: 'Dairy Products', image: '🥛' },
    { id: 2, name: 'Taze Ekmek', price: 4.5, oldPrice: 5.5, market: 'Migros', discount: 18, category: 'Bread & Grains', image: '🍞' },
    { id: 3, name: 'Yumurta 10lu', price: 12.0, oldPrice: 14.5, market: 'Carrefour', discount: 17, category: 'Eggs & Fish', image: '🥚' },
    { id: 4, name: 'Zeytin Yağı 500ml', price: 45.0, oldPrice: 55.0, market: 'Bim', discount: 18, category: 'Oils', image: '🫒' },
    { id: 5, name: 'Beyaz Peynir 500g', price: 28.5, oldPrice: 35.0, market: 'Tesco', discount: 19, category: 'Dairy Products', image: '🧀' },
    { id: 6, name: 'Domates 1kg', price: 8.5, oldPrice: 10.0, market: 'A101', discount: 15, category: 'Vegetables', image: '🍅' },
    { id: 7, name: 'Salatalık 1kg', price: 6.0, oldPrice: 7.5, market: 'Migros', discount: 20, category: 'Vegetables', image: '🥒' },
    { id: 8, name: 'Sarımsak 500g', price: 15.0, oldPrice: 18.5, market: 'Carrefour', discount: 19, category: 'Vegetables', image: '🧄' },
    { id: 9, name: 'Elma 1kg', price: 12.0, oldPrice: 15.0, market: 'Migros', discount: 20, category: 'Fruits', image: '🍎' },
    { id: 10, name: 'Muz 1kg', price: 25.0, oldPrice: 30.0, market: 'Carrefour', discount: 17, category: 'Fruits', image: '🍌' },
    { id: 11, name: 'Portakal 1kg', price: 10.0, oldPrice: 12.5, market: 'Bim', discount: 20, category: 'Fruits', image: '🍊' },
    { id: 12, name: 'Çilek 500g', price: 35.0, oldPrice: 45.0, market: 'Migros', discount: 22, category: 'Fruits', image: '🍓' },
    { id: 13, name: 'Karpuz kg', price: 5.0, oldPrice: 7.0, market: 'A101', discount: 28, category: 'Fruits', image: '🍉' },
    { id: 14, name: 'Üzüm 1kg', price: 20.0, oldPrice: 25.0, market: 'Tesco', discount: 20, category: 'Fruits', image: '🍇' },
    { id: 15, name: 'Kiraz 500g', price: 40.0, oldPrice: 50.0, market: 'Carrefour', discount: 20, category: 'Fruits', image: '🍒' },
    { id: 16, name: 'Şeftali 1kg', price: 18.0, oldPrice: 22.0, market: 'Migros', discount: 18, category: 'Fruits', image: '🍑' },
    { id: 17, name: 'Armut 1kg', price: 15.0, oldPrice: 18.0, market: 'Bim', discount: 17, category: 'Fruits', image: '🍐' },
    { id: 18, name: 'Limon 1kg', price: 12.0, oldPrice: 15.0, market: 'A101', discount: 20, category: 'Fruits', image: '🍋' },
    { id: 19, name: 'Ananas Adet', price: 45.0, oldPrice: 55.0, market: 'Migros', discount: 18, category: 'Fruits', image: '🍍' },
    { id: 20, name: 'Mango Adet', price: 35.0, oldPrice: 45.0, market: 'Carrefour', discount: 22, category: 'Fruits', image: '🥭' },
    { id: 21, name: 'Kaşar Peyniri 500g', price: 65.0, oldPrice: 80.0, market: 'Bim', discount: 19, category: 'Dairy Products', image: '🧀' },
    { id: 22, name: 'Yoğurt 1kg', price: 25.0, oldPrice: 30.0, market: 'A101', discount: 17, category: 'Dairy Products', image: '🥣' },
    { id: 23, name: 'Tereyağı 250g', price: 55.0, oldPrice: 65.0, market: 'Migros', discount: 15, category: 'Dairy Products', image: '🧈' },
    { id: 24, name: 'Ayran 1L', price: 12.0, oldPrice: 15.0, market: 'Carrefour', discount: 20, category: 'Dairy Products', image: '🥛' },
    { id: 25, name: 'Kefir 1L', price: 22.0, oldPrice: 28.0, market: 'Tesco', discount: 21, category: 'Dairy Products', image: '🥛' },
    { id: 26, name: 'Tam Buğday Ekmeği', price: 8.0, oldPrice: 10.0, market: 'Bim', discount: 20, category: 'Bread & Grains', image: '🍞' },
    { id: 27, name: 'Simit', price: 7.5, oldPrice: 9.0, market: 'Sok', discount: 17, category: 'Bread & Grains', image: '🥯' },
    { id: 28, name: 'Kruvasan', price: 15.0, oldPrice: 18.0, market: 'Migros', discount: 17, category: 'Bread & Grains', image: '🥐' },
    { id: 29, name: 'Baget Ekmek', price: 6.0, oldPrice: 7.5, market: 'Carrefour', discount: 20, category: 'Bread & Grains', image: '🥖' },
    { id: 30, name: 'Ayçiçek Yağı 1L', price: 35.0, oldPrice: 45.0, market: 'A101', discount: 22, category: 'Oils', image: '🌻' },
    { id: 31, name: 'Mısır Özü Yağı 1L', price: 40.0, oldPrice: 50.0, market: 'Migros', discount: 20, category: 'Oils', image: '🌽' },
    { id: 32, name: 'Su 5L', price: 10.0, oldPrice: 12.0, market: 'Bim', discount: 17, category: 'Beverages', image: '💧' },
    { id: 33, name: 'Maden Suyu 6lı', price: 24.0, oldPrice: 30.0, market: 'Carrefour', discount: 20, category: 'Beverages', image: '🥤' },
    { id: 34, name: 'Portakal Suyu 1L', price: 25.0, oldPrice: 32.0, market: 'Migros', discount: 22, category: 'Beverages', image: '🧃' },
    { id: 35, name: 'Türk Kahvesi 100g', price: 25.0, oldPrice: 30.0, market: 'A101', discount: 17, category: 'Coffee & Tea', image: '☕' },
    { id: 36, name: 'Çay 1kg', price: 85.0, oldPrice: 100.0, market: 'Bim', discount: 15, category: 'Coffee & Tea', image: '🍵' },
    { id: 37, name: 'Filtre Kahve 250g', price: 65.0, oldPrice: 80.0, market: 'Migros', discount: 19, category: 'Coffee & Tea', image: '☕' },
    { id: 38, name: 'Patates 1kg', price: 10.0, oldPrice: 12.0, market: 'Sok', discount: 17, category: 'Vegetables', image: '🥔' },
    { id: 39, name: 'Soğan 1kg', price: 8.0, oldPrice: 10.0, market: 'A101', discount: 20, category: 'Vegetables', image: '🧅' },
    { id: 40, name: 'Havuç 1kg', price: 12.0, oldPrice: 15.0, market: 'Migros', discount: 20, category: 'Vegetables', image: '🥕' },
    { id: 41, name: 'Somon Fileto 500g', price: 150.0, oldPrice: 180.0, market: 'Carrefour', discount: 17, category: 'Eggs & Fish', image: '🐟' },
    { id: 42, name: 'Levrek Adet', price: 80.0, oldPrice: 100.0, market: 'Migros', discount: 20, category: 'Eggs & Fish', image: '🐟' },
    { id: 43, name: 'Ton Balığı 3lü', price: 60.0, oldPrice: 75.0, market: 'Bim', discount: 20, category: 'Eggs & Fish', image: '🥫' }
];
