using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WishList.DAL.Core.Migrations
{
    public partial class Roles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "Admin",
                    "ADMIN",
                    Guid.NewGuid().ToString()
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[]
                {
                    Guid.NewGuid(),
                    "User",
                    "USER",
                    Guid.NewGuid().ToString()
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
