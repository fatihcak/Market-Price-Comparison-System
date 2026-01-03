using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProductPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Indexes for performance (Explicitly added because they are missing in Prod)
            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                table: "Product",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ProductName",
                table: "Product",
                column: "ProductName");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Brand",
                table: "Product",
                column: "Brand");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Product_CategoryId",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_ProductName",
                table: "Product");

            migrationBuilder.DropIndex(
                name: "IX_Product_Brand",
                table: "Product");
        }
    }
}
