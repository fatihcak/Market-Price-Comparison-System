CREATE PROCEDURE AddMarketProductPrice
    @MarketID INT,
    @ProductID INT,
    @DistrictID INT,
    @Price DECIMAL(10,2)
AS
BEGIN
    INSERT INTO MarketProductPrice (MarketID, ProductID, DistrictID, Price)
    VALUES (@MarketID, @ProductID, @DistrictID, @Price);
END;
GO
CREATE PROCEDURE UpdateMarketProductPrice
    @PriceID INT,
    @MarketID INT,
    @ProductID INT,
    @DistrictID INT,
    @Price DECIMAL(10,2)
AS
BEGIN
    UPDATE MarketProductPrice
    SET 
        MarketID = @MarketID,
        ProductID = @ProductID,
        DistrictID = @DistrictID,
        Price = @Price,
        LastUpdated = GETDATE()
    WHERE PriceID = @PriceID;
END;
GO
CREATE PROCEDURE DeleteMarketProductPrice
    @PriceID INT
AS
BEGIN
    DELETE FROM MarketProductPrice
    WHERE PriceID = @PriceID;
END;
GO
CREATE PROCEDURE GetAllMarketProductPrices
AS
BEGIN
    SELECT 
        MPP.PriceID,
        M.MarketName,
        P.ProductName,
        P.Brand,
        D.DistrictName,
        C.CityName,
        MPP.Price,
        MPP.LastUpdated
    FROM MarketProductPrice MPP
    INNER JOIN Market M ON MPP.MarketID = M.MarketID
    INNER JOIN Product P ON MPP.ProductID = P.ProductID
    INNER JOIN District D ON MPP.DistrictID = D.DistrictID
    INNER JOIN City C ON D.CityID = C.CityID
    ORDER BY C.CityName, D.DistrictName, P.ProductName;
END;
