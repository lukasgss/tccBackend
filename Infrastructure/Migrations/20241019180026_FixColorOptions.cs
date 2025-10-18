using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixColorOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#DEB887", "Fulvo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#CD5C5C", "Ruivo" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#E4D5B7", "Bege" });

            migrationBuilder.UpdateData(
                table: "Colors",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "HexCode", "Name" },
                values: new object[] { "#8B4513", "Tigrado" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.InsertData(
                table: "Colors",
                columns: new[] { "Id", "HexCode", "Name" },
                values: new object[,]
                {
                    { 13, "#F5F5DC", "Bege" },
                    { 14, "#8B4513", "Tigrado" }
                });
        }
    }
}
