CREATE TABLE Market (
    MarketID INT IDENTITY(1,1) PRIMARY KEY,
    MarketName NVARCHAR(100) NOT NULL,
    WebsiteURL NVARCHAR(255)
);

INSERT INTO Market (MarketName, WebsiteURL) VALUES
('BİM', 'https://www.bim.com.tr'),
('ŞOK', 'https://www.sokmarket.com.tr'),
('A101', 'https://www.a101.com.tr'),
('Migros', 'https://www.migros.com.tr'),
('CarrefourSA', 'https://www.carrefoursa.com');
