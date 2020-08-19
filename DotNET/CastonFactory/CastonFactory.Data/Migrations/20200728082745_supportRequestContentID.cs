using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CastonFactory.Data.Migrations
{
    public partial class supportRequestContentID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContentId",
                table: "SupportRequests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportRequests_ContentId",
                table: "SupportRequests",
                column: "ContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportRequests_Contents_ContentId",
                table: "SupportRequests",
                column: "ContentId",
                principalTable: "Contents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportRequests_Contents_ContentId",
                table: "SupportRequests");

            migrationBuilder.DropIndex(
                name: "IX_SupportRequests_ContentId",
                table: "SupportRequests");

            migrationBuilder.DropColumn(
                name: "ContentId",
                table: "SupportRequests");
        }
    }
}
