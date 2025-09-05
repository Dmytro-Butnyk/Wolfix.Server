using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Remove : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_BlobResource_PhotoId",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropTable(
                name: "BlobResource",
                schema: "catalog");

            migrationBuilder.DropIndex(
                name: "IX_Products_PhotoId",
                schema: "catalog",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                schema: "catalog",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PhotoId",
                schema: "catalog",
                table: "Products",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BlobResource",
                schema: "catalog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BlobName = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<byte>(type: "smallint", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlobResource", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_PhotoId",
                schema: "catalog",
                table: "Products",
                column: "PhotoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_BlobResource_PhotoId",
                schema: "catalog",
                table: "Products",
                column: "PhotoId",
                principalSchema: "catalog",
                principalTable: "BlobResource",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
