using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wolfix.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBaseEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlobResource_BaseEntity_EntityId",
                table: "BlobResource");

            migrationBuilder.DropForeignKey(
                name: "FK_BlobResource_BaseEntity_Id",
                table: "BlobResource");

            migrationBuilder.DropForeignKey(
                name: "FK_Categories_BaseEntity_Id",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_Discounts_BaseEntity_Id",
                table: "Discounts");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributes_BaseEntity_Id",
                table: "ProductAttributes");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductAttributeValues_BaseEntity_Id",
                table: "ProductAttributeValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_BaseEntity_Id",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_BaseEntity_Id",
                table: "ProductVariants");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantValues_BaseEntity_Id",
                table: "ProductVariantValues");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_BaseEntity_Id",
                table: "Reviews");

            migrationBuilder.DropTable(
                name: "BaseEntity");

            migrationBuilder.DropIndex(
                name: "IX_BlobResource_EntityId",
                table: "BlobResource");

            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "BlobResource");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EntityId",
                table: "BlobResource",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BaseEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaseEntity", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlobResource_EntityId",
                table: "BlobResource",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlobResource_BaseEntity_EntityId",
                table: "BlobResource",
                column: "EntityId",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlobResource_BaseEntity_Id",
                table: "BlobResource",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_BaseEntity_Id",
                table: "Categories",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discounts_BaseEntity_Id",
                table: "Discounts",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributes_BaseEntity_Id",
                table: "ProductAttributes",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductAttributeValues_BaseEntity_Id",
                table: "ProductAttributeValues",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_BaseEntity_Id",
                table: "Products",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_BaseEntity_Id",
                table: "ProductVariants",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantValues_BaseEntity_Id",
                table: "ProductVariantValues",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_BaseEntity_Id",
                table: "Reviews",
                column: "Id",
                principalTable: "BaseEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
