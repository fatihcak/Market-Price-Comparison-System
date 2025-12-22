# Extra Features Ideas for Market Price Comparison System

## 1. 📍 Location Based Pricing (Konum Bazlı Fiyatlandırma)
Currently, we store `DistrictId` but don't actively filter by it. Prices can vary significantly between districts.
*   **Backend Function:** `GetPricesByLocationAsync(int productId, int districtId)`
*   **Frontend:** User selects their location (e.g., "Kadıköy") upon entry, and only sees prices relevant to that district.
*   **Value:** Increases accuracy of price comparison for the user's specific locality.

## 2. 🔔 Price Alert (Fiyat Alarmı)
Allow users to follow a product and receive notifications when the price drops below a certain threshold.
*   **Backend Function:** `SetPriceAlertAsync(int productId, decimal targetPrice, string email)`
*   **Logic:** Trigger `NotificationService` whenever the scraper updates a price and it falls below the user's target.
*   **Value:** Increases user retention and engagement.

## 3. 📱 Barcode Search (Barkod ile Arama)
Practical for mobile users to quickly find products they have in hand.
*   **Backend:** Add `Barcode` field to `Product` table. Implement `GetProductByBarcodeAsync(string barcode)`.
*   **Frontend:** Integrate camera barcode scanner to redirect directly to the product comparison page.
*   **Value:** Bridges the physical and digital shopping experience.

## 4. 🔥 Trending Products (Trend Ürünler)
Showcase the most popular or most viewed products.
*   **Backend Function:** `GetTrendingProductsAsync()`
*   **Logic:** Add `ViewCount` to `Product` table. Increment on every `GetProductById` request. Return top 10 sorted by view count.
*   **Value:** Helps users discover popular items and deals ("Günün Fırsatları").
