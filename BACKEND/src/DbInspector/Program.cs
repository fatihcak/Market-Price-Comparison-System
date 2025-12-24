using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433;Database=compsys;User Id=admin;Password=admin-0-0;TrustServerCertificate=True;";

    static void Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         PRODUCT CONSOLIDATOR - Data Migration Tool        ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            Console.WriteLine("[✓] Connected to database\n");

            // Step 1: Fetch all products with their market info
            var products = FetchAllProducts(connection);
            Console.WriteLine($"[i] Total products in database: {products.Count}\n");

            // Step 2: Group products by normalized key (brand + name + unit)
            var groups = GroupProductsByNormalizedKey(products);
            
            // Step 3: Find duplicates (groups with more than one product)
            var duplicates = groups.Where(g => g.Value.Count > 1).ToList();
            Console.WriteLine($"[!] Found {duplicates.Count} product groups with duplicates\n");

            if (duplicates.Count == 0)
            {
                Console.WriteLine("[✓] No duplicates found. Database is clean!");
                return;
            }

            // Step 4: Show preview of what will be merged
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("PREVIEW: Products that will be merged");
            Console.WriteLine("═══════════════════════════════════════════════════════════\n");

            int shownCount = 0;
            foreach (var group in duplicates.Take(20)) // Show first 20
            {
                Console.WriteLine($"Group Key: {group.Key}");
                Console.WriteLine($"  → Will merge {group.Value.Count} products:");
                foreach (var p in group.Value)
                {
                    Console.WriteLine($"    - ID:{p.ProductId} | Brand: '{p.Brand}' | Name: '{p.ProductName}' | Unit: '{p.Unit}' | Market: {p.MarketName}");
                }
                Console.WriteLine();
                shownCount++;
            }

            if (duplicates.Count > 20)
            {
                Console.WriteLine($"... and {duplicates.Count - 20} more groups\n");
            }

            // Step 5: Ask for confirmation
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.Write("Do you want to proceed with the migration? (yes/no): ");
            var response = Console.ReadLine()?.ToLower();
            
            if (response != "yes")
            {
                Console.WriteLine("\n[!] Migration cancelled.");
                return;
            }

            // Step 6: Perform the merge
            Console.WriteLine("\n[...] Starting migration...\n");
            int mergedCount = 0;
            int updatedPricesCount = 0;

            foreach (var group in duplicates)
            {
                var result = MergeProductGroup(connection, group.Value);
                mergedCount += result.DeletedProducts;
                updatedPricesCount += result.UpdatedPrices;
            }

            Console.WriteLine("\n═══════════════════════════════════════════════════════════");
            Console.WriteLine($"[✓] Migration complete!");
            Console.WriteLine($"    - Products merged: {mergedCount}");
            Console.WriteLine($"    - Price records updated: {updatedPricesCount}");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n[ERROR] {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }

    static List<ProductInfo> FetchAllProducts(SqlConnection connection)
    {
        var products = new List<ProductInfo>();

        // Fetch products with their market info from MarketProductPrice
        var query = @"
            SELECT DISTINCT
                p.ProductID,
                p.ProductName,
                p.Brand,
                p.Unit,
                p.CategoryID,
                p.ImageURL,
                m.MarketName
            FROM Product p
            LEFT JOIN MarketProductPrice mpp ON p.ProductID = mpp.ProductID
            LEFT JOIN Market m ON mpp.MarketID = m.MarketID
            ORDER BY p.ProductName";

        using var cmd = new SqlCommand(query, connection);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            products.Add(new ProductInfo
            {
                ProductId = reader.GetInt32(0),
                ProductName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                Brand = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Unit = reader.IsDBNull(3) ? "" : reader.GetString(3),
                CategoryId = reader.GetInt32(4),
                ImageUrl = reader.IsDBNull(5) ? "" : reader.GetString(5),
                MarketName = reader.IsDBNull(6) ? "No Market" : reader.GetString(6)
            });
        }

        return products;
    }

    static Dictionary<string, List<ProductInfo>> GroupProductsByNormalizedKey(List<ProductInfo> products)
    {
        var groups = new Dictionary<string, List<ProductInfo>>();

        foreach (var product in products)
        {
            var key = GetNormalizedKey(product.Brand, product.ProductName, product.Unit);
            
            if (!groups.ContainsKey(key))
            {
                groups[key] = new List<ProductInfo>();
            }
            
            // Avoid adding same product ID twice (can happen due to multiple market prices)
            if (!groups[key].Any(p => p.ProductId == product.ProductId))
            {
                groups[key].Add(product);
            }
        }

        return groups;
    }

    static string GetNormalizedKey(string brand, string productName, string unit)
    {
        // IMPORTANT: We ignore brand because it's often already included in productName
        // and different markets write brands differently (e.g., "CAPRI" vs "CapriSun")
        
        // Normalize product name only
        var normName = NormalizeText(productName);
        
        // Normalize unit: extract number and unit type
        var normUnit = NormalizeUnit(unit);

        // Key is just productName + unit (no brand)
        return $"{normName}_{normUnit}";
    }

    static string NormalizeText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return "";

        // Convert to lowercase
        var result = text.ToLowerInvariant();

        // Replace Turkish characters
        result = result
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ı", "i")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("İ", "i");

        // Remove all non-alphanumeric characters
        result = Regex.Replace(result, @"[^a-z0-9]", "");

        return result;
    }

    static string NormalizeUnit(string unit)
    {
        if (string.IsNullOrWhiteSpace(unit)) return "";

        var lower = unit.ToLowerInvariant();

        // Extract number and unit type
        var match = Regex.Match(lower, @"(\d+(?:[.,]\d+)?)\s*(kg|gr|g|ml|lt|l|cl|adet|tane|paket)");
        
        if (match.Success)
        {
            var value = decimal.Parse(match.Groups[1].Value.Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture);
            var unitType = match.Groups[2].Value;

            // Normalize to base units
            return unitType switch
            {
                "kg" => $"{value * 1000}g",
                "gr" or "g" => $"{value}g",
                "lt" or "l" => $"{value * 1000}ml",
                "ml" => $"{value}ml",
                "cl" => $"{value * 10}ml",
                _ => $"{value}{unitType}"
            };
        }

        // If no pattern matched, just normalize the text
        return NormalizeText(unit);
    }

    static (int DeletedProducts, int UpdatedPrices) MergeProductGroup(SqlConnection connection, List<ProductInfo> products)
    {
        // Sort to get the "canonical" product (prefer one with image, then by ID)
        var sorted = products
            .OrderByDescending(p => !string.IsNullOrEmpty(p.ImageUrl))
            .ThenBy(p => p.ProductId)
            .ToList();

        var canonical = sorted[0];
        var duplicates = sorted.Skip(1).ToList();

        if (duplicates.Count == 0) return (0, 0);

        Console.WriteLine($"  Keeping Product ID {canonical.ProductId} as canonical");

        int updatedPrices = 0;
        int deletedProducts = 0;

        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var dup in duplicates)
            {
                // Update MarketProductPrice to point to canonical product
                var updatePricesQuery = @"
                    UPDATE MarketProductPrice 
                    SET ProductID = @CanonicalId 
                    WHERE ProductID = @DuplicateId";

                using (var cmd = new SqlCommand(updatePricesQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@CanonicalId", canonical.ProductId);
                    cmd.Parameters.AddWithValue("@DuplicateId", dup.ProductId);
                    updatedPrices += cmd.ExecuteNonQuery();
                }

                // Update UserProductList to point to canonical product
                var updateListQuery = @"
                    UPDATE UserProductList 
                    SET ProductID = @CanonicalId 
                    WHERE ProductID = @DuplicateId";

                using (var cmd = new SqlCommand(updateListQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@CanonicalId", canonical.ProductId);
                    cmd.Parameters.AddWithValue("@DuplicateId", dup.ProductId);
                    cmd.ExecuteNonQuery();
                }

                // Delete duplicate product
                var deleteQuery = "DELETE FROM Product WHERE ProductID = @ProductId";
                using (var cmd = new SqlCommand(deleteQuery, connection, transaction))
                {
                    cmd.Parameters.AddWithValue("@ProductId", dup.ProductId);
                    cmd.ExecuteNonQuery();
                    deletedProducts++;
                }

                Console.WriteLine($"    → Merged Product ID {dup.ProductId} into {canonical.ProductId}");
            }

            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    [ERROR] Rolling back: {ex.Message}");
            transaction.Rollback();
            throw;
        }

        return (deletedProducts, updatedPrices);
    }
}

class ProductInfo
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public string Brand { get; set; } = "";
    public string Unit { get; set; } = "";
    public int CategoryId { get; set; }
    public string ImageUrl { get; set; } = "";
    public string MarketName { get; set; } = "";
}
