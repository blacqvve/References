using Microsoft.EntityFrameworkCore.Migrations;

namespace CastonFactory.Data.Migrations
{
    public partial class contentModelClear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Contact",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "ContentData",
                table: "Contents");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Contents");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "Contents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentData",
                table: "Contents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Contents",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
