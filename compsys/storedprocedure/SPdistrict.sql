CREATE PROCEDURE AddDistrict
    @CityID INT,
    @DistrictName NVARCHAR(100)
AS
BEGIN
    INSERT INTO District (CityID, DistrictName)
    VALUES (@CityID, @DistrictName);
END;
GO
CREATE PROCEDURE UpdateDistrict
    @DistrictID INT,
    @CityID INT,
    @DistrictName NVARCHAR(100)
AS
BEGIN
    UPDATE District
    SET CityID = @CityID,
        DistrictName = @DistrictName
    WHERE DistrictID = @DistrictID;
END;
GO
CREATE PROCEDURE DeleteDistrict
    @DistrictID INT
AS
BEGIN
    DELETE FROM District
    WHERE DistrictID = @DistrictID;
END;
GO
CREATE PROCEDURE GetAllDistricts
AS
BEGIN
    SELECT D.DistrictID, D.DistrictName, C.CityName
    FROM District D
    INNER JOIN City C ON D.CityID = C.CityID
    ORDER BY C.CityName, D.DistrictName;
END;
