using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CastonFactory.Data.Migrations
{
    public partial class GenreAndConnectorTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GenreId",
                table: "Contents",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    CreateDate = table.Column<DateTime>(type: "TIMESTAMP", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    ContentId = table.Column<Guid>(nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Contents_GenreId",
                table: "Contents",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContents_ContentId",
                table: "UserContents",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContents_UserId",
                table: "UserContents",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contents_Genres_GenreId",
                table: "Contents",
                column: "GenreId",
                principalTable: "Genres",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contents_Genres_GenreId",
                table: "Contents");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "UserContents");

            migrationBuilder.DropIndex(
                name: "IX_Contents_GenreId",
                table: "Contents");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b654b9cd-adc7-466b-8971-f53e55f2dfe2");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "6dbd9372-8b7a-4a02-a3ca-3b0800351f10", "5ce0c307-a08d-4082-b301-ae89d1e84172" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5ce0c307-a08d-4082-b301-ae89d1e84172");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6dbd9372-8b7a-4a02-a3ca-3b0800351f10");

            migrationBuilder.DropColumn(
                name: "GenreId",
                table: "Contents");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b371eaf7-966e-4e79-87af-077ad36a3655", "82f59b1b-c8ab-496c-b6ef-94cdf50b7147", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "255920ad-74b8-44ba-b01a-c88f358fef9f", "8eaea95b-9fa0-4df5-b24e-a10be72e9a7d", "Otoriter", "otoriter" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName" },
                values: new object[] { "750769f2-0fc0-49a0-9fda-b02284cedc28", 0, "a9d8682a-1850-490a-8098-ff48ad69a69e", "software@caston.tv", true, false, null, null, "software@caston.tv", "admin", "AQAAAAEAACcQAAAAEOo0/xi+VOFl/y2aPo1VpsfXcgJ3WTwdLWqkUmOcIvlbxk4jRKLSK+c+KZyga+lQLw==", null, false, "", null, false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "750769f2-0fc0-49a0-9fda-b02284cedc28", "b371eaf7-966e-4e79-87af-077ad36a3655" });
        }
    }
}
