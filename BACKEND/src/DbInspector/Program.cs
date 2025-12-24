using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433;Database=compsys;User Id=admin;Password=admin-0-0;TrustServerCertificate=True;";
    static Random random = new Random();

    static void Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║     ADD NEW MARKETS - Database Migration Tool (FAST)      ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("[✓] Connected to database\n");

            // Check current state
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("DATABASE STATUS CHECK");
            Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            // Check markets
            var marketCheck = @"SELECT MarketID, MarketName, 
                (SELECT COUNT(*) FROM MarketProductPrice WHERE MarketID = m.MarketID) as PriceCount
                FROM Market m ORDER BY MarketID";
            
            using (var cmd = new SqlCommand(marketCheck, connection))
            using (var reader = cmd.ExecuteReader())
            {
                Console.WriteLine("Markets:");
                while (reader.Read())
                {
                    Console.WriteLine($"  ID:{reader.GetInt32(0)} | {reader.GetString(1)} | {reader.GetInt32(2)} prices");
                }
            }
            Console.WriteLine();

            // Check if Mymarket or Alagök has any prices
            var newMarketPrices = "SELECT COUNT(*) FROM MarketProductPrice WHERE MarketID IN (SELECT MarketID FROM Market WHERE MarketName IN ('Mymarket', N'Alagök Market'))";
            using (var cmd = new SqlCommand(newMarketPrices, connection))
            {
                var count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    Console.WriteLine($"[!] New markets already have {count} price entries.");
                    Console.Write("Do you want to DELETE these and start fresh? (yes/no): ");
                    if (Console.ReadLine()?.ToLower() == "yes")
                    {
                        var deleteQuery = "DELETE FROM MarketProductPrice WHERE MarketID IN (SELECT MarketID FROM Market WHERE MarketName IN ('Mymarket', N'Alagök Market'))";
                        using var delCmd = new SqlCommand(deleteQuery, connection);
                        var deleted = delCmd.ExecuteNonQuery();
                        Console.WriteLine($"[✓] Deleted {deleted} price entries\n");
                    }
                    else
                    {
                        Console.WriteLine("[i] Keeping existing entries, will only add missing ones.\n");
                    }
                }
                else
                {
                    Console.WriteLine("[✓] New markets have no price entries yet.\n");
                }
            }

            // Get or create markets
            var mymarketId = GetOrCreateMarket(connection, "Mymarket");
            var alagokId = GetOrCreateMarket(connection, "Alagök Market");
            
            Console.WriteLine($"\n[i] Mymarket ID: {mymarketId}");
            Console.WriteLine($"[i] Alagök Market ID: {alagokId}\n");

            // Get ONE district (to keep it simple and fast)
            var districtId = GetFirstDistrictId(connection);
            Console.WriteLine($"[i] Using District ID: {districtId}\n");

            // Get existing products with prices
            var productsWithPrices = GetProductsWithPrices(connection);
            Console.WriteLine($"[i] Found {productsWithPrices.Count} products with prices\n");

            // Select 50% of products randomly
            var selectedProducts = productsWithPrices
                .OrderBy(_ => random.Next())
                .Take(productsWithPrices.Count / 2)
                .ToList();
            
            Console.WriteLine($"[i] Selected {selectedProducts.Count} random products for new market prices\n");

            // Confirmation
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine($"Will add ~{selectedProducts.Count * 2} price entries using BATCH INSERT (fast)");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.Write("Proceed? (yes/no): ");

            if (Console.ReadLine()?.ToLower() != "yes")
            {
                Console.WriteLine("\n[!] Migration cancelled.");
                return;
            }

            // BATCH INSERT - Much faster!
            Console.WriteLine("\n[...] Adding prices with batch insert...\n");
            
            var batchSize = 500;
            var totalAdded = 0;
            var insertValues = new List<string>();

            foreach (var product in selectedProducts)
            {
                var mymarketPrice = Math.Round(product.BasePrice * (decimal)(0.8 + random.NextDouble() * 0.4), 2);
                var alagokPrice = Math.Round(product.BasePrice * (decimal)(0.8 + random.NextDouble() * 0.4), 2);
                var now = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

                insertValues.Add($"({mymarketId}, {product.ProductId}, {districtId}, {mymarketPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, '{now}')");
                insertValues.Add($"({alagokId}, {product.ProductId}, {districtId}, {alagokPrice.ToString(System.Globalization.CultureInfo.InvariantCulture)}, '{now}')");

                // Execute batch when we reach batch size
                if (insertValues.Count >= batchSize)
                {
                    ExecuteBatchInsert(connection, insertValues);
                    totalAdded += insertValues.Count;
                    Console.WriteLine($"  Added {totalAdded} entries...");
                    insertValues.Clear();
                }
            }

            // Insert remaining
            if (insertValues.Count > 0)
            {
                ExecuteBatchInsert(connection, insertValues);
                totalAdded += insertValues.Count;
            }

            Console.WriteLine($"\n[✓] Migration complete! Added {totalAdded} price entries.");

            // Final check
            Console.WriteLine("\n═══════════════════════════════════════════════════════════");
            Console.WriteLine("FINAL STATUS:");
            using (var cmd = new SqlCommand(marketCheck, connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"  {reader.GetString(1)}: {reader.GetInt32(2)} prices");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    static void ExecuteBatchInsert(SqlConnection connection, List<string> values)
    {
        var sql = $@"INSERT INTO MarketProductPrice (MarketID, ProductID, DistrictID, Price, LastUpdated) VALUES {string.Join(",", values)}";
        using var cmd = new SqlCommand(sql, connection);
        cmd.CommandTimeout = 120;
        cmd.ExecuteNonQuery();
    }

    static int GetOrCreateMarket(SqlConnection connection, string marketName)
    {
        var checkQuery = "SELECT MarketID FROM Market WHERE MarketName = @Name";
        using (var cmd = new SqlCommand(checkQuery, connection))
        {
            cmd.Parameters.AddWithValue("@Name", marketName);
            var result = cmd.ExecuteScalar();
            if (result != null) return (int)result;
        }

        var insertQuery = "INSERT INTO Market (MarketName) OUTPUT INSERTED.MarketID VALUES (@Name)";
        using (var cmd = new SqlCommand(insertQuery, connection))
        {
            cmd.Parameters.AddWithValue("@Name", marketName);
            var newId = (int)cmd.ExecuteScalar();
            Console.WriteLine($"[+] Created new market: {marketName} (ID: {newId})");
            return newId;
        }
    }

    static int GetFirstDistrictId(SqlConnection connection)
    {
        using var cmd = new SqlCommand("SELECT TOP 1 DistrictID FROM District", connection);
        return (int)cmd.ExecuteScalar();
    }

    static List<ProductWithPrice> GetProductsWithPrices(SqlConnection connection)
    {
        var products = new List<ProductWithPrice>();
        var query = @"
            SELECT p.ProductID, p.ProductName, MIN(mpp.Price) as BasePrice
            FROM Product p
            INNER JOIN MarketProductPrice mpp ON p.ProductID = mpp.ProductID
            WHERE mpp.Price > 0
            GROUP BY p.ProductID, p.ProductName";

        using var cmd = new SqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(new ProductWithPrice
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.GetString(1),
                BasePrice = reader.GetDecimal(2)
            });
        }
        return products;
    }
}

class ProductWithPrice
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal BasePrice { get; set; }
}
