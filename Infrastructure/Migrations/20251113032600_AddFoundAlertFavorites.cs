using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFoundAlertFavorites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FoundAnimalFavorites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FoundAnimalAlertId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundAnimalFavorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoundAnimalFavorites_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundAnimalFavorites_FoundAnimalAlerts_FoundAnimalAlertId",
                        column: x => x.FoundAnimalAlertId,
                        principalTable: "FoundAnimalAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalFavorites_FoundAnimalAlertId",
                table: "FoundAnimalFavorites",
                column: "FoundAnimalAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundAnimalFavorites_UserId",
                table: "FoundAnimalFavorites",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FoundAnimalFavorites");
        }
    }
}
