using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoardsAPI.Migrations
{
    public partial class AddWeightToSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Workouts");

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Sets",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Sets");

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Workouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
