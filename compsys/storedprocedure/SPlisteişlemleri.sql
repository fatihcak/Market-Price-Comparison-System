CREATE OR ALTER PROCEDURE RemoveProductFromUserList
    @SessionID NVARCHAR(100),
    @ProductID INT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM UserProductList
        WHERE SessionID = @SessionID AND ProductID = @ProductID
    )
    BEGIN
        DECLARE @Quantity INT;
        SELECT @Quantity = Quantity 
        FROM UserProductList 
        WHERE SessionID = @SessionID AND ProductID = @ProductID;

        IF @Quantity > 1
        BEGIN
            UPDATE UserProductList
            SET Quantity = Quantity - 1
            WHERE SessionID = @SessionID AND ProductID = @ProductID;
        END
        ELSE
        BEGIN
            DELETE FROM UserProductList
            WHERE SessionID = @SessionID AND ProductID = @ProductID;
        END
    END
END;
GO
--kullanıcının seçtiği miktarlara göre toplam fiyat
CREATE OR ALTER PROCEDURE GetPriceComparisonForUserList
    @SessionID NVARCHAR(100),
    @DistrictID INT
AS
BEGIN
    SELECT 
        Product.ProductID,
        Product.ProductName,
        Product.Brand,
        Product.Unit,
        ProductCategory.CategoryName,
        Market.MarketName,
        MarketProductPrice.Price,
        UserProductList.Quantity,
        (MarketProductPrice.Price * UserProductList.Quantity) AS TotalProductPrice,
        District.DistrictName,
        City.CityName,
        MarketProductPrice.LastUpdated
    FROM UserProductList
    INNER JOIN Product ON UserProductList.ProductID = Product.ProductID
    INNER JOIN ProductCategory ON Product.CategoryID = ProductCategory.CategoryID
    INNER JOIN MarketProductPrice ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN Market ON MarketProductPrice.MarketID = Market.MarketID
    INNER JOIN District ON MarketProductPrice.DistrictID = District.DistrictID
    INNER JOIN City ON District.CityID = City.CityID
    WHERE UserProductList.SessionID = @SessionID
      AND MarketProductPrice.DistrictID = @DistrictID
    ORDER BY Product.ProductName, MarketProductPrice.Price ASC;
END;
GO
CREATE OR ALTER PROCEDURE GetCheapestMarketForUserList
    @SessionID NVARCHAR(100),
    @DistrictID INT
AS
BEGIN
    SELECT TOP 1
        Market.MarketName,
        SUM(MarketProductPrice.Price * UserProductList.Quantity) AS TotalPrice,
        COUNT(DISTINCT Product.ProductID) AS ProductCount
    FROM UserProductList
    INNER JOIN Product ON UserProductList.ProductID = Product.ProductID
    INNER JOIN MarketProductPrice ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN Market ON MarketProductPrice.MarketID = Market.MarketID
    WHERE UserProductList.SessionID = @SessionID
      AND MarketProductPrice.DistrictID = @DistrictID
    GROUP BY Market.MarketName
    ORDER BY TotalPrice ASC;
END;
GO
EXEC GetCheapestMarketForUserList 
    @SessionID = 'session_12345',
    @DistrictID = 34;
go
CREATE OR ALTER PROCEDURE GetAllMarketComparisonsForUserList
    @SessionID NVARCHAR(100),
    @DistrictID INT
AS
BEGIN
    SELECT 
        Market.MarketName,
        SUM(MarketProductPrice.Price * UserProductList.Quantity) AS TotalPrice,
        COUNT(DISTINCT Product.ProductID) AS ProductCount
    FROM UserProductList
    INNER JOIN Product ON UserProductList.ProductID = Product.ProductID
    INNER JOIN MarketProductPrice ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN Market ON MarketProductPrice.MarketID = Market.MarketID
    WHERE UserProductList.SessionID = @SessionID
      AND MarketProductPrice.DistrictID = @DistrictID
    GROUP BY Market.MarketName
    ORDER BY TotalPrice ASC;
END;
GO
CREATE OR ALTER PROCEDURE GetAllMarketComparisonsForUserList_FullMatch
    @SessionID NVARCHAR(100),
    @DistrictID INT
AS
BEGIN
    -- Kullanıcının listedeki toplam ürün sayısını bul
    DECLARE @TotalProducts INT;

    SELECT @TotalProducts = COUNT(DISTINCT ProductID)
    FROM UserProductList
    WHERE SessionID = @SessionID;

    -- Market karşılaştırması (sadece tüm ürünleri içeren marketler)
    SELECT 
        Market.MarketName,
        SUM(MarketProductPrice.Price * UserProductList.Quantity) AS TotalPrice,
        COUNT(DISTINCT Product.ProductID) AS ProductCount
    FROM UserProductList
    INNER JOIN Product ON UserProductList.ProductID = Product.ProductID
    INNER JOIN MarketProductPrice ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN Market ON MarketProductPrice.MarketID = Market.MarketID
    WHERE UserProductList.SessionID = @SessionID
      AND MarketProductPrice.DistrictID = @DistrictID
    GROUP BY Market.MarketName
    HAVING COUNT(DISTINCT Product.ProductID) = @TotalProducts
    ORDER BY TotalPrice ASC;
END;
GO
CREATE OR ALTER PROCEDURE GetAllMarketComparisonsWithMissingProducts
    @SessionID NVARCHAR(100),
    @DistrictID INT
AS
BEGIN
    -- Kullanıcının listesindeki toplam ürün sayısını bul
    DECLARE @TotalProducts INT;

    SELECT @TotalProducts = COUNT(DISTINCT ProductID)
    FROM UserProductList
    WHERE SessionID = @SessionID;

    -- Tüm marketlerde fiyat karşılaştırması (eksik ürünleri dahil)
    SELECT 
        Market.MarketName,
        COUNT(DISTINCT MarketProductPrice.ProductID) AS AvailableProducts,
        @TotalProducts - COUNT(DISTINCT MarketProductPrice.ProductID) AS MissingProducts,
        SUM(MarketProductPrice.Price * UserProductList.Quantity) AS TotalPrice
    FROM Market
    INNER JOIN MarketProductPrice 
        ON Market.MarketID = MarketProductPrice.MarketID
    INNER JOIN Product 
        ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN UserProductList 
        ON UserProductList.ProductID = Product.ProductID
    WHERE UserProductList.SessionID = @SessionID
      AND MarketProductPrice.DistrictID = @DistrictID
    GROUP BY Market.MarketName
    ORDER BY MissingProducts ASC, TotalPrice ASC;
END;
GO
EXEC GetAllMarketComparisonsWithMissingProducts 
    @SessionID = 'session_12345',
    @DistrictID = 34;
