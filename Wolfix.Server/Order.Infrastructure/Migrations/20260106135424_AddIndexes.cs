using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Number",
                schema: "order",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "019b9396-5f89-7ea1-9287-592246cafdbf",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "e2afca3d-7886-4210-8da2-6d018921beb3");

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_customerId_SORT_createdAt",
                schema: "order",
                table: "Orders",
                columns: new[] { "CustomerId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "idx_UNIQUE_number",
                schema: "order",
                table: "Orders",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_EQUALS_sellerId",
                schema: "order",
                table: "OrderItems",
                column: "SellerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_EQUALS_customerId_SORT_createdAt",
                schema: "order",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "idx_UNIQUE_number",
                schema: "order",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "idx_EQUALS_sellerId",
                schema: "order",
                table: "OrderItems");

            migrationBuilder.AlterColumn<string>(
                name: "Number",
                schema: "order",
                table: "Orders",
                type: "text",
                nullable: false,
                defaultValue: "e2afca3d-7886-4210-8da2-6d018921beb3",
                oldClrType: typeof(string),
                oldType: "text",
                oldDefaultValue: "019b9396-5f89-7ea1-9287-592246cafdbf");
        }
    }
}
