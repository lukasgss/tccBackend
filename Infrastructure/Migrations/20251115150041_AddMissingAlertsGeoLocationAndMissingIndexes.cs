using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingAlertsGeoLocationAndMissingIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MissingAlerts",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "MissingAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "MissingAlerts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "MissingAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Neighborhood",
                table: "FoundAnimalAlerts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_Age",
                table: "Pets",
                column: "Age");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_Gender",
                table: "Pets",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_Pets_Size",
                table: "Pets",
                column: "Size");

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlerts_CityId",
                table: "MissingAlerts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlerts_Location",
                table: "MissingAlerts",
                column: "Location");

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlerts_StateId",
                table: "MissingAlerts",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_Age",
                table: "FoundAnimalAlerts",
                column: "Age");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_Gender",
                table: "FoundAnimalAlerts",
                column: "Gender");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalAlerts_Size",
                table: "FoundAnimalAlerts",
                column: "Size");

            migrationBuilder.AddForeignKey(
                name: "FK_MissingAlerts_Cities_CityId",
                table: "MissingAlerts",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MissingAlerts_States_StateId",
                table: "MissingAlerts",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MissingAlerts_Cities_CityId",
                table: "MissingAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_MissingAlerts_States_StateId",
                table: "MissingAlerts");

            migrationBuilder.DropIndex(
                name: "IX_Pets_Age",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Pets_Gender",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_Pets_Size",
                table: "Pets");

            migrationBuilder.DropIndex(
                name: "IX_MissingAlerts_CityId",
                table: "MissingAlerts");

            migrationBuilder.DropIndex(
                name: "IX_MissingAlerts_Location",
                table: "MissingAlerts");

            migrationBuilder.DropIndex(
                name: "IX_MissingAlerts_StateId",
                table: "MissingAlerts");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalAlerts_Age",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalAlerts_Gender",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropIndex(
                name: "IX_FoundAnimalAlerts_Size",
                table: "FoundAnimalAlerts");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "MissingAlerts");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "MissingAlerts");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "MissingAlerts");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "MissingAlerts",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Neighborhood",
                table: "FoundAnimalAlerts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);
        }
    }
}
