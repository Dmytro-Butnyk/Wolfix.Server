using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGuidIdToValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryAttributeId",
                schema: "catalog",
                table: "ProductAttributeValues",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryVariantId",
                schema: "catalog",
                table: "ProductVariantValues",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryAttributeId",
                schema: "catalog",
                table: "ProductAttributeValues");

            migrationBuilder.DropColumn(
                name: "CategoryVariantId",
                schema: "catalog",
                table: "ProductVariantValues");
        }
    }
}
