using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HotelListing3.API.Migrations
{
    /// <inheritdoc />
    public partial class AddedDefaultRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3c2113ba-573b-46a3-8fa3-c69ada4ccdeb", "f71be4e4-5e45-458e-816c-9d8043677952", "User", "USER" },
                    { "3d881e35-dc08-4d6b-8ebc-8a8a4829a1ed", "e8015b46-7718-4ae4-9b50-182eeeb381fa", "Administrator", "ADMINISTRATOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3c2113ba-573b-46a3-8fa3-c69ada4ccdeb");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3d881e35-dc08-4d6b-8ebc-8a8a4829a1ed");
        }
    }
}
