using System;
using Microsoft.Data.SqlClient;

class Program
{
    static void Main(string[] args)
    {
        string connectionString = "Server=database-compsys.cl0806yimqf2.eu-central-1.rds.amazonaws.com,1433;Database=compsys;User Id=admin;Password=admin-0-0;TrustServerCertificate=True;";
        
        Console.WriteLine("[INSPECTOR] Connecting...");
        try
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("[INSPECTOR] Connected!\n");

                // Get columns for each table
                string[] tables = { "Product", "ProductCategory", "Market", "City", "District", "MarketProductPrice" };
                
                foreach (var table in tables)
                {
                    Console.WriteLine($"=== {table} COLUMNS ===");
                    var query = $@"
                        SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = '{table}'
                        ORDER BY ORDINAL_POSITION";
                    
                    using (var cmd = new SqlCommand(query, connection))
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine($"  {reader["COLUMN_NAME"]} ({reader["DATA_TYPE"]}, nullable: {reader["IS_NULLABLE"]})");
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] {ex.Message}");
        }
    }
}
