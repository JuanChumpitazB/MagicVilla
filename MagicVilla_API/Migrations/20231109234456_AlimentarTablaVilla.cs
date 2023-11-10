using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagicVillaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlimentarTablaVilla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Villas",
                columns: new[] { "Id", "Amenidad", "Detalle", "FechaActualizacion", "FechaCreacion", "ImagenURL", "MetrosCuadrados", "Nombre", "Ocupantes", "Tarifa" },
                values: new object[,]
                {
                    { 1, "", "Detalle de Villa...", new DateTime(2023, 11, 9, 18, 44, 56, 137, DateTimeKind.Local).AddTicks(5056), new DateTime(2023, 11, 9, 18, 44, 56, 137, DateTimeKind.Local).AddTicks(5048), "", 50, "Villa Real", 5, 200.0 },
                    { 2, "", "Detalle de Villa Moderna...", new DateTime(2023, 11, 9, 18, 44, 56, 137, DateTimeKind.Local).AddTicks(5060), new DateTime(2023, 11, 9, 18, 44, 56, 137, DateTimeKind.Local).AddTicks(5060), "", 40, "Villa Moderna", 4, 400.0 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Villas",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
