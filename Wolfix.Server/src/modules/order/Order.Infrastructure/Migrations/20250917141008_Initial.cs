using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "order");

            migrationBuilder.CreateTable(
                name: "DeliveryMethods",
                schema: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryMethods", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                schema: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Customer_FirstName = table.Column<string>(type: "text", nullable: false),
                    Customer_LastName = table.Column<string>(type: "text", nullable: false),
                    Customer_MiddleName = table.Column<string>(type: "text", nullable: false),
                    Customer_PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    Customer_Email = table.Column<string>(type: "text", nullable: false),
                    CustomerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Recipient_FirstName = table.Column<string>(type: "text", nullable: false),
                    Recipient_LastName = table.Column<string>(type: "text", nullable: false),
                    Recipient_MiddleName = table.Column<string>(type: "text", nullable: false),
                    Recipient_PhoneNumber = table.Column<string>(type: "text", nullable: false),
                    PaymentOption = table.Column<string>(type: "text", nullable: false),
                    PaymentStatus = table.Column<string>(type: "text", nullable: false),
                    DeliveryNumber = table.Column<long>(type: "bigint", nullable: true),
                    DeliveryCity = table.Column<string>(type: "text", nullable: false),
                    DeliveryStreet = table.Column<string>(type: "text", nullable: false),
                    DeliveryHouseNumber = table.Column<long>(type: "bigint", nullable: false),
                    DeliveryOption = table.Column<string>(type: "text", nullable: false),
                    DeliveryMethodName = table.Column<string>(type: "text", nullable: false),
                    WithBonuses = table.Column<bool>(type: "boolean", nullable: false),
                    UsedBonusesAmount = table.Column<decimal>(type: "numeric", nullable: false, defaultValue: 0m),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                schema: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    DeliveryMethodId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_DeliveryMethods_DeliveryMethodId",
                        column: x => x.DeliveryMethodId,
                        principalSchema: "order",
                        principalTable: "DeliveryMethods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                schema: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PhotoUrl = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    Price = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalSchema: "order",
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                schema: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    HouseNumber = table.Column<long>(type: "bigint", nullable: false),
                    CityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Departments_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "order",
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostMachines",
                schema: "order",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<long>(type: "bigint", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    HouseNumber = table.Column<long>(type: "bigint", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    CityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostMachines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostMachines_Cities_CityId",
                        column: x => x.CityId,
                        principalSchema: "order",
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_DeliveryMethodId",
                schema: "order",
                table: "Cities",
                column: "DeliveryMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_CityId",
                schema: "order",
                table: "Departments",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                schema: "order",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PostMachines_CityId",
                schema: "order",
                table: "PostMachines",
                column: "CityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Departments",
                schema: "order");

            migrationBuilder.DropTable(
                name: "OrderItems",
                schema: "order");

            migrationBuilder.DropTable(
                name: "PostMachines",
                schema: "order");

            migrationBuilder.DropTable(
                name: "Orders",
                schema: "order");

            migrationBuilder.DropTable(
                name: "Cities",
                schema: "order");

            migrationBuilder.DropTable(
                name: "DeliveryMethods",
                schema: "order");
        }
    }
}
