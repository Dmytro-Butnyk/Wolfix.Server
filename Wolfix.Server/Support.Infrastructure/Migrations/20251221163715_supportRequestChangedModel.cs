using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class supportRequestChangedModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "support",
                table: "SupportRequests");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "support",
                table: "SupportRequests",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "Category",
                schema: "support",
                table: "SupportRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                schema: "support",
                table: "SupportRequests");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "BirthDate",
                schema: "support",
                table: "SupportRequests",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "support",
                table: "SupportRequests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
