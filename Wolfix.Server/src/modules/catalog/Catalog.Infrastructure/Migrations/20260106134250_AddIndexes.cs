using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_isChild",
                schema: "catalog",
                table: "Categories",
                column: "ParentId",
                filter: "\"ParentId\" IS NULL");

            migrationBuilder.CreateIndex(
                name: "idx_UNIQUE_name",
                schema: "catalog",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductAttributeValues_CategoryAttributeId",
                schema: "catalog",
                table: "ProductAttributeValues",
                column: "CategoryAttributeId");

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_categoryId_SORT_finalPrice",
                schema: "catalog",
                table: "Products",
                columns: new[] { "CategoryId", "FinalPrice" });

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_sellerId",
                schema: "catalog",
                table: "Products",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_sellerId_EQUALS_categoryId",
                schema: "catalog",
                table: "Products",
                columns: new[] { "SellerId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantValues_CategoryVariantId",
                schema: "catalog",
                table: "ProductVariantValues",
                column: "CategoryVariantId");

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_productId_SORT_createdAt",
                schema: "catalog",
                table: "Reviews",
                columns: new[] { "ProductId", "CreatedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_EQUALS_isChild",
                schema: "catalog",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "idx_UNIQUE_name",
                schema: "catalog",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_ProductAttributeValues_CategoryAttributeId",
                schema: "catalog",
                table: "ProductAttributeValues");

            migrationBuilder.DropIndex(
                name: "idx_EQUALS_categoryId_SORT_finalPrice",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "idx_EQUALS_sellerId",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "idx_EQUALS_sellerId_EQUALS_categoryId",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_ProductVariantValues_CategoryVariantId",
                schema: "catalog",
                table: "ProductVariantValues");

            migrationBuilder.DropIndex(
                name: "idx_EQUALS_productId_SORT_createdAt",
                schema: "catalog",
                table: "Reviews");
        }
    }
}
