using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MagicBox.Data.Migrations
{
    public partial class m6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_Pictures_PictureId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_PictureId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "PictureId",
                table: "UserInfos");

            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "UserInfos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "UserInfos");

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
    }
}
