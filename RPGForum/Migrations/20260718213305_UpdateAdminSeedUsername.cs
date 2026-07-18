using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPGForum.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAdminSeedUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "NormalizedUserName", "UserName" },
                values: new object[] { "ADMIN", "admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin",
                columns: new[] { "NormalizedUserName", "UserName" },
                values: new object[] { "ADMIN@MAIL.PT", "admin@mail.pt" });
        }
    }
}
