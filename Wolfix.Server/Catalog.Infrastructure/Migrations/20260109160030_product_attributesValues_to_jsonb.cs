using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class product_attributesValues_to_jsonb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalog");
            // 1. Добавляем колонку для JSON (как и планировали)
            migrationBuilder.AddColumn<string>(
                name: "ProductAttributeValues", // Имя свойства в классе Product (или то, как EF его замапил)
                schema: "catalog",
                table: "Products",
                type: "jsonb",              // Для Postgres используем jsonb
                nullable: true);

            // 2. Копируем данные из старой таблицы в новую колонку
            // ВАЖНО: Мы формируем массив объектов. Ключи JSON должны совпадать с именами свойств в C# Record!
            migrationBuilder.Sql(@"
                UPDATE ""catalog"".""Products"" p
                SET ""ProductAttributeValues"" = (
                    SELECT jsonb_agg(
                        jsonb_build_object(
                            'CategoryAttributeId', pav.""CategoryAttributeId"",
                            'Key', pav.""Key"", 
                            'Value', pav.""Value""
                        )
                    )
                    FROM ""catalog"".""ProductAttributeValues"" pav
                    WHERE pav.""ProductId"" = p.""Id""
                );
            ");

            // 3. А вот удаление таблицы мы КОММЕНТИРУЕМ.
            // Таблица останется в базе, но EF Core про неё "забудет" (так как её нет в DbContext).
            // migrationBuilder.DropTable(
            //     name: "ProductAttributeValues",
            //     schema: "catalog");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Если нужно откатить миграцию (вернуть код на старую версию),
            // мы просто удаляем созданную JSON колонку.
            // Старую таблицу восстанавливать не надо — ОНА И ТАК ТАМ ЕСТЬ.
    
            migrationBuilder.DropColumn(
                name: "ProductAttributeValues",
                schema: "catalog",
                table: "Products");
        }
    }
}
