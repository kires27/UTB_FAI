using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UTB.eshop25.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mysql_101_seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "Id", "Description", "ImageSrc", "Name", "Price" },
                values: new object[,]
                {
                    { 1, "Nejlepší chleba na kontinentu", "/img/products/product-1.png", "Chleba", 50.0 },
                    { 2, "Nejlepší rohlík v galaxii", "/img/products/product-2.png", "Rohlík", 4.0 },
                    { 3, "Nejlepší bageta ve vesmíru", "/img/products/product-3.png", "Bageta", 25.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Product",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
