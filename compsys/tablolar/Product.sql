CREATE TABLE Product (
    ProductID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryID INT NOT NULL FOREIGN KEY REFERENCES ProductCategory(CategoryID),
    ProductName NVARCHAR(150) NOT NULL,
    Brand NVARCHAR(100),
    Unit NVARCHAR(20),
    LastUpdated DATETIME DEFAULT GETDATE()
);
GO
INSERT INTO Product (CategoryID, ProductName, Brand, Unit)
VALUES
(1, 'Pirinç 1 Kg', 'Yayla', 'kg'),
(1, 'Ayçiçek Yağı 1 L', 'Yudum', 'lt'),
(2, 'Yoğurt 1 Kg', 'Sütaş', 'kg'),
(2, 'Tereyağı 500 g', 'Pınar', 'g'),
(3, 'Gazoz 1 L', 'Uludağ', 'lt'),
(8, 'Çamaşır Deterjanı 2 Kg', 'Ariel', 'kg'),
(9, 'Diş Macunu 75 ml', 'Colgate', 'ml'),
(10, 'Tava 26 cm', 'Tefal', 'adet');
