--listede yer alan tüm ürünlerin toplam fiyatı en düşük olan market
CREATE PROCEDURE GetCheapestMarketForUserList
    @SessionID NVARCHAR(100),
    @DistrictID INT
AS
BEGIN
    SELECT TOP 1
        Market.MarketName,
        SUM(MarketProductPrice.Price) AS TotalPrice,
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
EXEC AddProductToUserList @SessionID = 'session_12345', @ProductID = 1;
EXEC AddProductToUserList @SessionID = 'session_12345', @ProductID = 2;
EXEC AddProductToUserList @SessionID = 'session_12345', @ProductID = 4;
SELECT * FROM UserProductList WHERE SessionID = 'session_12345';
go
EXEC GetCheapestMarketForUserList 
    @SessionID = 'session_12345',
    @DistrictID = 34;


