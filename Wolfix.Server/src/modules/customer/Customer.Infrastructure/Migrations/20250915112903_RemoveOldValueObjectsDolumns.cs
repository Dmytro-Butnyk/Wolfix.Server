using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveOldValueObjectsDolumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Address",
                schema: "customer",
                table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Address",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);
        }
    }
}
