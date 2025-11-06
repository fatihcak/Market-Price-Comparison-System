CREATE PROCEDURE GetPricesByProductAndDistrict
    @ProductID INT,
    @DistrictID INT
AS
BEGIN
    SELECT 
        MarketProductPrice.PriceID,
        Market.MarketName,
        Product.ProductName,
        Product.Brand,
        City.CityName,
        District.DistrictName,
        MarketProductPrice.Price,
        MarketProductPrice.LastUpdated
    FROM MarketProductPrice
    INNER JOIN Market ON MarketProductPrice.MarketID = Market.MarketID
    INNER JOIN Product ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN District ON MarketProductPrice.DistrictID = District.DistrictID
    INNER JOIN City ON District.CityID = City.CityID
    WHERE MarketProductPrice.ProductID = @ProductID
      AND MarketProductPrice.DistrictID = @DistrictID
    ORDER BY MarketProductPrice.Price ASC;
END;
GO
CREATE PROCEDURE GetLowestPriceByProductAndDistrict
    @ProductID INT,
    @DistrictID INT
AS
BEGIN
    SELECT TOP 1 
        Market.MarketName,
        Product.ProductName,
        Product.Brand,
        City.CityName,
        District.DistrictName,
        MarketProductPrice.Price,
        MarketProductPrice.LastUpdated
    FROM MarketProductPrice
    INNER JOIN Market ON MarketProductPrice.MarketID = Market.MarketID
    INNER JOIN Product ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN District ON MarketProductPrice.DistrictID = District.DistrictID
    INNER JOIN City ON District.CityID = City.CityID
    WHERE MarketProductPrice.ProductID = @ProductID
      AND MarketProductPrice.DistrictID = @DistrictID
    ORDER BY MarketProductPrice.Price ASC;
END;
GO
CREATE PROCEDURE GetAveragePriceByProductSmart
    @ProductID INT,
    @CityID INT = NULL
AS
BEGIN
    SELECT 
        Product.ProductName,
        Product.Brand,
        City.CityName,
        AVG(MarketProductPrice.Price) AS AveragePrice
    FROM MarketProductPrice
    INNER JOIN Product ON MarketProductPrice.ProductID = Product.ProductID
    INNER JOIN District ON MarketProductPrice.DistrictID = District.DistrictID
    INNER JOIN City ON District.CityID = City.CityID
    WHERE MarketProductPrice.ProductID = @ProductID
      AND (@CityID IS NULL OR City.CityID = @CityID)
    GROUP BY Product.ProductName, Product.Brand, City.CityName;
END;
GO
