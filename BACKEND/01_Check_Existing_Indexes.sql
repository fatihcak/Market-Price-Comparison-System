-- =====================================================
-- CHECK EXISTING INDEXES ON AWS RDS DATABASE
-- Run this first to see what indexes already exist
-- =====================================================

USE compsys;
GO

PRINT '========================================';
PRINT 'EXISTING INDEXES ON CRITICAL TABLES';
PRINT '========================================';
PRINT '';

-- Show all indexes on Products table
PRINT '--- PRODUCTS TABLE ---';
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(COL_NAME(ic.object_id, ic.column_id), ', ') 
        WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns,
    i.is_unique AS IsUnique
FROM 
    sys.indexes i
INNER JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    i.object_id = OBJECT_ID('Products')
    AND i.type > 0  -- Exclude heaps
GROUP BY
    i.name, i.type_desc, i.is_unique
ORDER BY 
    i.name;

PRINT '';

-- Show all indexes on MarketProductPrices table
PRINT '--- MARKETPRODUCTPRICES TABLE ---';
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(COL_NAME(ic.object_id, ic.column_id), ', ') 
        WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns,
    i.is_unique AS IsUnique
FROM 
    sys.indexes i
INNER JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    i.object_id = OBJECT_ID('MarketProductPrices')
    AND i.type > 0
GROUP BY
    i.name, i.type_desc, i.is_unique
ORDER BY 
    i.name;

PRINT '';

-- Show all indexes on Markets table
PRINT '--- MARKETS TABLE ---';
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(COL_NAME(ic.object_id, ic.column_id), ', ') 
        WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns,
    i.is_unique AS IsUnique
FROM 
    sys.indexes i
INNER JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    i.object_id = OBJECT_ID('Markets')
    AND i.type > 0
GROUP BY
    i.name, i.type_desc, i.is_unique
ORDER BY 
    i.name;

PRINT '';

-- Show all indexes on ProductCategories table
PRINT '--- PRODUCTCATEGORIES TABLE ---';
SELECT 
    i.name AS IndexName,
    i.type_desc AS IndexType,
    STRING_AGG(COL_NAME(ic.object_id, ic.column_id), ', ') 
        WITHIN GROUP (ORDER BY ic.key_ordinal) AS Columns,
    i.is_unique AS IsUnique
FROM 
    sys.indexes i
INNER JOIN 
    sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    i.object_id = OBJECT_ID('ProductCategories')
    AND i.type > 0
GROUP BY
    i.name, i.type_desc, i.is_unique
ORDER BY 
    i.name;

PRINT '';
PRINT '========================================';
PRINT 'TABLE ROW COUNTS';
PRINT '========================================';

-- Check table sizes
SELECT 'Products' AS TableName, COUNT(*) AS RowCount FROM Products;
SELECT 'MarketProductPrices' AS TableName, COUNT(*) AS RowCount FROM MarketProductPrices;
SELECT 'Markets' AS TableName, COUNT(*) AS RowCount FROM Markets;
SELECT 'ProductCategories' AS TableName, COUNT(*) AS RowCount FROM ProductCategories;
SELECT 'Districts' AS TableName, COUNT(*) AS RowCount FROM Districts;

PRINT '';
PRINT 'ANALYSIS COMPLETE';
