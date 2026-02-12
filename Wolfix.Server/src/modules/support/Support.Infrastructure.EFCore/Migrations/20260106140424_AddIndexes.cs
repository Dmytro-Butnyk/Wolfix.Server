using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Support.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SupportRequests",
                schema: "support");

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_fullName",
                schema: "support",
                table: "Supports",
                columns: new[] { "LastName", "FirstName", "MiddleName" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_EQUALS_fullName",
                schema: "support",
                table: "Supports");

            migrationBuilder.CreateTable(
                name: "SupportRequests",
                schema: "support",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SupportId = table.Column<Guid>(type: "uuid", nullable: true),
                    Category = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProcessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RequestContent = table.Column<string>(type: "text", nullable: false),
                    ResponseContent = table.Column<string>(type: "text", nullable: false, defaultValue: ""),
                    Status = table.Column<string>(type: "text", nullable: false, defaultValue: "Pending"),
                    BirthDate = table.Column<DateOnly>(type: "date", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    MiddleName = table.Column<string>(type: "text", nullable: false),
                    PhoneNumber = table.Column<string>(type: "text", nullable: false)
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
    }
}
