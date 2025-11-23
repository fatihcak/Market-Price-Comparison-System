CREATE TABLE ProductCategory (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    CategoryName NVARCHAR(100) NOT NULL
);

INSERT INTO ProductCategory (CategoryName) VALUES
('Temel Gıda'),
('Süt ve Kahvaltılık Ürünler'),
('İçecekler'),
('Meyve ve Sebze'),
('Et, Tavuk ve Balık'),
('Dondurulmuş Ürünler'),
('Atıştırmalıklar'),
('Temizlik Ürünleri'),
('Kişisel Bakım'),
('Ev ve Mutfak Gereçleri'),
('Bebek Ürünleri'),
('Evcil Hayvan Ürünleri');
