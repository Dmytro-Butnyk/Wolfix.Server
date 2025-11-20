using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seller.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Change : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                schema: "seller",
                table: "Sellers",
                type: "text",
                nullable: false,
                defaultValue: "Active"); // или нужное тебе значение по умолчанию
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                schema: "seller",
                table: "Sellers");
        }
    }
}