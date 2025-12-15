using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace GestionIntApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedDataInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "Icono", "Nombre", "Url" },
                values: new object[,]
                {
                    { 1, "dashboard", "DashBoard", "/pages/dashboard" },
                    { 2, "group", "Usuarios", "/pages/usuarios" },
                    { 3, "fa-user", "Clientes", "/clientes" },
                    { 4, "currency_exchange", "Venta", "/pages/venta" },
                    { 5, "edit_note", "Historial", "/pages/historial_venta" },
                    { 6, "receipt", "Reportes", "/pages/reportes" }
                });

            migrationBuilder.InsertData(
                table: "Rol",
                columns: new[] { "Id", "Descripcion", "FechaRegistro" },
                values: new object[,]
                {
                    { 1, "Administrador", new DateTime(2025, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, "Cliente", new DateTime(2025, 12, 12, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "MenuRols",
                columns: new[] { "Id", "MenuId", "RolId" },
                values: new object[,]
                {
                    { 1, 1, 1 },
                    { 2, 2, 1 },
                    { 3, 3, 1 },
                    { 4, 4, 1 },
                    { 5, 5, 1 },
                    { 6, 6, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuRols",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MenuRols",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MenuRols",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MenuRols",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MenuRols",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MenuRols",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rol",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Menus",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Rol",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
