using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Todo.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMtOUserTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TeamId1",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TeamId1",
                table: "AspNetUsers",
                column: "TeamId1");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Teams_TeamId1",
                table: "AspNetUsers",
                column: "TeamId1",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Teams_TeamId1",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TeamId1",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamId1",
                table: "AspNetUsers");
        }
    }
}
