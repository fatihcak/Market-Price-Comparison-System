using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

class Program
{
    static string connectionString = "Server=database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433;Database=compsys;User Id=admin;Password=admin-0-0;TrustServerCertificate=True;";
    static string CatalogPath = @"c:\Users\Doguk\Market-Price-Comparison-System\Frontend\src\constants\corrected_full_catalog.md";

    static void Main(string[] args)
    {
        Console.WriteLine("STARTING DATABASE SYNC FROM CATALOG...");

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
                        // Console.WriteLine($"Switched to Category: {currentSubCategory}");
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
                    // We match by ProductName. Note: This updates ALL products with this exact name.
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
