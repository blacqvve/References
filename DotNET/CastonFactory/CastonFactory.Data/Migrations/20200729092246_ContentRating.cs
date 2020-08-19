using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CastonFactory.Data.Migrations
{
    public partial class ContentRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentRatings",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ViewCount = table.Column<int>(nullable: false),
                    ManagerRating = table.Column<int>(nullable: false),
                    ContentID = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentRatings_Contents_ContentID",
                        column: x => x.ContentID,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContentRatings_ContentID",
                table: "ContentRatings",
                column: "ContentID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentRatings");
        }
    }
}
