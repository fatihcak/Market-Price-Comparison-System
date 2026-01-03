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
        if (args.Length > 0 && args[0] == "--check-indexes")
        {
            VerifyIndexes();
        }
        else if (args.Length > 0 && args[0] == "--fix-migration-history")
        {
             FixMigrationHistory();
        }
        else
        {
            // Show database stats first
            Console.WriteLine("=== DATABASE STATISTICS ===");
            ShowDatabaseStats();
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --check-indexes        : Verify DB indexes");
            Console.WriteLine("  --fix-migration-history : Insert InitialCreate into history if missing");
        }
    }

    static void FixMigrationHistory()
    {
        Console.WriteLine("=== FIXING MIGRATION HISTORY ===");
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Check if __EFMigrationsHistory exists
            var checkTable = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '__EFMigrationsHistory'";
            using (var cmd = new SqlCommand(checkTable, connection))
            {
                int exists = (int)cmd.ExecuteScalar();
                if (exists == 0)
                {
                    Console.WriteLine("Creating __EFMigrationsHistory table...");
                    var createTable = @"
                        CREATE TABLE [dbo].[__EFMigrationsHistory](
                            [MigrationId] [nvarchar](150) NOT NULL,
                            [ProductVersion] [nvarchar](32) NOT NULL,
                         CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
                        (
                            [MigrationId] ASC
                        ))";
                    using var createCmd = new SqlCommand(createTable, connection);
                    createCmd.ExecuteNonQuery();
                }
            }

            // Check if InitialCreate is already recorded
            // Note: We need the EXACT name of the migration class we generated.
            // Based on file list: 20260102212833_InitialCreate.cs
            string migrationName = "20260102212833_InitialCreate"; 

            var checkMig = "SELECT COUNT(*) FROM __EFMigrationsHistory WHERE MigrationId = @mig";
            using (var cmd = new SqlCommand(checkMig, connection))
            {
                cmd.Parameters.AddWithValue("@mig", migrationName);
                int count = (int)cmd.ExecuteScalar();
                
                if (count == 0)
                {
                    Console.WriteLine($"Migration '{migrationName}' missing from history. Inserting...");
                    var insert = "INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES (@mig, '8.0.0')";
                    using (var insertCmd = new SqlCommand(insert, connection))
                    {
                        insertCmd.Parameters.AddWithValue("@mig", migrationName);
                        insertCmd.ExecuteNonQuery();
                        Console.WriteLine("Inserted successfully.");
                    }
                }
                else
                {
                    Console.WriteLine($"Migration '{migrationName}' is already recorded.");
                }
            }
        }
        catch (Exception ex)
        {
             Console.WriteLine("Error: " + ex.Message);
        }
    }

    static void VerifyIndexes()
    {
        Console.WriteLine("=== VERIFYING INDEXES ===");
        try
        {
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Query provided by user prompt
            var sql = @"
                SELECT 
                    i.name AS IndexName,
                    COL_NAME(ic.object_id, ic.column_id) AS ColumnName,
                    i.type_desc AS IndexType
                FROM sys.indexes i
                INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
                WHERE i.object_id = OBJECT_ID('Product')
                ORDER BY i.name, ic.key_ordinal;";

            Console.WriteLine("Executing query for table 'Product' (Singular)...");
            bool foundSingular = false;
            using (var cmd = new SqlCommand(sql, connection))
            using (var reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    foundSingular = true;
                    while (reader.Read())
                    {
                        Console.WriteLine($"{reader["IndexName"]} | {reader["ColumnName"]} | {reader["IndexType"]}");
                    }
                }
            }

            if (!foundSingular)
            {
                Console.WriteLine("No indexes found for 'Product'. Checking 'Products' (Plural)...");
                
                var sqlPlural = sql.Replace("'Product'", "'Products'");
                using (var cmd = new SqlCommand(sqlPlural, connection))
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"{reader["IndexName"]} | {reader["ColumnName"]} | {reader["IndexType"]}");
                        }
                    }
                    else
                    {
                         Console.WriteLine("No indexes found for 'Products' either.");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
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
                // Handle potential missing table exception gracefully just in case
                try {
                     var count = (int)cmd.ExecuteScalar();
                     Console.WriteLine($"Total Products: {count}");
                } catch { Console.WriteLine("Table 'Product' not found (or error)."); }
            }

            // Products (plural) count check?
            using (var cmd = new SqlCommand("SELECT OBJECT_ID('Products')", connection))
            {
                 var objId = cmd.ExecuteScalar();
                 if (objId != DBNull.Value && objId != null) 
                 {
                     Console.WriteLine("(Warning: 'Products' table also exists!)");
                 }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error connecting to database: " + ex.Message);
        }
    }
}
