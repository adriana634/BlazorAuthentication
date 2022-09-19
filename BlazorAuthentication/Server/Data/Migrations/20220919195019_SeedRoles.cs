using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorAuthentication.Server.Data.Migrations
{
    public partial class SeedRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "0c0ce790-5c4f-450a-9196-2d0c38a45b87", "62d0eef4-8dab-429e-b33f-5c6e7563453c", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "16dc2538-5588-4513-9845-6b1ce290d6ad", "f1cbdc3c-52ec-4aef-b871-f358661299a6", "Admin", "ADMIN" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "0c0ce790-5c4f-450a-9196-2d0c38a45b87");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16dc2538-5588-4513-9845-6b1ce290d6ad");
        }
    }
}
