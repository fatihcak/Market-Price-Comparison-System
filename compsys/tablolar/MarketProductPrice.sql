CREATE TABLE MarketProductPrice (
    PriceID INT IDENTITY(1,1) PRIMARY KEY,
    MarketID INT NOT NULL FOREIGN KEY REFERENCES Market(MarketID),
    ProductID INT NOT NULL FOREIGN KEY REFERENCES Product(ProductID),
    DistrictID INT NOT NULL FOREIGN KEY REFERENCES District(DistrictID),
    Price DECIMAL(10,2) NOT NULL,
    LastUpdated DATETIME DEFAULT GETDATE()
);
GO
-- Örnek fiyat verileri
INSERT INTO MarketProductPrice (MarketID, ProductID, DistrictID, Price)
VALUES
(1, 1, 34, 44.90),   -- BİM - Pirinç 1 Kg
(2, 1, 34, 45.50),   -- ŞOK - Pirinç 1 Kg
(3, 1, 34, 43.75),   -- A101 - Pirinç 1 Kg
(1, 2, 34, 89.90),   -- BİM - Ayçiçek Yağı 1 L
(2, 2, 34, 87.50),   -- ŞOK - Ayçiçek Yağı 1 L
(3, 2, 34, 90.00),   -- A101 - Ayçiçek Yağı 1 L
(1, 3, 34, 22.90),   -- BİM - Yoğurt 1 Kg
(2, 3, 34, 24.50),   -- ŞOK - Yoğurt 1 Kg
(3, 3, 34, 23.25),   -- A101 - Yoğurt 1 Kg
(1, 4, 34, 119.90),  -- BİM - Tereyağı 500 g
(2, 4, 34, 124.50),  -- ŞOK - Tereyağı 500 g
(3, 4, 34, 118.75);  -- A101 - Tereyağı 500 g
