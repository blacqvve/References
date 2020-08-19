using Microsoft.EntityFrameworkCore.Migrations;

namespace MagicBox.Data.Migrations
{
    public partial class m3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Stock",
                table: "ShopItems",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<float>(
                name: "Price",
                table: "ShopItems",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Stock",
                table: "ShopItems",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "ShopItems",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
