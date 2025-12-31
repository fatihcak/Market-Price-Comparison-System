using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433;Database=compsys;User Id=admin;Password=admin-0-0;TrustServerCertificate=True;";
    static string CatalogPath = @"c:\Users\DOU\Documents\GitHub\Market-Price-Comparison-System\Frontend\src\constants\product_category_update.md";

    static void Main(string[] args)
    {
        // Show database stats first
        Console.WriteLine("=== DATABASE STATISTICS ===");
        ShowDatabaseStats();
        
        if (args.Length > 0 && args[0] == "--sync")
        {
            Console.WriteLine("\n=== STARTING DATABASE SYNC FROM CATALOG ===");
            SyncFromCatalog();
        }
        else
        {
            Console.WriteLine("\nTo sync from catalog, run: dotnet run -- --sync");
        }
    }

    static void ShowDatabaseStats()
    {
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Product count
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Product", connection))
            {
                var count = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Total Products: {count}");
            }

            // Category count
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM ProductCategory", connection))
            {
                var count = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Total Categories: {count}");
            }

            // Market count
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM Market", connection))
            {
                var count = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Total Markets: {count}");
            }

            // Price count
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM MarketProductPrice", connection))
            {
                var count = (int)cmd.ExecuteScalar();
                Console.WriteLine($"Total Prices: {count}");
            }

            // Sample products
            Console.WriteLine("\n=== SAMPLE PRODUCTS (First 10) ===");
            using (var cmd = new SqlCommand("SELECT TOP 10 ProductID, ProductName, Brand, Unit FROM Product ORDER BY ProductID", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    Console.WriteLine($"  [{reader.GetInt32(0)}] {reader.GetString(1)} - {(reader.IsDBNull(2) ? "N/A" : reader.GetString(2))} ({(reader.IsDBNull(3) ? "N/A" : reader.GetString(3))})");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error connecting to database: " + ex.Message);
        }
    }

    static void SyncFromCatalog()
    {
        if (!File.Exists(CatalogPath))
        {
            Console.WriteLine($"Error: File not found at {CatalogPath}");
            return;
        }

        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // 1. Load Categories Map (Name -> ID)
            var categoryMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = new SqlCommand("SELECT CategoryId, CategoryName FROM ProductCategory", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    categoryMap[reader.GetString(1)] = reader.GetInt32(0);
                }
            }
            Console.WriteLine($"Loaded {categoryMap.Count} categories from DB.");

            // 2. Parse Markdown Table
            var lines = File.ReadAllLines(CatalogPath);
            int updatedCount = 0;
            int notFoundCount = 0;
            int categoryNotFoundCount = 0;

            Console.WriteLine($"Processing {lines.Length} lines from catalog...");

            foreach (var line in lines)
            {
                string text = line.Trim();

                // Skip headers and separators
                if (string.IsNullOrWhiteSpace(text) || text.StartsWith("Main Category") || text.StartsWith("---"))
                    continue;

                var parts = text.Split('|').Select(p => p.Trim()).ToArray();
                if (parts.Length < 3) continue;

                // Format: Main Category | Sub Category | Product Name
                string mainCategory = parts[0];
                string subCategory = parts[1];
                string productName = parts[2];

                // Determine Target Category ID
                // We assume 'Sub Category' maps to DB 'ProductCategory.CategoryName'
                // If Sub Category is empty or same as Main, we might try Main.
                string targetCategoryName = subCategory;
                
                if (!categoryMap.ContainsKey(targetCategoryName))
                {
                    // Fallback try: maybe SubCategory doesn't exist, try MainCategory?
                    // Or log error.
                    // For now, let's log and skip to be safe.
                    // Console.WriteLine($"Category not found: '{targetCategoryName}' for product '{productName}'");
                    categoryNotFoundCount++;
                    continue;
                }

                int targetCatId = categoryMap[targetCategoryName];

                // Update Product
                // We use exact match on ProductName
                var updateSql = @"UPDATE Product 
                                  SET CategoryId = @catId, LastUpdated = GETDATE()
                                  WHERE ProductName = @name AND CategoryId != @catId";
                
                using var upCmd = new SqlCommand(updateSql, connection);
                upCmd.Parameters.AddWithValue("@catId", targetCatId);
                upCmd.Parameters.AddWithValue("@name", productName);
                
                int rows = upCmd.ExecuteNonQuery();
                if (rows > 0) 
                {
                    updatedCount += rows;
                    if (updatedCount % 100 == 0) Console.Write(".");
                }
                else
                {
                    // Check if it's because product not found or already in category
                    // Optional: Fuzzy check could go here if we wanted
                    notFoundCount++;
                }
            }

            Console.WriteLine("\n=== SYNC COMPLETE ===");
            Console.WriteLine($"Updated: {updatedCount} products");
            Console.WriteLine($"Skipped/Not Found/Already Covered: {notFoundCount}");
            Console.WriteLine($"Category Not Found Errors: {categoryNotFoundCount}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}

