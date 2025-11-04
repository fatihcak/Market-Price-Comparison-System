--market ile ilgili stored procedureler
CREATE PROCEDURE AddMarket
    @MarketName NVARCHAR(100),
    @WebsiteURL NVARCHAR(255)
AS
BEGIN
    INSERT INTO Market (MarketName, WebsiteURL)
    VALUES (@MarketName, @WebsiteURL);
END;
GO
CREATE PROCEDURE UpdateMarket
    @MarketID INT,
    @MarketName NVARCHAR(100),
    @WebsiteURL NVARCHAR(255)
AS
BEGIN
    UPDATE Market
    SET MarketName = @MarketName,
        WebsiteURL = @WebsiteURL
    WHERE MarketID = @MarketID;
END;
GO
CREATE PROCEDURE DeleteMarket
    @MarketID INT
AS
BEGIN
    DELETE FROM Market
    WHERE MarketID = @MarketID;
END;
GO
CREATE PROCEDURE GetAllMarkets
AS
BEGIN
    SELECT MarketID, MarketName, WebsiteURL
    FROM Market
    ORDER BY MarketName;
END;
