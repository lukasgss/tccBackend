using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionFormToAdoptionAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionAlertComments");

            migrationBuilder.DropTable(
                name: "MissingAlertComments");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(255)",
                oldMaxLength: 255);

            migrationBuilder.AddColumn<string>(
                name: "DefaultAdoptionFormUrl",
                table: "AspNetUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdoptionForm_FileName",
                table: "AdoptionAlerts",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AdoptionForm_FileUrl",
                table: "AdoptionAlerts",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultAdoptionFormUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AdoptionForm_FileName",
                table: "AdoptionAlerts");

            migrationBuilder.DropColumn(
                name: "AdoptionForm_FileUrl",
                table: "AdoptionAlerts");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256);

            migrationBuilder.CreateTable(
                name: "AdoptionAlertComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AdoptionAlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionAlertComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionAlertComments_AdoptionAlerts_AdoptionAlertId",
                        column: x => x.AdoptionAlertId,
                        principalTable: "AdoptionAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionAlertComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MissingAlertComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MissingAlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Content = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MissingAlertComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MissingAlertComments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MissingAlertComments_MissingAlerts_MissingAlertId",
                        column: x => x.MissingAlertId,
                        principalTable: "MissingAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlertComments_AdoptionAlertId",
                table: "AdoptionAlertComments",
                column: "AdoptionAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlertComments_UserId",
                table: "AdoptionAlertComments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlertComments_MissingAlertId",
                table: "MissingAlertComments",
                column: "MissingAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_MissingAlertComments_UserId",
                table: "MissingAlertComments",
                column: "UserId");
        }
    }
}
