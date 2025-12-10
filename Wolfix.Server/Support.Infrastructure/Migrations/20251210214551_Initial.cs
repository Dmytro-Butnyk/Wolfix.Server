using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "support");

            migrationBuilder.CreateTable(
                name: "Supports",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SupportRequests",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: true),
                    RequestContent = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    SupportId = table.Column<Guid>(type: "uuid", nullable: true),
                    ResponseContent = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportRequests_Supports_SupportId",
                        column: x => x.SupportId,
                        principalSchema: "support",
                        principalTable: "Supports",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_SupportId",
                schema: "support",
                table: "SupportRequests",
                column: "SupportId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportRequests",
                schema: "support");

            migrationBuilder.DropTable(
                name: "Supports",
                schema: "support");
        }
    }
}
