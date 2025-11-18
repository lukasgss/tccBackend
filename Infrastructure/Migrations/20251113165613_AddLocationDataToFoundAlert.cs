using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationDataToFoundAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "FoundAnimalAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "FoundAnimalAlerts",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "FoundAnimalAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_CityId",
                table: "FoundAnimalAlerts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_StateId",
                table: "FoundAnimalAlerts",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_FoundAnimalAlerts_Cities_CityId",
                table: "FoundAnimalAlerts",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FoundAnimalAlerts_States_StateId",
                table: "FoundAnimalAlerts",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalAlerts_Cities_CityId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_FoundAnimalAlerts_States_StateId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalAlerts_CityId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalAlerts_StateId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "FoundAnimalAlerts");
        }
    }
}
