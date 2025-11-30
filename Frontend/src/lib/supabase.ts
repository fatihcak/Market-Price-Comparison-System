import { createClient } from '@supabase/supabase-js';

const supabaseUrl = import.meta.env.VITE_SUPABASE_URL;
const supabaseKey = import.meta.env.VITE_SUPABASE_ANON_KEY;

if (!supabaseUrl || !supabaseKey) {
  throw new Error('Missing Supabase credentials');
}

export const supabase = createClient(supabaseUrl, supabaseKey);

export type Database = {
  public: {
    Tables: {
      markets: {
        Row: {
          id: string;
          name: string;
          logo_url: string | null;
          city: string;
          rating: number;
          created_at: string;
        };
      };
      categories: {
        Row: {
          id: string;
          name: string;
          icon: string;
          color: string;
          created_at: string;
        };
      };
      products: {
        Row: {
          id: string;
          name: string;
          category_id: string;
          description: string | null;
          image_url: string | null;
          created_at: string;
        };
      };
      product_prices: {
        Row: {
          id: string;
          product_id: string;
          market_id: string;
          price: number;
          original_price: number | null;
          discount_percent: number;
          last_updated: string;
        };
      };
      favorites: {
        Row: {
          id: string;
          user_id: string;
          product_id: string;
          created_at: string;
        };
      };
    };
  };
};
