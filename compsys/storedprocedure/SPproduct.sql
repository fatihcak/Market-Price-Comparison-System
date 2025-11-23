CREATE PROCEDURE AddProduct
    @CategoryID INT,
    @ProductName NVARCHAR(150),
    @Brand NVARCHAR(100),
    @Unit NVARCHAR(20)
AS
BEGIN
    INSERT INTO Product (CategoryID, ProductName, Brand, Unit)
    VALUES (@CategoryID, @ProductName, @Brand, @Unit);
END;
GO
CREATE PROCEDURE DeleteProduct
    @ProductID INT
AS
BEGIN
    DELETE FROM Product
    WHERE ProductID = @ProductID;
END;
GO
CREATE PROCEDURE GetAllProducts
AS
BEGIN
    SELECT 
        P.ProductID, 
        P.ProductName, 
        P.Brand, 
        P.Unit, 
        C.CategoryName, 
        P.LastUpdated
    FROM Product P
    INNER JOIN ProductCategory C ON P.CategoryID = C.CategoryID
    ORDER BY C.CategoryName, P.ProductName;
END;
GO
