CREATE PROCEDURE GetDistrictsByCity
    @CityID INT
AS
BEGIN
    SELECT DistrictID, DistrictName
    FROM District
    WHERE CityID = @CityID
    ORDER BY DistrictName;
END;
GO
CREATE PROCEDURE GetMarketsByDistrict
    @DistrictID INT
AS
BEGIN
    SELECT DISTINCT 
        M.MarketID, 
        M.MarketName
    FROM MarketProductPrice MPP
    INNER JOIN Market M ON MPP.MarketID = M.MarketID
    WHERE MPP.DistrictID = @DistrictID
    ORDER BY M.MarketName;
END;
GO
CREATE PROCEDURE GetAvailableProductsByDistrict
    @DistrictID INT
AS
BEGIN
    SELECT DISTINCT 
        Product.ProductID,
        Product.ProductName,
        Product.Brand,
        ProductCategory.CategoryName
    FROM MarketProductPrice
    INNER JOIN Product ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN ProductCategory ON Product.CategoryID = ProductCategory.CategoryID
    WHERE MarketProductPrice.DistrictID = @DistrictID
    ORDER BY ProductCategory.CategoryName, Product.ProductName;
END;
EXEC GetAvailableProductsByDistrict @DistrictID = 34;
