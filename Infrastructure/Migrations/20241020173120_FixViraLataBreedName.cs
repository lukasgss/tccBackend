using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixViraLataBreedName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 89,
                column: "Name",
                value: "Vira-lata/SRD");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 141,
                column: "Name",
                value: "Vira-lata/SRD");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 89,
                column: "Name",
                value: "SRD");

            migrationBuilder.UpdateData(
                table: "Breeds",
                keyColumn: "Id",
                keyValue: 141,
                column: "Name",
                value: "SRD");
        }
    }
}
