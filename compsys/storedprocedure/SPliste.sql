CREATE PROCEDURE AddProductToUserList
    @SessionID NVARCHAR(100),
    @ProductID INT
AS
BEGIN
    IF NOT EXISTS (
        SELECT 1 FROM UserProductList
        WHERE SessionID = @SessionID AND ProductID = @ProductID
    )
    BEGIN
        INSERT INTO UserProductList (SessionID, ProductID)
        VALUES (@SessionID, @ProductID);
    END;
END;
go
CREATE PROCEDURE RemoveProductFromUserList
    @SessionID NVARCHAR(100),
    @ProductID INT
AS
BEGIN
    DELETE FROM UserProductList
    WHERE SessionID = @SessionID AND ProductID = @ProductID;
END;
go
ALTER PROCEDURE GetUserProductList
    @SessionID NVARCHAR(100)
AS
BEGIN
    SELECT 
        UserProductList.ListID,
        Product.ProductID,
        Product.ProductName,
        Product.Brand,
        Product.Unit,
        ProductCategory.CategoryName,
        UserProductList.AddedDate
    FROM UserProductList
    INNER JOIN Product ON UserProductList.ProductID = Product.ProductID
    INNER JOIN ProductCategory ON Product.CategoryID = ProductCategory.CategoryID
    WHERE UserProductList.SessionID = @SessionID
    ORDER BY UserProductList.AddedDate DESC;
END;
GO
CREATE PROCEDURE ClearUserProductList
    @SessionID NVARCHAR(100)
AS
BEGIN
    DELETE FROM UserProductList
    WHERE SessionID = @SessionID;
END;
go
CREATE PROCEDURE GetPriceComparisonForUserList
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
EXEC GetPriceComparisonForUserList 
    @SessionID = 'session_12345',
    @DistrictID = 34;  -- Yalova / Merkez
