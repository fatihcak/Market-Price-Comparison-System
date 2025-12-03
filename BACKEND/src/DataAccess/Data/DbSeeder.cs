using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Apply migrations if any
        // await context.Database.MigrateAsync(); // Not supported by InMemory

        // Check if data exists
        if (await context.ProductCategories.AnyAsync())
        {
            return; // Data already seeded
        }

        // 1. Categories
        var categories = new List<ProductCategory>
        {
            new() { CategoryName = "Dairy Products", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Fruits", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Vegetables", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Bread & Grains", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Oils", CreatedAt = DateTime.UtcNow }
        };
        await context.ProductCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        // 2. Products
        var dairy = categories.First(c => c.CategoryName == "Dairy Products");
        var fruits = categories.First(c => c.CategoryName == "Fruits");
        var vegetables = categories.First(c => c.CategoryName == "Vegetables");
        var grains = categories.First(c => c.CategoryName == "Bread & Grains");
        var oils = categories.First(c => c.CategoryName == "Oils");

        var products = new List<Product>
        {
            // Fruits & Vegetables & Others matching products.ts order for ID alignment
            // ID 1: Organik Süt 1L
            new() { ProductName = "Organik Süt", Brand = "Sütaş", Unit = "1L", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 2: Taze Ekmek
            new() { ProductName = "Taze Ekmek", Brand = "Halk Ekmek", Unit = "Adet", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 3: Yumurta 10lu
            new() { ProductName = "Yumurta", Brand = "Köy Yumurtası", Unit = "10lu", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 4: Zeytin Yağı 500ml
            new() { ProductName = "Zeytin Yağı", Brand = "Komili", Unit = "500ml", CategoryId = oils.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 5: Beyaz Peynir 500g
            new() { ProductName = "Beyaz Peynir", Brand = "Pınar", Unit = "500g", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 6: Domates 1kg
            new() { ProductName = "Domates", Brand = "Salkım", Unit = "1kg", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 7: Salatalık 1kg
            new() { ProductName = "Salatalık", Brand = "Çengelköy", Unit = "1kg", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 8: Sarımsak 500g
            new() { ProductName = "Sarımsak", Brand = "Yerli", Unit = "500g", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 9: Elma 1kg
            new() { ProductName = "Elma", Brand = "Amasya", Unit = "1kg", CategoryId = fruits.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            // ID 10: Muz 1kg
            new() { ProductName = "Muz", Brand = "İthal", Unit = "1kg", CategoryId = fruits.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow },
            new() { ProductName = "aleynfıstık", Brand = "İthal", Unit = "10kg", CategoryId = fruits.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow }
        };
        await context.Products.AddRangeAsync(products);
        await context.SaveChangesAsync();

        // 3. Markets
        var markets = new List<Market>
        {
            new() { MarketName = "Migros", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2b/Migros_logo.svg/2560px-Migros_logo.svg.png", CreatedAt = DateTime.UtcNow },
            new() { MarketName = "A101", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/36/A101_logo.svg/1200px-A101_logo.svg.png", CreatedAt = DateTime.UtcNow },
            new() { MarketName = "Bim", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c2/BİM_logo.svg/1200px-BİM_logo.svg.png", CreatedAt = DateTime.UtcNow },
            new() { MarketName = "CarrefourSA", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Carrefour_logo.svg/1200px-Carrefour_logo.svg.png", CreatedAt = DateTime.UtcNow }
        };
        await context.Markets.AddRangeAsync(markets);
        await context.SaveChangesAsync();

        // 4. Cities & Districts
        var city = new City { CityName = "Istanbul", CreatedAt = DateTime.UtcNow };
        await context.Cities.AddAsync(city);
        await context.SaveChangesAsync();

        var districts = new List<District>
        {
            new() { DistrictName = "Kadikoy", CityId = city.Id, CreatedAt = DateTime.UtcNow },
            new() { DistrictName = "Besiktas", CityId = city.Id, CreatedAt = DateTime.UtcNow },
            new() { DistrictName = "Sisli", CityId = city.Id, CreatedAt = DateTime.UtcNow }
        };
        await context.Districts.AddRangeAsync(districts);
        await context.SaveChangesAsync();

        // 5. Prices
        var random = new Random();
        var prices = new List<MarketProductPrice>();

        foreach (var product in products)
        {
            foreach (var market in markets)
            {
                foreach (var district in districts)
                {
                    // Always add price for testing
                    // if (random.NextDouble() > 0.2)
                    {
                        var basePrice = 10 + random.Next(100); // Random price between 10 and 110
                        prices.Add(new MarketProductPrice
                        {
                            ProductId = product.Id,
                            MarketId = market.Id,
                            DistrictId = district.Id,
                            Price = (decimal)(basePrice + random.NextDouble() * 5), // Add some cents
                            LastUpdated = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }
        await context.MarketProductPrices.AddRangeAsync(prices);
        await context.SaveChangesAsync();
    }
}
