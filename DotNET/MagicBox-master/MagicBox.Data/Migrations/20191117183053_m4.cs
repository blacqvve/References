using Microsoft.EntityFrameworkCore.Migrations;

namespace MagicBox.Data.Migrations
{
    public partial class m4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CouponCodes",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "CouponCodes");

        }
    }
}
