using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAddressToAdoptionAlert : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AdoptionAlerts",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AdoptionAlerts");

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
        }
    }
}
