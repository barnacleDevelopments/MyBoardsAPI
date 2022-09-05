using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoardsAPI.Migrations
{
    public partial class hasCreatedFirsts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "hasCreatedFirstHangboard",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "hasCreatedFirstWorkout",
                table: "AspNetUsers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hasCreatedFirstHangboard",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "hasCreatedFirstWorkout",
                table: "AspNetUsers");
        }
    }
}
