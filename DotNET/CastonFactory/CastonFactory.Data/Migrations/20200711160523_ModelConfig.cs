using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CastonFactory.Data.Migrations
{
    public partial class ModelConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserContents");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "8ef94b6f-a4e1-4cb7-99b3-d5392bb075d8");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Contents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ContentDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    FileExtension = table.Column<string>(nullable: true),
                    ContentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentDatas_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "679df82d-dc15-43eb-b9d2-28969674635b", "80ca6d9c-92de-45d4-91f4-ad25e729483a", "ContentProducer", "contentproducer" });

            migrationBuilder.CreateIndex(
                name: "IX_Contents_UserId",
                table: "Contents",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentDatas_ContentId",
                table: "ContentDatas",
                column: "ContentId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_AspNetUsers_UserId",
                table: "Contents",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_AspNetUsers_UserId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "ContentDatas");

            migrationBuilder.DropIndex(
                name: "IX_Contents_UserId",
                table: "Contents");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "679df82d-dc15-43eb-b9d2-28969674635b");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Contents");

            migrationBuilder.CreateTable(
                name: "UserContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ContentId = table.Column<Guid>(type: "char(36)", nullable: true),
                    UserId = table.Column<string>(type: "varchar(85) CHARACTER SET utf8mb4", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContents_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserContents_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "8ef94b6f-a4e1-4cb7-99b3-d5392bb075d8", "50d8a74c-1347-413e-9b14-6aa5f6a0485f", "ContentProducer", "contentproducer" });

            migrationBuilder.CreateIndex(
                name: "IX_UserContents_ContentId",
                table: "UserContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContents_UserId",
                table: "UserContents",
                column: "UserId");
        }
    }
}
