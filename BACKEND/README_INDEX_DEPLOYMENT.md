# Database Index Deployment Guide

## Problem
Frontend ürünleri yüklerken 50 saniye sürüyor. Bu performans problemi database indexlerinin eksik olmasından kaynaklanıyor olabilir.

## Solution
AWS RDS SQL Server database'e performans indexleri eklemek.

## Deployment Steps

### Option 1: SQL Server Management Studio (SSMS) ile

1. **SSMS'i açın** ve AWS RDS'e bağlanın:
   - Server: `database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433`
   - Database: `compsys`
   - Username: `admin`
   - Password: `admin-0-0`

2. **Önce mevcut indexleri kontrol edin**:
   - `01_Check_Existing_Indexes.sql` dosyasını açın
   - Execute (F5) tuşuna basın
   - Hangi indexlerin eksik olduğunu not edin

3. **Eksik indexleri oluşturun**:
   - `02_Create_Missing_Indexes.sql` dosyasını açın
   - Execute (F5) tuşuna basın
   - Tüm indexlerin başarıyla oluşturulduğunu kontrol edin

### Option 2: Azure Data Studio ile

1. **Azure Data Studio'yu açın**
2. AWS RDS'e bağlanın (yukarıdaki connection string ile)
3. Aynı SQL dosyalarını çalıştırın

### Option 3: Command Line (sqlcmd) ile

```powershell
# Check existing indexes
sqlcmd -S database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433 -d compsys -U admin -P admin-0-0 -i 01_Check_Existing_Indexes.sql -C

# Create missing indexes
sqlcmd -S database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433 -d compsys -U admin -P admin-0-0 -i 02_Create_Missing_Indexes.sql -C
```

### Option 4: EF Core Migration ile (Alternatif)

Eğer migrations kullanmak isterseniz:

```bash
cd BACKEND/src/API
dotnet ef migrations add AddPerformanceIndexes
dotnet ef database update
```

**NOT**: Bu seçenek yeni bir migration oluşturur, ancak indexler zaten migration dosyasında var. Bu yüzden önce live database'in migrated olup olmadığını kontrol edin:

```sql
SELECT * FROM [__EFMigrationsHistory];
```

## Critical Indexes Being Created

### Products Table
- `IX_Products_CategoryID` - Category ile JOIN için ⚡
- `IX_Products_ProductName` - Arama ve sıralama için
- `IX_Products_Brand` - Brand filtering için

### MarketProductPrices Table (EN KRİTİK!)
- `IX_MarketProductPrices_ProductID` - Product JOIN için ⚡⚡⚡
- `IX_MarketProductPrices_MarketID` - Market JOIN için ⚡⚡⚡
- `IX_MarketProductPrices_DistrictID` - District filtering için
- `IX_MarketProductPrices_ProductID_DistrictID` - Composite index
- `IX_MarketProductPrices_LastUpdated` - Date filtering için

### Other Tables
- `IX_Districts_CityID` - City JOIN için
- `IX_Markets_MarketName` - Market arama için
- `IX_UserProductLists_ProductID` - User list JOIN için

## Expected Impact

### Before Indexes:
- ❌ Product loading: **50 seconds**
- ❌ Table scans on every query
- ❌ Poor user experience

### After Indexes:
- ✅ Product loading: **2-5 seconds** (10-25x faster!)
- ✅ Index seeks instead of table scans
- ✅ Great user experience

## Verification

Index oluşturma işleminden sonra, aşağıdaki SQL ile doğrulayın:

```sql
-- Check if indexes exist
SELECT 
    OBJECT_NAME(i.object_id) AS TableName,
    i.name AS IndexName,
    i.type_desc AS IndexType
FROM sys.indexes i
WHERE OBJECT_NAME(i.object_id) IN ('Products', 'MarketProductPrices')
ORDER BY TableName, IndexName;
```

## Testing

1. **Backend'i test edin**:
   ```bash
   # API'yi çalıştırın
   dotnet run --project BACKEND/src/API
   
   # Products endpoint'ini test edin
   curl http://localhost:5000/api/Products
   ```

2. **Frontend'i test edin**:
   - Uygulamayı açın
   - Products sayfasına gidin
   - Yükleme süresini ölçün
   - 50 saniye → 2-5 saniye olmalı!

## Rollback (If Needed)

Eğer bir sorun olursa, indexleri kaldırabilirsiniz:

```sql
-- Remove all custom indexes
DROP INDEX IF EXISTS [IX_Products_CategoryID] ON [dbo].[Products];
DROP INDEX IF EXISTS [IX_Products_ProductName] ON [dbo].[Products];
DROP INDEX IF EXISTS [IX_MarketProductPrices_ProductID] ON [dbo].[MarketProductPrices];
-- etc...
```

## Notes

- ✅ Index oluşturma işlemi **NON-BLOCKING** - Production'da çalışırken yapılabilir
- ✅ Kategori düzeltmeleri indexleri etkilemez - otomatik güncellenir
- ⚠️ Indexler disk alanı kullanır (minimal, ~5-10MB tahmin)
- ⚠️ INSERT/UPDATE/DELETE işlemleri hafif yavaşlar (ihmal edilebilir)

## Support

Sorun yaşarsanız:
1. İlk olarak `01_Check_Existing_Indexes.sql` çalıştırın
2. Error mesajlarını kontrol edin
3. Index oluşturma loglarını inceleyin
