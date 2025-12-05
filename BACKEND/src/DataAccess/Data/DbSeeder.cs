using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Apply migrations if any
        // await context.Database.MigrateAsync(); // Not supported by InMemory

        Console.WriteLine("--- SEEDING STARTED ---");

        // Always clear database for manual seeding testing
        context.MarketProductPrices.RemoveRange(context.MarketProductPrices);
        context.Products.RemoveRange(context.Products);
        context.Districts.RemoveRange(context.Districts);
        context.Cities.RemoveRange(context.Cities);
        context.Markets.RemoveRange(context.Markets);
        context.ProductCategories.RemoveRange(context.ProductCategories);
        await context.SaveChangesAsync();

        // 1. Categories
        var categories = new List<ProductCategory>
        {
            new() { CategoryName = "Dairy Products", Icon = "🥛", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Fruits", Icon = "🍎", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Vegetables", Icon = "🥦", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Bread & Grains", Icon = "🍞", CreatedAt = DateTime.UtcNow },
            new() { CategoryName = "Oils", Icon = "🌻", CreatedAt = DateTime.UtcNow }
        };
        await context.ProductCategories.AddRangeAsync(categories);
        await context.SaveChangesAsync();

        var dairy = categories.First(c => c.CategoryName == "Dairy Products");
        var fruits = categories.First(c => c.CategoryName == "Fruits");
        var vegetables = categories.First(c => c.CategoryName == "Vegetables");
        var grains = categories.First(c => c.CategoryName == "Bread & Grains");
        var oils = categories.First(c => c.CategoryName == "Oils");

        // 2. Markets
        var markets = new List<Market>
        {
            new() { MarketName = "Migros", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/2/2b/Migros_logo.svg/2560px-Migros_logo.svg.png", CreatedAt = DateTime.UtcNow },
            new() { MarketName = "A101", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/3/36/A101_logo.svg/1200px-A101_logo.svg.png", CreatedAt = DateTime.UtcNow },
            new() { MarketName = "Bim", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/c/c2/BİM_logo.svg/1200px-BİM_logo.svg.png", CreatedAt = DateTime.UtcNow },
            new() { MarketName = "CarrefourSA", LogoUrl = "https://upload.wikimedia.org/wikipedia/commons/thumb/4/44/Carrefour_logo.svg/1200px-Carrefour_logo.svg.png", CreatedAt = DateTime.UtcNow }
        };
        await context.Markets.AddRangeAsync(markets);
        await context.SaveChangesAsync();

        var migros = markets.First(m => m.MarketName == "Migros");
        var bim = markets.First(m => m.MarketName == "Bim");
        var a101 = markets.First(m => m.MarketName == "A101");
        var carrefour = markets.First(m => m.MarketName == "CarrefourSA");

        // 3. Cities & Districts
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

        var kadikoy = districts.First(d => d.DistrictName == "Kadikoy");
        var besiktas = districts.First(d => d.DistrictName == "Besiktas");
        var sisli = districts.First(d => d.DistrictName == "Sisli");

        // 4. Products & Prices (Manual Entry Section)
        var products = new List<Product>();
        var prices = new List<MarketProductPrice>();

        // --- 1. Pınar Süt (Example) ---
        var pinarSut = new Product { ProductName = "Süt", Brand = "Pınar", Unit = "1L", CategoryId = dairy.Id, ImageUrl = "https://marketkarsilastirma.com/images/sutas-sut.png", CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(pinarSut);

        // CarrefourSA - Kadikoy: 40 TL
        prices.Add(new MarketProductPrice { Product = pinarSut, MarketId = carrefour.Id, DistrictId = kadikoy.Id, Price = 40.00m, LastUpdated = DateTime.UtcNow });
        // Bim - Kadikoy: 35 TL
        prices.Add(new MarketProductPrice { Product = pinarSut, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 35.00m, LastUpdated = DateTime.UtcNow });
        // Migros - Sisli: 38 TL
        prices.Add(new MarketProductPrice { Product = pinarSut, MarketId = migros.Id, DistrictId = sisli.Id, Price = 38.00m, LastUpdated = DateTime.UtcNow });


        // --- 2. Halk Ekmek (Example) ---
        var ekmek = new Product { ProductName = "Ekmek", Brand = "Halk Ekmek", Unit = "Adet", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(ekmek);

        prices.Add(new MarketProductPrice { Product = ekmek, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 8.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = ekmek, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 10.00m, LastUpdated = DateTime.UtcNow });


        // --- ADD YOUR NEW PRODUCTS BELOW ---
        // Copy-paste the block above and change details
        var pirinç = new Product { ProductName = "Pirinç", Brand = "Baldo", Unit = "1Kg", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(pirinç);

        prices.Add(new MarketProductPrice { Product = pirinç, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 32.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = pirinç, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 40.00m, LastUpdated = DateTime.UtcNow });

        // --- 1. Yoğurt (Dairy) ---
        var yogurt = new Product { ProductName = "Yoğurt", Brand = "Sütaş", Unit = "2kg", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(yogurt);
        prices.Add(new MarketProductPrice { Product = yogurt, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 85.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = yogurt, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 79.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = yogurt, MarketId = a101.Id, DistrictId = kadikoy.Id, Price = 80.00m, LastUpdated = DateTime.UtcNow });

        // --- 2. Kaşar Peyniri (Dairy) ---
        var kasar = new Product { ProductName = "Kaşar Peyniri", Brand = "Torku", Unit = "400g", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(kasar);
        prices.Add(new MarketProductPrice { Product = kasar, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 140.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = kasar, MarketId = carrefour.Id, DistrictId = besiktas.Id, Price = 145.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = kasar, MarketId = bim.Id, DistrictId = besiktas.Id, Price = 130.00m, LastUpdated = DateTime.UtcNow });

        // --- 3. Tereyağı (Dairy) ---
        var tereyagi = new Product { ProductName = "Tereyağı", Brand = "Pınar", Unit = "250g", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(tereyagi);
        prices.Add(new MarketProductPrice { Product = tereyagi, MarketId = migros.Id, DistrictId = sisli.Id, Price = 95.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = tereyagi, MarketId = a101.Id, DistrictId = sisli.Id, Price = 89.00m, LastUpdated = DateTime.UtcNow });

        // --- 4. Ayran (Dairy) ---
        var ayran = new Product { ProductName = "Ayran", Brand = "Sütaş", Unit = "1L", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(ayran);
        prices.Add(new MarketProductPrice { Product = ayran, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 25.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = ayran, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 22.00m, LastUpdated = DateTime.UtcNow });

        // --- 5. Kefir (Dairy) ---
        var kefir = new Product { ProductName = "Kefir", Brand = "Altınkılıç", Unit = "1L", CategoryId = dairy.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(kefir);
        prices.Add(new MarketProductPrice { Product = kefir, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 45.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = kefir, MarketId = carrefour.Id, DistrictId = besiktas.Id, Price = 48.00m, LastUpdated = DateTime.UtcNow });

        // --- 6. Makarna (Grains) ---
        var makarna = new Product { ProductName = "Makarna", Brand = "Ankara", Unit = "500g", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(makarna);
        prices.Add(new MarketProductPrice { Product = makarna, MarketId = migros.Id, DistrictId = sisli.Id, Price = 18.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = makarna, MarketId = bim.Id, DistrictId = sisli.Id, Price = 15.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = makarna, MarketId = a101.Id, DistrictId = sisli.Id, Price = 16.00m, LastUpdated = DateTime.UtcNow });

        // --- 7. Un (Grains) ---
        var un = new Product { ProductName = "Un", Brand = "Söke", Unit = "1kg", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(un);
        prices.Add(new MarketProductPrice { Product = un, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 35.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = un, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 29.00m, LastUpdated = DateTime.UtcNow });

        // --- 8. Bulgur (Grains) ---
        var bulgur = new Product { ProductName = "Bulgur", Brand = "Duru", Unit = "1kg", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(bulgur);
        prices.Add(new MarketProductPrice { Product = bulgur, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 42.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = bulgur, MarketId = carrefour.Id, DistrictId = besiktas.Id, Price = 45.00m, LastUpdated = DateTime.UtcNow });

        // --- 9. Kırmızı Mercimek (Grains) ---
        var mercimek = new Product { ProductName = "Kırmızı Mercimek", Brand = "Reis", Unit = "1kg", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(mercimek);
        prices.Add(new MarketProductPrice { Product = mercimek, MarketId = migros.Id, DistrictId = sisli.Id, Price = 65.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = mercimek, MarketId = bim.Id, DistrictId = sisli.Id, Price = 55.00m, LastUpdated = DateTime.UtcNow });

        // --- 10. İrmik (Grains) ---
        var irmik = new Product { ProductName = "İrmik", Brand = "Piyale", Unit = "500g", CategoryId = grains.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(irmik);
        prices.Add(new MarketProductPrice { Product = irmik, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 20.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = irmik, MarketId = a101.Id, DistrictId = kadikoy.Id, Price = 18.00m, LastUpdated = DateTime.UtcNow });

        // --- 11. Ayçiçek Yağı (Oils) ---
        var aycicek = new Product { ProductName = "Ayçiçek Yağı", Brand = "Yudum", Unit = "1L", CategoryId = oils.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(aycicek);
        prices.Add(new MarketProductPrice { Product = aycicek, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 60.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = aycicek, MarketId = bim.Id, DistrictId = besiktas.Id, Price = 52.00m, LastUpdated = DateTime.UtcNow });

        // --- 12. Mısır Özü Yağı (Oils) ---
        var misir = new Product { ProductName = "Mısır Özü Yağı", Brand = "Kırlangıç", Unit = "2L", CategoryId = oils.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(misir);
        prices.Add(new MarketProductPrice { Product = misir, MarketId = carrefour.Id, DistrictId = sisli.Id, Price = 150.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = misir, MarketId = migros.Id, DistrictId = sisli.Id, Price = 145.00m, LastUpdated = DateTime.UtcNow });

        // --- 13. Patates (Vegetables) ---
        var patates = new Product { ProductName = "Patates", Brand = "Yerli", Unit = "1kg", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(patates);
        prices.Add(new MarketProductPrice { Product = patates, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 18.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = patates, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 14.00m, LastUpdated = DateTime.UtcNow });

        // --- 14. Soğan (Vegetables) ---
        var sogan = new Product { ProductName = "Soğan", Brand = "Yerli", Unit = "1kg", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(sogan);
        prices.Add(new MarketProductPrice { Product = sogan, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 15.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = sogan, MarketId = a101.Id, DistrictId = besiktas.Id, Price = 12.00m, LastUpdated = DateTime.UtcNow });

        // --- 15. Çarliston Biber (Vegetables) ---
        var biber = new Product { ProductName = "Çarliston Biber", Brand = "Yerli", Unit = "500g", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(biber);
        prices.Add(new MarketProductPrice { Product = biber, MarketId = migros.Id, DistrictId = sisli.Id, Price = 25.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = biber, MarketId = carrefour.Id, DistrictId = sisli.Id, Price = 28.00m, LastUpdated = DateTime.UtcNow });

        // --- 16. Havuç (Vegetables) ---
        var havuc = new Product { ProductName = "Havuç", Brand = "Beypazarı", Unit = "1kg", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(havuc);
        prices.Add(new MarketProductPrice { Product = havuc, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 20.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = havuc, MarketId = bim.Id, DistrictId = kadikoy.Id, Price = 16.00m, LastUpdated = DateTime.UtcNow });

        // --- 17. Ispanak (Vegetables) ---
        var ispanak = new Product { ProductName = "Ispanak", Brand = "Yerli", Unit = "1kg", CategoryId = vegetables.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(ispanak);
        prices.Add(new MarketProductPrice { Product = ispanak, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 35.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = ispanak, MarketId = carrefour.Id, DistrictId = besiktas.Id, Price = 38.00m, LastUpdated = DateTime.UtcNow });

        // --- 18. Portakal (Fruits) ---
        var portakal = new Product { ProductName = "Portakal", Brand = "Finike", Unit = "1kg", CategoryId = fruits.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(portakal);
        prices.Add(new MarketProductPrice { Product = portakal, MarketId = migros.Id, DistrictId = sisli.Id, Price = 30.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = portakal, MarketId = bim.Id, DistrictId = sisli.Id, Price = 25.00m, LastUpdated = DateTime.UtcNow });

        // --- 19. Mandalina (Fruits) ---
        var mandalina = new Product { ProductName = "Mandalina", Brand = "Satsuma", Unit = "1kg", CategoryId = fruits.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(mandalina);
        prices.Add(new MarketProductPrice { Product = mandalina, MarketId = migros.Id, DistrictId = kadikoy.Id, Price = 28.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = mandalina, MarketId = a101.Id, DistrictId = kadikoy.Id, Price = 24.00m, LastUpdated = DateTime.UtcNow });

        // --- 20. Armut (Fruits) ---
        var armut = new Product { ProductName = "Armut", Brand = "Deveci", Unit = "1kg", CategoryId = fruits.Id, CreatedAt = DateTime.UtcNow, LastUpdated = DateTime.UtcNow };
        products.Add(armut);
        prices.Add(new MarketProductPrice { Product = armut, MarketId = migros.Id, DistrictId = besiktas.Id, Price = 45.00m, LastUpdated = DateTime.UtcNow });
        prices.Add(new MarketProductPrice { Product = armut, MarketId = carrefour.Id, DistrictId = besiktas.Id, Price = 50.00m, LastUpdated = DateTime.UtcNow });


        
        
        // Save All
        await context.Products.AddRangeAsync(products);
        await context.MarketProductPrices.AddRangeAsync(prices);
        await context.SaveChangesAsync();
        
        Console.WriteLine("--- SEEDING FINISHED ---");
    }
}
