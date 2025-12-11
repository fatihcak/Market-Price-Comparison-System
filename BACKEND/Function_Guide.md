# Backend Fonksiyon Rehberi ve Hata Ayıklama (Debugging) Prosedürleri

Bu doküman, backend servislerinin temel fonksiyonlarını, beklenen davranışlarını ve breakpoint kullanarak bunların nasıl doğrulanacağını özetler.

---

## 1. ProductService (`Domain/Services/ProductService.cs`)

### `GetAllProductsAsync()`
- **İşleyiş:** Veritabanındaki tüm ürünleri; Kategori, Market Fiyatları ve Fiyat Geçmişi detaylarıyla birlikte getirir.
- **Beklenen Sonuç:** `ProductResponseDTO` nesnelerinden oluşan bir liste.
- **Debug Adımları:**
    1. `GetAllProductsAsync` fonksiyonunun başlangıcına Breakpoint koyun.
    2. **Tetikleme:** Frontend -> Anasayfa (tüm ürünler yüklenir) VEYA `GET /api/products`.
    3. `_productRepository.GetAllWithDetailsAsync()` döndükten sonra `products` değişkenini kontrol edin. Boş olmadığından emin olun.

### `GetProductByIdAsync(int id)`
- **İşleyiş:** ID'ye göre tek bir ürün getirir. Bulunamazsa hata fırlatır veya null döner.
- **Beklenen Sonuç:** Tek bir `ProductResponseDTO`.
- **Debug Adımları:**
    1. Metodun başlangıcına Breakpoint koyun.
    2. **Tetikleme:** Frontend -> Bir ürün kartına tıklayın VEYA `GET /api/products/1`.
    3. `product` değişkeninin veritabanında bulunduğunu doğrulayın.

### `CreateProductAsync(CreateProductDTO dto)`
- **İşleyiş:** Yeni bir ürün oluşturur. Kategorinin var olup olmadığını kontrol eder.
- **Beklenen Sonuç:** Oluşturulan `ProductResponseDTO`.
- **Debug Adımları:**
    1. `_productRepository.AddAsync` satırına Breakpoint koyun.
    2. **Tetikleme:** Swagger `POST /api/products` (Admin Token gerektirir).
    3. Ekleme yapılmadan önce `entity` değişkenini inceleyerek özelliklerin doğru eşleştiğinden emin olun.

---

## 2. BasketService (`Domain/Services/BasketService.cs`)

### `GetBasketComparisonAsync(List<CartItemDTO> items)`
- **İşleyiş:** En karmaşık fonksiyondur. Ürün listesini ve adetlerini alır, TÜM marketlerdeki fiyatlarını bulur ve bunları "Market Sepetleri" olarak gruplar. Her market için toplam tutarı hesaplar ve eksik ürünleri işaretler.
- **Beklenen Sonuç:** `MarketBasketDTO` listesi (market başına bir tane). Önce eksiksiz olanlar (en az eksik ürün), sonra fiyata göre (en düşük önce) sıralanır.
- **Debug Adımları:**
    1. `_marketRepository.GetAllAsync()` döngüsünün olduğu yere Breakpoint koyun.
    2. **Tetikleme:** Frontend -> Alışveriş Listesindeki "Compare Prices" (Kıyasla) butonu.
    3. Liste oluşturulurken `marketBaskets` değişkenini izleyin (Watch). Ürünün olmadığı marketler için `missingCount` değerinin doğru hesaplandığını kontrol edin.

---

## 3. ChatService (`Domain/Services/ChatService.cs`)

### `ProcessUserMessageAsync(string userId, string message)`
- **İşleyiş:** Bir karar mekanizması gibi çalışır.
    - Mesaj "cheapest" (en ucuz), "basket" (sepet), "calculate" (hesapla) gibi anahtar kelimeler içeriyorsa `smart_basket` niyeti (intent) olarak algılar.
    - Aksi takdirde, genel bir cevap için istemi (prompt) Gemini API'ye gönderir.
