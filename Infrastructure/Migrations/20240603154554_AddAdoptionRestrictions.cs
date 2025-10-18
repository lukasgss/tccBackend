using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAdoptionRestrictions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnlyForScreenedProperties",
                table: "AdoptionAlerts");

            migrationBuilder.AddColumn<List<string>>(
                name: "AdoptionRestrictions",
                table: "AdoptionAlerts",
                type: "text[]",
                maxLength: 100,
                nullable: false,
                defaultValue: new List<string>());
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdoptionRestrictions",
                table: "AdoptionAlerts");

            migrationBuilder.AddColumn<bool>(
                name: "OnlyForScreenedProperties",
                table: "AdoptionAlerts",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
