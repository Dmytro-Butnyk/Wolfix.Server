using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Customer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Убедимся, что схема существует (безопасно если она уже есть)
            migrationBuilder.EnsureSchema(name: "customer");

            // Добавляем поля для FullName (разложенный VO)
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MiddleName",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);

            // Добавляем поля для Address (разложенный VO)
            migrationBuilder.AddColumn<string>(
                name: "City",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                schema: "customer",
                table: "Customers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "HouseNumber",
                schema: "customer",
                table: "Customers",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ApartmentNumber",
                schema: "customer",
                table: "Customers",
                type: "bigint",
                nullable: true);

            // Примечание: другие колонки (PhoneNumber, BirthDate, ViolationsCount, AccountStatus, BonusesAmount, AccountId и т.д.)
            // предполагается, что уже существуют в таблице и поэтому сюда не помещаем их снова.
            //
            // Если ты хочешь одновременно удалить старые сериализованные колонки (FullName, Address),
            // расскомментируй и используй следующие строки (будь осторожен — это удалит данные):
            //
            // migrationBuilder.DropColumn(name: "FullName", schema: "customer", table: "Customers");
            // migrationBuilder.DropColumn(name: "Address", schema: "customer", table: "Customers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // В обратной миграции -- удаляем добавленные поля
            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "MiddleName",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "City",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "Street",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "HouseNumber",
                schema: "customer",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                schema: "customer",
                table: "Customers");

            // Если ты удалял старые колонки в Up(), можешь восстановить их здесь (вместо реального восстановления данных
            // будет создана пустая колонка):
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "FullName",
            //     schema: "customer",
            //     table: "Customers",
            //     type: "text",
            //     nullable: true);
            //
            // migrationBuilder.AddColumn<string>(
            //     name: "Address",
            //     schema: "customer",
            //     table: "Customers",
            //     type: "text",
            //     nullable: true);
        }
    }
}
