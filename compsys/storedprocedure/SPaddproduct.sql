CREATE PROCEDURE AddProductToUserList
    @SessionID NVARCHAR(100),
    @ProductID INT
AS
BEGIN
    IF EXISTS (
        SELECT 1 FROM UserProductList
        WHERE SessionID = @SessionID AND ProductID = @ProductID
    )
    BEGIN
        UPDATE UserProductList
        SET Quantity = Quantity + 1
        WHERE SessionID = @SessionID AND ProductID = @ProductID;
    END
    ELSE
    BEGIN
        INSERT INTO UserProductList (SessionID, ProductID, Quantity)
        VALUES (@SessionID, @ProductID, 1);
    END;
END;
