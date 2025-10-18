using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStateAndCityToAdoptionAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetVaccine");

            migrationBuilder.DropTable(
                name: "SpeciesVaccine");

            migrationBuilder.DropTable(
                name: "Vaccines");

            migrationBuilder.DropColumn(
                name: "Observations",
                table: "Pets");

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

            migrationBuilder.AddColumn<int>(
                name: "CityId",
                table: "AdoptionAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StateId",
                table: "AdoptionAlerts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlerts_CityId",
                table: "AdoptionAlerts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlerts_StateId",
                table: "AdoptionAlerts",
                column: "StateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionAlerts_Cities_CityId",
                table: "AdoptionAlerts",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AdoptionAlerts_States_StateId",
                table: "AdoptionAlerts",
                column: "StateId",
                principalTable: "States",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionAlerts_Cities_CityId",
                table: "AdoptionAlerts");

            migrationBuilder.DropForeignKey(
                name: "FK_AdoptionAlerts_States_StateId",
                table: "AdoptionAlerts");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionAlerts_CityId",
                table: "AdoptionAlerts");

            migrationBuilder.DropIndex(
                name: "IX_AdoptionAlerts_StateId",
                table: "AdoptionAlerts");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "AdoptionAlerts");

            migrationBuilder.DropColumn(
                name: "StateId",
                table: "AdoptionAlerts");

            migrationBuilder.AddColumn<string>(
                name: "Observations",
                table: "Pets",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

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

            migrationBuilder.CreateTable(
                name: "Vaccines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vaccines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PetVaccine",
                columns: table => new
                {
                    PetsId = table.Column<Guid>(type: "uuid", nullable: false),
                    VaccinesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetVaccine", x => new { x.PetsId, x.VaccinesId });
                    table.ForeignKey(
                        name: "FK_PetVaccine_Pets_PetsId",
                        column: x => x.PetsId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PetVaccine_Vaccines_VaccinesId",
                        column: x => x.VaccinesId,
                        principalTable: "Vaccines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpeciesVaccine",
                columns: table => new
                {
                    SpeciesId = table.Column<int>(type: "integer", nullable: false),
                    VaccinesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeciesVaccine", x => new { x.SpeciesId, x.VaccinesId });
                    table.ForeignKey(
                        name: "FK_SpeciesVaccine_Species_SpeciesId",
                        column: x => x.SpeciesId,
                        principalTable: "Species",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpeciesVaccine_Vaccines_VaccinesId",
                        column: x => x.VaccinesId,
                        principalTable: "Vaccines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetVaccine_VaccinesId",
                table: "PetVaccine",
                column: "VaccinesId");

            migrationBuilder.CreateIndex(
                name: "IX_SpeciesVaccine_VaccinesId",
                table: "SpeciesVaccine",
                column: "VaccinesId");
        }
    }
}
