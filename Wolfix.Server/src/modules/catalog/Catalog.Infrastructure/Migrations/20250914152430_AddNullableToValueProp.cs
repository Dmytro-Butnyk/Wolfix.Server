using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNullableToValueProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "catalog",
                table: "ProductVariantValues",
                type: "text",
                nullable: true,    // <-- вот оно!
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                schema: "catalog",
                table: "ProductVariantValues",
                type: "text",
                nullable: false,
                defaultValue: "", // если нужно
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
