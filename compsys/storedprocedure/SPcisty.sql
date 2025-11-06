CREATE PROCEDURE AddCity
    @CityName NVARCHAR(100)
AS
BEGIN
    INSERT INTO City (CityName)
    VALUES (@CityName);
END;
GO
CREATE PROCEDURE UpdateCity
    @CityID INT,
    @CityName NVARCHAR(100)
AS
BEGIN
    UPDATE City
    SET CityName = @CityName
    WHERE CityID = @CityID;
END;
GO
CREATE PROCEDURE DeleteCity
    @CityID INT
AS
BEGIN
    DELETE FROM City
    WHERE CityID = @CityID;
END;
GO
CREATE PROCEDURE GetAllCities
AS
BEGIN
    SELECT CityID, CityName
    FROM City
    ORDER BY CityName;
END;

