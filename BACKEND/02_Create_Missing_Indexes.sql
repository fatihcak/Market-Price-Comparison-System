-- =====================================================
-- CREATE MISSING INDEXES FOR PERFORMANCE OPTIMIZATION
-- AWS RDS SQL Server - Market Price Comparison System
-- Run this AFTER checking existing indexes
-- =====================================================

USE compsys;
GO

PRINT '========================================';
PRINT 'CREATING MISSING INDEXES';
PRINT '========================================';
PRINT '';

-- =====================================================
-- PRODUCTS TABLE INDEXES
-- =====================================================
PRINT 'Creating indexes on Products table...';

-- Index for CategoryID (used in JOIN with ProductCategories)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_CategoryID' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_CategoryID] 
    ON [dbo].[Products] ([CategoryID])
    INCLUDE ([ProductName], [Brand], [ImageUrl]);
    PRINT '✓ Created IX_Products_CategoryID';
END
ELSE
    PRINT '  IX_Products_CategoryID already exists';

-- Index for ProductName (used in searches and ORDER BY)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_ProductName' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_ProductName] 
    ON [dbo].[Products] ([ProductName])
    INCLUDE ([CategoryID], [Brand]);
    PRINT '✓ Created IX_Products_ProductName';
END
ELSE
    PRINT '  IX_Products_ProductName already exists';

-- Index for Brand (used in filtering)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Brand' AND object_id = OBJECT_ID('Products'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Products_Brand] 
    ON [dbo].[Products] ([Brand])
    WHERE [Brand] IS NOT NULL;
    PRINT '✓ Created IX_Products_Brand';
END
ELSE
    PRINT '  IX_Products_Brand already exists';

PRINT '';

-- =====================================================
-- MARKETPRODUCTPRICES TABLE INDEXES
-- =====================================================
PRINT 'Creating indexes on MarketProductPrices table...';

-- Index for ProductID (CRITICAL - used in JOIN with Products)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarketProductPrices_ProductID' AND object_id = OBJECT_ID('MarketProductPrices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MarketProductPrices_ProductID] 
    ON [dbo].[MarketProductPrices] ([ProductID])
    INCLUDE ([MarketID], [Price], [DistrictID], [LastUpdated]);
    PRINT '✓ Created IX_MarketProductPrices_ProductID';
END
ELSE
    PRINT '  IX_MarketProductPrices_ProductID already exists';

-- Index for MarketID (CRITICAL - used in JOIN with Markets)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarketProductPrices_MarketID' AND object_id = OBJECT_ID('MarketProductPrices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MarketProductPrices_MarketID] 
    ON [dbo].[MarketProductPrices] ([MarketID])
    INCLUDE ([ProductID], [Price], [LastUpdated]);
    PRINT '✓ Created IX_MarketProductPrices_MarketID';
END
ELSE
    PRINT '  IX_MarketProductPrices_MarketID already exists';

-- Index for DistrictID (used in filtering by location)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarketProductPrices_DistrictID' AND object_id = OBJECT_ID('MarketProductPrices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MarketProductPrices_DistrictID] 
    ON [dbo].[MarketProductPrices] ([DistrictID]);
    PRINT '✓ Created IX_MarketProductPrices_DistrictID';
END
ELSE
    PRINT '  IX_MarketProductPrices_DistrictID already exists';

-- Composite index for ProductID + DistrictID (used together in queries)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarketProductPrices_ProductID_DistrictID' AND object_id = OBJECT_ID('MarketProductPrices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MarketProductPrices_ProductID_DistrictID] 
    ON [dbo].[MarketProductPrices] ([ProductID], [DistrictID])
    INCLUDE ([MarketID], [Price]);
    PRINT '✓ Created IX_MarketProductPrices_ProductID_DistrictID';
END
ELSE
    PRINT '  IX_MarketProductPrices_ProductID_DistrictID already exists';

