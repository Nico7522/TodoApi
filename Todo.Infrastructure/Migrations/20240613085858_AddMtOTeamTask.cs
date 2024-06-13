using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMtOTeamTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId1",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TeamId1",
                table: "Tasks",
                column: "TeamId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Teams_TeamId1",
                table: "Tasks",
                column: "TeamId1",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Teams_TeamId1",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TeamId1",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                table: "Tasks");
        }
    }
}
