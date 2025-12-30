using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433;Database=compsys;User Id=admin;Password=admin-0-0;TrustServerCertificate=True;";
    static string CatalogPath = @"c:\Users\DOU\Documents\GitHub\Market-Price-Comparison-System\Frontend\src\constants\corrected_full_catalog.md";

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
            Console.WriteLine($"Loaded {categoryMap.Count} categories.");

            // 2. Parse Markdown
            var lines = File.ReadAllLines(CatalogPath);
            string currentSubCategory = null;
            int updatedCount = 0;
            int notFoundCount = 0;

            foreach (var line in lines)
            {
                string text = line.Trim();

                // Detect SubCategory Header (### Name)
                if (text.StartsWith("### "))
                {
                    string catName = text.Substring(4).Trim();
                    if (categoryMap.ContainsKey(catName))
                    {
                        currentSubCategory = catName;
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: Category '{catName}' not found in DB! Skipping products under it.");
                        currentSubCategory = null;
                    }
                    continue;
                }

                // Detect Product Item (- Name)
                if (text.StartsWith("- ") && currentSubCategory != null)
                {
                    string productName = text.Substring(2).Trim();
                    int targetCatId = categoryMap[currentSubCategory];

                    // Update Product
                    var updateSql = "UPDATE Product SET CategoryId = @catId WHERE ProductName = @name AND CategoryId != @catId";
                    using var upCmd = new SqlCommand(updateSql, connection);
                    upCmd.Parameters.AddWithValue("@catId", targetCatId);
                    upCmd.Parameters.AddWithValue("@name", productName);
                    
                    int rows = upCmd.ExecuteNonQuery();
                    if (rows > 0) updatedCount += rows;
                }
            }

            Console.WriteLine($"SYNC COMPLETE.");
            Console.WriteLine($"Updated {updatedCount} product records to match the catalog.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}

