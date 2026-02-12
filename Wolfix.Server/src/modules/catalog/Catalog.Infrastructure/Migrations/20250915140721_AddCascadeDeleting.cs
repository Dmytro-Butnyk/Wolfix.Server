using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCascadeDeleting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Для ProductAttributeValues → ProductAttributes
            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributes_CategoryAttributeId",
                schema: "catalog",
                table: "ProductAttributeValues",
                column: "CategoryAttributeId",
                principalSchema: "catalog",
                principalTable: "ProductAttributes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Для ProductVariantValues → ProductVariants
            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantValues_ProductVariants_CategoryVariantId",
                schema: "catalog",
                table: "ProductVariantValues",
                column: "CategoryVariantId",
                principalSchema: "catalog",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_ProductAttributes_CategoryAttributeId",
                schema: "catalog",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantValues_ProductVariants_CategoryVariantId",
                schema: "catalog",
                table: "ProductVariantValues");
        }
    }
}
