using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoardsAPI.Migrations
{
    public partial class hasCreatedFirstsPascal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "hasCreatedFirstWorkout",
                table: "AspNetUsers",
                newName: "HasCreatedFirstWorkout");

            migrationBuilder.RenameColumn(
                name: "hasCreatedFirstHangboard",
                table: "AspNetUsers",
                newName: "HasCreatedFirstHangboard");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "HasCreatedFirstWorkout",
                table: "AspNetUsers",
                newName: "hasCreatedFirstWorkout");

            migrationBuilder.RenameColumn(
                name: "HasCreatedFirstHangboard",
                table: "AspNetUsers",
                newName: "hasCreatedFirstHangboard");
        }
    }
}
