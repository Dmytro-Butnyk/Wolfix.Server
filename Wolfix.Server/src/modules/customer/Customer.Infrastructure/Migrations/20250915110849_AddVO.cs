using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddVO : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ViolationsCount",
                schema: "customer",
                table: "Customers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "AccountStatus",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: false,
                defaultValue: "Active"); // или int, если enum хранится числом
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ViolationsCount",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "AccountStatus",
                schema: "customer",
                table: "Customers");
        }
    }
}
