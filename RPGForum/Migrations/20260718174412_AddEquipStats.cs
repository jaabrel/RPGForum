using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPGForum.Migrations
{
    /// <inheritdoc />
    public partial class AddEquipStats : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Personagens_CharClassId",
                table: "Builds");

            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Utilizadores_UserId",
                table: "Builds");

            migrationBuilder.DropIndex(
                name: "IX_Builds_CharClassId",
                table: "Builds");

            migrationBuilder.DropIndex(
                name: "IX_Builds_UserId",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "CharClassId",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Builds");

            migrationBuilder.AddColumn<string>(
                name: "StatAfetada",
                table: "Armas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatBonus",
                table: "Armas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatAfetada",
                table: "Acessorios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StatBonus",
                table: "Acessorios",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Builds_CharacterId",
                table: "Builds",
                column: "CharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Builds_UtilizadorID",
                table: "Builds",
                column: "UtilizadorID");

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Personagens_CharacterId",
                table: "Builds",
                column: "CharacterId",
                principalTable: "Personagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Utilizadores_UtilizadorID",
                table: "Builds",
                column: "UtilizadorID",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Personagens_CharacterId",
                table: "Builds");

            migrationBuilder.DropForeignKey(
                name: "FK_Builds_Utilizadores_UtilizadorID",
                table: "Builds");

            migrationBuilder.DropIndex(
                name: "IX_Builds_CharacterId",
                table: "Builds");

            migrationBuilder.DropIndex(
                name: "IX_Builds_UtilizadorID",
                table: "Builds");

            migrationBuilder.DropColumn(
                name: "StatAfetada",
                table: "Armas");

            migrationBuilder.DropColumn(
                name: "StatBonus",
                table: "Armas");

            migrationBuilder.DropColumn(
                name: "StatAfetada",
                table: "Acessorios");

            migrationBuilder.DropColumn(
                name: "StatBonus",
                table: "Acessorios");

            migrationBuilder.AddColumn<int>(
                name: "CharClassId",
                table: "Builds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Builds",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Builds_CharClassId",
                table: "Builds",
                column: "CharClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Builds_UserId",
                table: "Builds",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Personagens_CharClassId",
                table: "Builds",
                column: "CharClassId",
                principalTable: "Personagens",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Builds_Utilizadores_UserId",
                table: "Builds",
                column: "UserId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
