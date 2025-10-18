using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixStoredColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.AlterColumn<List<string>>(
                name: "AdoptionRestrictions",
                table: "AdoptionAlerts",
                type: "text[]",
                maxLength: 100,
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldMaxLength: 100,
                oldDefaultValue: new List<string>());

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#C68E17", "Caramelo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4,
                column: "HexCode",
                value: "#8B4513");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#808080", "Cinza" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#FFA500", "Laranja" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#FFFDD0", "Creme" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#7B3F00", "Chocolate" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#D2691E", "Canela" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#DEB887", "Fulvo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#CD5C5C", "Ruivo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#FA8072", "Salmão" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#F5F5DC", "Bege" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#8B4513", "Tigrado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<string>>(
                name: "AdoptionRestrictions",
                table: "AdoptionAlerts",
                type: "text[]",
                maxLength: 100,
                nullable: false,
                defaultValue: new List<string>(),
                oldClrType: typeof(List<string>),
                oldType: "text[]",
                oldMaxLength: 100,
                oldDefaultValue: new List<string>());

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#808080", "Cinza" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 4,
                column: "HexCode",
                value: "#A52A2A");

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#FFD700", "Dourado" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#C0C0C0", "Prateado" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#F5F5DC", "Bege" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#D2691E", "Tigrado" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#FFFDD0", "Creme" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#FF4500", "Ruivo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#4682B4", "Azul" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#C68E17", "Caramelo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#7B3F00", "Chocolate" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#6B4226", "Fígado" });

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "HexCode", "Name" },
                values: new object[,]
                {
                    { 15, "#C8A2C8", "Lilás" },
                    { 16, "#704214", "Sépia" },
                    { 17, "#3B3B3B", "Tigrado Preto" },
                    { 18, "#8E593C", "Zibelina" },
                    { 19, "#D2B48C", "Areia" },
                    { 20, "#FFF5EE", "Albino" }
                });
        }
    }
}