- **Beklenen Sonuç:** AI'nın cevabını veya hesaplanan sonucu içeren bir `ChatResponse`.
- **Debug Adımları:**
    1. `if (intent == "smart_basket")` kontrolüne Breakpoint koyun.
    2. **Tetikleme:** Frontend Chatbot -> "Calculate basket for Milk" (Süt için sepeti hesapla) yazın.
    3. `IdentifyIntent` fonksiyonunun girdiyi doğru sınıflandırıp sınıflandırmadığını doğrulayın.

---

## 4. ShoppingListService (`Domain/Services/ShoppingListService.cs`)

### `AddItemToShoppingListAsync(CreateShoppingListDTO dto)`
- **İşleyiş:** Kullanıcının kalıcı alışveriş listesine (DB) bir ürün ekler. Ürün zaten varsa adedini artırır.
- **Beklenen Sonuç:** Eklenen/Güncellenen `UserProductListDTO`.
- **Debug Adımları:**
    1. `_shoppingListRepository.GetBySessionAndProductIdAsync` satırına Breakpoint koyun.
    2. **Tetikleme:** Frontend -> Bir üründeki "Add" butonuna tıklayın.
    3. `existingItem` değişkenini kontrol edin. Null değilse `UpdateAsync`'e gittiğinden, null ise `AddAsync` gibi davrandığından emin olun.

---

## 5. PriceService (`Domain/Services/PriceService.cs`)

### `GetPricesByProductAsync(int productId)`
- **İşleyiş:** Belirli bir ürün için farklı marketlerdeki bilinen tüm fiyatları getirir.
- **Beklenen Sonuç:** `PriceResponseDTO` listesi.
- **Debug Adımları:**
    1. `_priceRepository.GetByProductIdAsync` satırına Breakpoint koyun.
    2. **Tetikleme:** `GET /api/prices/product/{id}`.
    3. Dönen fiyatların Market ve İlçe (District) isimlerini içerdiğini doğrulayın.

---

## 6. AdminAuthService (`Domain/Services/AdminAuthService.cs`)

### `LoginAsync(AdminLoginDTO dto)`
- **İşleyiş:** Kullanıcı adı ve şifreyi (hash kontrolü ile) doğrular. JWT token üretir.
- **Beklenen Sonuç:** Token içeren `AuthResponse`.
- **Debug Adımları:**
    1. `BCrypt.Net.BCrypt.Verify` satırına Breakpoint koyun.
    2. **Tetikleme:** Swagger `POST /api/auth/login`.
    3. Şifre doğrulamasının `true` dönüp dönmediğini kontrol edin.

---

## 7. MarketService (`Domain/Services/MarketService.cs`)

### `GetAllMarketsAsync()`
- **İşleyiş:** Tanımlı tüm marketleri (Migros, Carrefour vb.) basitçe getirir.
- **Beklenen Sonuç:** `MarketResponseDTO` listesi.
- **Debug Adımları:**
    1. `_marketRepository.GetAllAsync` satırına Breakpoint koyun.
    2. **Tetikleme:** Frontend -> Filtre Çubuğu (Marketler dropdown) VEYA `GET /api/markets`.

---

## 8. CategoryService (`Domain/Services/CategoryService.cs`)

### `GetAllCategoriesAsync()`
- **İşleyiş:** Kategori hiyerarşisini getirir.
- **Beklenen Sonuç:** Kategoriler listesi.
- **Debug Adımları:**
    1. Metodun başlangıcına Breakpoint koyun.
    2. **Tetikleme:** Frontend -> Anasayfa (Kategori Bölümü).

---

## Genel Hata Ayıklama (Debugging) İpuçları
1. **Erişilemeyen Kod (Gray Code):** Eğer bir breakpoint gri/içi boş görünüyorsa, kod yüklenmemiş veya optimize edilmiş olabilir. `Debug` konfigürasyonunda çalıştırdığınızdan emin olun.
2. **İstisnalar (Exceptions):** 500 hatalarını oluştuğu anda yakalamak için IDE'nizde "Break on All Exceptions" (Tüm İstisnalarda Dur) seçeneğini etkinleştirin.
3. **Async/Await:** `await` üzerinden geçerken dikkatli olun. Yanlışlıkla framework metodunun içine girmemek için "Step Over" yerine "Step Out" gerekebilir.
