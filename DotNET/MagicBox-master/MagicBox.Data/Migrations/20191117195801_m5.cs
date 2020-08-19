using Microsoft.EntityFrameworkCore.Migrations;

namespace MagicBox.Data.Migrations
{
    public partial class m5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "UserInfos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserInfos",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PictureId",
                table: "UserInfos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_PictureId",
                table: "UserInfos",
                column: "PictureId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_Pictures_PictureId",
                table: "UserInfos",
                column: "PictureId",
                principalTable: "Pictures",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_Pictures_PictureId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_PictureId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "UserInfos");

            migrationBuilder.AddColumn<string>(
                name: "Test",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: "");
        }
    }
}
