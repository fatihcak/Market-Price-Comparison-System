CREATE PROCEDURE AddProductCategory
    @CategoryName NVARCHAR(100)
AS
BEGIN
    INSERT INTO ProductCategory (CategoryName)
    VALUES (@CategoryName);
END;
GO
CREATE PROCEDURE UpdateProductCategory
    @CategoryID INT,
    @CategoryName NVARCHAR(100)
AS
BEGIN
    UPDATE ProductCategory
    SET CategoryName = @CategoryName
    WHERE CategoryID = @CategoryID;
END;
GO
CREATE PROCEDURE DeleteProductCategory
    @CategoryID INT
AS
BEGIN
    DELETE FROM ProductCategory
    WHERE CategoryID = @CategoryID;
END;
GO
CREATE PROCEDURE GetAllProductCategories
AS
BEGIN
    SELECT CategoryID, CategoryName
    FROM ProductCategory
    ORDER BY CategoryName;
END;
