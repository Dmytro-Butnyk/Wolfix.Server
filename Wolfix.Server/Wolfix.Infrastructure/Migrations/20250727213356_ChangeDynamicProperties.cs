using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wolfix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDynamicProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Products",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Bonuses",
                table: "Products",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalPrice",
                table: "Products",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "ProductsCount",
                table: "Categories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Bonuses",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "FinalPrice",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ProductsCount",
                table: "Categories");
        }
    }
}
