using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoardsAPI.Migrations
{
    public partial class WorkoutWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Workouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Workouts");
        }
    }
}