-- Index for LastUpdated (used in date filtering)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarketProductPrices_LastUpdated' AND object_id = OBJECT_ID('MarketProductPrices'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_MarketProductPrices_LastUpdated] 
    ON [dbo].[MarketProductPrices] ([LastUpdated] DESC);
    PRINT '✓ Created IX_MarketProductPrices_LastUpdated';
END
ELSE
    PRINT '  IX_MarketProductPrices_LastUpdated already exists';

PRINT '';

-- =====================================================
-- DISTRICTS TABLE INDEXES
-- =====================================================
PRINT 'Creating indexes on Districts table...';

-- Index for CityID (used in JOIN with Cities)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Districts_CityID' AND object_id = OBJECT_ID('Districts'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Districts_CityID] 
    ON [dbo].[Districts] ([CityID]);
    PRINT '✓ Created IX_Districts_CityID';
END
ELSE
    PRINT '  IX_Districts_CityID already exists';

-- Index for DistrictName (used in searches)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Districts_DistrictName' AND object_id = OBJECT_ID('Districts'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Districts_DistrictName] 
    ON [dbo].[Districts] ([DistrictName]);
    PRINT '✓ Created IX_Districts_DistrictName';
END
ELSE
    PRINT '  IX_Districts_DistrictName already exists';

PRINT '';

-- =====================================================
-- MARKETS TABLE INDEXES
-- =====================================================
PRINT 'Creating indexes on Markets table...';

-- Index for MarketName (used in searches)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Markets_MarketName' AND object_id = OBJECT_ID('Markets'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Markets_MarketName] 
    ON [dbo].[Markets] ([MarketName]);
    PRINT '✓ Created IX_Markets_MarketName';
END
ELSE
    PRINT '  IX_Markets_MarketName already exists';

PRINT '';

-- =====================================================
-- USERPRODUCTLISTS TABLE INDEXES
-- =====================================================
PRINT 'Creating indexes on UserProductLists table...';

-- Index for ProductID (used in JOIN with Products)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserProductLists_ProductID' AND object_id = OBJECT_ID('UserProductLists'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UserProductLists_ProductID] 
    ON [dbo].[UserProductLists] ([ProductID]);
    PRINT '✓ Created IX_UserProductLists_ProductID';
END
ELSE
    PRINT '  IX_UserProductLists_ProductID already exists';

-- Index for SessionID (used in user session queries)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserProductLists_SessionID' AND object_id = OBJECT_ID('UserProductLists'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_UserProductLists_SessionID] 
    ON [dbo].[UserProductLists] ([SessionID]);
    PRINT '✓ Created IX_UserProductLists_SessionID';
END
ELSE
    PRINT '  IX_UserProductLists_SessionID already exists';

-- Unique composite index for SessionID + ProductID
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_UserProductLists_SessionID_ProductID' AND object_id = OBJECT_ID('UserProductLists'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_UserProductLists_SessionID_ProductID] 
    ON [dbo].[UserProductLists] ([SessionID], [ProductID]);
    PRINT '✓ Created IX_UserProductLists_SessionID_ProductID';
END
ELSE
    PRINT '  IX_UserProductLists_SessionID_ProductID already exists';

PRINT '';

-- =====================================================
-- UPDATE STATISTICS
-- =====================================================
PRINT 'Updating statistics...';

UPDATE STATISTICS [dbo].[Products];
UPDATE STATISTICS [dbo].[MarketProductPrices];
UPDATE STATISTICS [dbo].[Markets];
UPDATE STATISTICS [dbo].[ProductCategories];
UPDATE STATISTICS [dbo].[Districts];
UPDATE STATISTICS [dbo].[UserProductLists];

PRINT '✓ Statistics updated';

PRINT '';
PRINT '========================================';
PRINT 'INDEX CREATION COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Expected Performance Improvement:';
PRINT '  - Product loading: 50s → 2-5s';
PRINT '  - JOIN operations: 10-100x faster';
PRINT '  - Search queries: 50-100x faster';
PRINT '';
PRINT 'Next Steps:';
PRINT '  1. Test product loading on frontend';
PRINT '  2. Monitor query performance';
PRINT '  3. Consider adding pagination if still slow';
