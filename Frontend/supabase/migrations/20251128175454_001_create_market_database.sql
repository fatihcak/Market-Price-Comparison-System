/*
  # Market Fiyatı - Database Schema

  1. New Tables
    - `markets`: Mağazalar ve market bilgileri
      - `id` (uuid, primary key)
      - `name` (text) - Market adı
      - `logo_url` (text) - Market logosu
      - `city` (text) - Şehir
      - `rating` (numeric) - Puan
      - `created_at` (timestamp)

    - `categories`: Ürün kategorileri
      - `id` (uuid, primary key)
      - `name` (text) - Kategori adı
      - `icon` (text) - İkon adı
      - `color` (text) - Renk sınıfı
      - `created_at` (timestamp)

    - `products`: Ürünler
      - `id` (uuid, primary key)
      - `name` (text) - Ürün adı
      - `category_id` (uuid, foreign key)
      - `description` (text)
      - `image_url` (text)
      - `created_at` (timestamp)

    - `product_prices`: Ürün fiyatları (market bazlı)
      - `id` (uuid, primary key)
      - `product_id` (uuid, foreign key)
      - `market_id` (uuid, foreign key)
      - `price` (numeric) - Güncel fiyat
      - `original_price` (numeric) - Orijinal fiyat
      - `discount_percent` (integer)
      - `last_updated` (timestamp)

    - `favorites`: Kullanıcı favorileri
      - `id` (uuid, primary key)
      - `user_id` (uuid, foreign key to auth.users)
      - `product_id` (uuid, foreign key)
      - `created_at` (timestamp)

  2. Security
    - Enable RLS on all tables
    - Add policies for public read access to products and markets
    - Add policies for authenticated users to manage favorites

  3. Indexes
    - Index on products.category_id for faster filtering
    - Index on product_prices.market_id for faster queries
    - Index on favorites.user_id for faster favorite retrieval
*/

CREATE TABLE IF NOT EXISTS markets (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  name text NOT NULL,
  logo_url text,
  city text NOT NULL,
  rating numeric DEFAULT 0,
  created_at timestamptz DEFAULT now()
);

CREATE TABLE IF NOT EXISTS categories (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  name text NOT NULL UNIQUE,
  icon text NOT NULL,
  color text NOT NULL,
  created_at timestamptz DEFAULT now()
);

CREATE TABLE IF NOT EXISTS products (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  name text NOT NULL,
  category_id uuid NOT NULL REFERENCES categories(id) ON DELETE CASCADE,
  description text,
  image_url text,
  created_at timestamptz DEFAULT now()
);

CREATE TABLE IF NOT EXISTS product_prices (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  product_id uuid NOT NULL REFERENCES products(id) ON DELETE CASCADE,
  market_id uuid NOT NULL REFERENCES markets(id) ON DELETE CASCADE,
  price numeric NOT NULL,
  original_price numeric,
  discount_percent integer DEFAULT 0,
  last_updated timestamptz DEFAULT now()
);

CREATE TABLE IF NOT EXISTS favorites (
  id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
  user_id uuid NOT NULL REFERENCES auth.users(id) ON DELETE CASCADE,
  product_id uuid NOT NULL REFERENCES products(id) ON DELETE CASCADE,
  created_at timestamptz DEFAULT now(),
  UNIQUE(user_id, product_id)
);

ALTER TABLE markets ENABLE ROW LEVEL SECURITY;
ALTER TABLE categories ENABLE ROW LEVEL SECURITY;
ALTER TABLE products ENABLE ROW LEVEL SECURITY;
ALTER TABLE product_prices ENABLE ROW LEVEL SECURITY;
ALTER TABLE favorites ENABLE ROW LEVEL SECURITY;

CREATE POLICY "Markets are viewable by everyone" ON markets FOR SELECT USING (true);
CREATE POLICY "Categories are viewable by everyone" ON categories FOR SELECT USING (true);
CREATE POLICY "Products are viewable by everyone" ON products FOR SELECT USING (true);
CREATE POLICY "Product prices are viewable by everyone" ON product_prices FOR SELECT USING (true);

CREATE POLICY "Users can manage own favorites" ON favorites
  FOR SELECT TO authenticated USING (auth.uid() = user_id);

CREATE POLICY "Users can add favorites" ON favorites
  FOR INSERT TO authenticated WITH CHECK (auth.uid() = user_id);

CREATE POLICY "Users can delete own favorites" ON favorites
  FOR DELETE TO authenticated USING (auth.uid() = user_id);

CREATE INDEX idx_products_category_id ON products(category_id);
CREATE INDEX idx_product_prices_market_id ON product_prices(market_id);
CREATE INDEX idx_product_prices_product_id ON product_prices(product_id);
CREATE INDEX idx_favorites_user_id ON favorites(user_id);
