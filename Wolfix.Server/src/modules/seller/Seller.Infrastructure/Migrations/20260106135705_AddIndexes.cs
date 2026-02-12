using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seller.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_status",
                schema: "seller",
                table: "SellerApplications",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_EQUALS_status",
                schema: "seller",
                table: "SellerApplications");
        }
    }
}
