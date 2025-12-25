using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updatedSupportRequestModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "support",
                table: "SupportRequests");

            migrationBuilder.DropColumn(
                name: "Title",
                schema: "support",
                table: "SupportRequests");

            migrationBuilder.AlterColumn<string>(
                name: "Category",
                schema: "support",
                table: "SupportRequests",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Category",
                schema: "support",
                table: "SupportRequests",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "support",
                table: "SupportRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "support",
                table: "SupportRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
