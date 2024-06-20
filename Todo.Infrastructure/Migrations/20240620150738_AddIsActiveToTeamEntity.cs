using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsActiveToTeamEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Teams",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CreationDate",
                table: "Tasks",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2024, 6, 20),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2024, 6, 19));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Teams");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "CreationDate",
                table: "Tasks",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(2024, 6, 19),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldDefaultValue: new DateOnly(2024, 6, 20));
        }
    }
}
