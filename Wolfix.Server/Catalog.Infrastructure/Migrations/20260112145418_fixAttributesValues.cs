using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixAttributesValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");
            // 3. А вот удаление таблицы мы КОММЕНТИРУЕМ.
            // Таблица останется в базе, но EF Core про неё "забудет" (так как её нет в DbContext).
            migrationBuilder.DropTable(
                name: "ProductAttributeValues",
                schema: "catalog");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
