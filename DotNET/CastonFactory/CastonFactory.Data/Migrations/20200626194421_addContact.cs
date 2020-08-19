using Microsoft.EntityFrameworkCore.Migrations;

namespace CastonFactory.Data.Migrations
{
    public partial class addContact : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.AddColumn<string>(
                name: "Contact",
                table: "Contents",
                nullable: true);


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "255920ad-74b8-44ba-b01a-c88f358fef9f");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "UserId", "RoleId" },
                keyValues: new object[] { "750769f2-0fc0-49a0-9fda-b02284cedc28", "b371eaf7-966e-4e79-87af-077ad36a3655" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b371eaf7-966e-4e79-87af-077ad36a3655");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "750769f2-0fc0-49a0-9fda-b02284cedc28");

            migrationBuilder.DropColumn(
                name: "Contact",
                table: "Contents");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "874edbcb-ac02-4d16-8294-2f19cbd0c1db", "7eb32e3a-23c9-481a-8d22-09303fba34c5", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "b8590196-01f0-48ca-af30-8792f01c6420", "eceb0dd0-348c-48ce-a721-15d6e7fcd29d", "Otoriter", "otoriter" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "Name", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "Surname", "TwoFactorEnabled", "UserName" },
                values: new object[] { "b1a0c7af-908d-4b8a-b2c6-31ca26e9aaa1", 0, "b0955d83-ac47-4608-95f9-4f3226731654", "software@caston.tv", true, false, null, null, "software@caston.tv", "admin", "AQAAAAEAACcQAAAAEL4yn14V945gwlAQaPGcjHgPlOcueCT09vGRXUUZY4ow+HLqGK+CHQNE3X8EwEoPlQ==", null, false, "", null, false, "Admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { "b1a0c7af-908d-4b8a-b2c6-31ca26e9aaa1", "874edbcb-ac02-4d16-8294-2f19cbd0c1db" });
        }
    }
}
