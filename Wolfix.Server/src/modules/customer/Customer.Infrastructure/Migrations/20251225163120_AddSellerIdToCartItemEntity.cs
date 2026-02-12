using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSellerIdToCartItemEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SellerId",
                schema: "customer",
                table: "CartItems",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerId",
                schema: "customer",
                table: "CartItems");
        }
    }
}
