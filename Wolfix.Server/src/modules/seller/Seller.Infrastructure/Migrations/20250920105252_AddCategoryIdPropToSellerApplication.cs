using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seller.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryIdPropToSellerApplication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "seller",
                table: "SellerApplications",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "seller",
                table: "SellerApplications");
        }
    }
}
