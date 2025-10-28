using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CATEGORYID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CATEGORYNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ICON = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CREATEDAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CATEGORYID);
                });

            migrationBuilder.CreateTable(
                name: "Markets",
                columns: table => new
                {
                    MARKETID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MARKETNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    LOGOURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    WEBSITE = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CREATEDAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Markets", x => x.MARKETID);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    USERID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USERNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SURNAME = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EMAIL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PASSWORDHASH = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    USERROLE = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ISDELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.USERID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    PRODUCTID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCTNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CATEGORYID = table.Column<int>(type: "int", nullable: false),
                    IMAGEURL = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UNIT = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CREATEDAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.PRODUCTID);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CATEGORYID",
                        column: x => x.CATEGORYID,
                        principalTable: "Categories",
                        principalColumn: "CATEGORYID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingLists",
                columns: table => new
                {
                    SHOPPINGLISTID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USERID = table.Column<int>(type: "int", nullable: false),
                    LISTNAME = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CREATEDAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingLists", x => x.SHOPPINGLISTID);
                    table.ForeignKey(
                        name: "FK_ShoppingLists_Users_USERID",
                        column: x => x.USERID,
                        principalTable: "Users",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersAdresses",
                columns: table => new
                {
                    ADRESSID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    USERID = table.Column<int>(type: "int", nullable: false),
                    ADRESS = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CITY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DISTRICT = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    POSTALCODE = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    COUNTRY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ISDELETED = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersAdresses", x => x.ADRESSID);
                    table.ForeignKey(
                        name: "FK_UsersAdresses_Users_USERID",
                        column: x => x.USERID,
                        principalTable: "Users",
                        principalColumn: "USERID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PRICEID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PRODUCTID = table.Column<int>(type: "int", nullable: false),
                    MARKETID = table.Column<int>(type: "int", nullable: false),
                    PRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DISCOUNTPRICE = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    UPDATEDAT = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PRICEID);
                    table.ForeignKey(
                        name: "FK_Prices_Markets_MARKETID",
                        column: x => x.MARKETID,
                        principalTable: "Markets",
                        principalColumn: "MARKETID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Prices_Products_PRODUCTID",
                        column: x => x.PRODUCTID,
                        principalTable: "Products",
                        principalColumn: "PRODUCTID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShoppingListItems",
                columns: table => new
                {
                    LISTITEMID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SHOPPINGLISTID = table.Column<int>(type: "int", nullable: false),
                    PRODUCTID = table.Column<int>(type: "int", nullable: false),
                    QUANTITY = table.Column<int>(type: "int", nullable: false),
                    CREATEDAT = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingListItems", x => x.LISTITEMID);
                    table.ForeignKey(
                        name: "FK_ShoppingListItems_Products_PRODUCTID",
                        column: x => x.PRODUCTID,
                        principalTable: "Products",
                        principalColumn: "PRODUCTID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ShoppingListItems_ShoppingLists_SHOPPINGLISTID",
                        column: x => x.SHOPPINGLISTID,
                        principalTable: "ShoppingLists",
                        principalColumn: "SHOPPINGLISTID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Prices_MARKETID",
                table: "Prices",
                column: "MARKETID");

            migrationBuilder.CreateIndex(
                name: "IX_Prices_PRODUCTID",
                table: "Prices",
                column: "PRODUCTID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CATEGORYID",
                table: "Products",
                column: "CATEGORYID");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_PRODUCTID",
                table: "ShoppingListItems",
                column: "PRODUCTID");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingListItems_SHOPPINGLISTID",
                table: "ShoppingListItems",
                column: "SHOPPINGLISTID");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingLists_USERID",
                table: "ShoppingLists",
                column: "USERID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_EMAIL",
                table: "Users",
                column: "EMAIL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsersAdresses_USERID",
                table: "UsersAdresses",
                column: "USERID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "ShoppingListItems");

            migrationBuilder.DropTable(
                name: "UsersAdresses");

            migrationBuilder.DropTable(
                name: "Markets");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ShoppingLists");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
