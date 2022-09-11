using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyBoardsAPI.Migrations
{
    public partial class FeatureIsRoadmap : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRoadMap",
                table: "Feature",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRoadMap",
                table: "Feature");
        }
    }
}
