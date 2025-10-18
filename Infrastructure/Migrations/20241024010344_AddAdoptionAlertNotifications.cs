using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionAlertNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdoptionAlertNotifications",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TimeStampUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AdoptionAlertId = table.Column<Guid>(type: "uuid", nullable: false),
                    HasBeenRead = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionAlertNotifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdoptionAlertNotifications_AdoptionAlerts_AdoptionAlertId",
                        column: x => x.AdoptionAlertId,
                        principalTable: "AdoptionAlerts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdoptionAlertNotificationUser",
                columns: table => new
                {
                    AdoptionAlertNotificationsId = table.Column<long>(type: "bigint", nullable: false),
                    UsersId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdoptionAlertNotificationUser", x => new { x.AdoptionAlertNotificationsId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_AdoptionAlertNotificationUser_AdoptionAlertNotifications_Ad~",
                        column: x => x.AdoptionAlertNotificationsId,
                        principalTable: "AdoptionAlertNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdoptionAlertNotificationUser_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlertNotifications_AdoptionAlertId",
                table: "AdoptionAlertNotifications",
                column: "AdoptionAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_AdoptionAlertNotificationUser_UsersId",
                table: "AdoptionAlertNotificationUser",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdoptionAlertNotificationUser");

            migrationBuilder.DropTable(
                name: "AdoptionAlertNotifications");
        }
    }
}
