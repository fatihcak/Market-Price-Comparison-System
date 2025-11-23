CREATE TABLE UserProductList (
    ListID INT IDENTITY(1,1) PRIMARY KEY,
    SessionID NVARCHAR(100) NOT NULL,  -- kullanıcıyı tanımlamak için (örnek: tarayıcı token, cookie id)
    ProductID INT NOT NULL FOREIGN KEY REFERENCES Product(ProductID),
    AddedDate DATETIME DEFAULT GETDATE()
);
