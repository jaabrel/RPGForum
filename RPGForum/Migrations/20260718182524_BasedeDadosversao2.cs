using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RPGForum.Migrations
{
    /// <inheritdoc />
    public partial class BasedeDadosversao2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_AspNetUsers_UserId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Builds_BuildId",
                table: "Comentario");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentario_Comentario_ParentId",
                table: "Comentario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comentario",
                table: "Comentario");

            migrationBuilder.RenameTable(
                name: "Comentario",
                newName: "Comentarios");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_UserId",
                table: "Comentarios",
                newName: "IX_Comentarios_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_ParentId",
                table: "Comentarios",
                newName: "IX_Comentarios_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentario_BuildId",
                table: "Comentarios",
                newName: "IX_Comentarios_BuildId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_AspNetUsers_UserId",
                table: "Comentarios",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Builds_BuildId",
                table: "Comentarios",
                column: "BuildId",
                principalTable: "Builds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentarios_Comentarios_ParentId",
                table: "Comentarios",
                column: "ParentId",
                principalTable: "Comentarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_AspNetUsers_UserId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Builds_BuildId",
                table: "Comentarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Comentarios_Comentarios_ParentId",
                table: "Comentarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comentarios",
                table: "Comentarios");

            migrationBuilder.RenameTable(
                name: "Comentarios",
                newName: "Comentario");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_UserId",
                table: "Comentario",
                newName: "IX_Comentario_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_ParentId",
                table: "Comentario",
                newName: "IX_Comentario_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Comentarios_BuildId",
                table: "Comentario",
                newName: "IX_Comentario_BuildId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comentario",
                table: "Comentario",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_AspNetUsers_UserId",
                table: "Comentario",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Builds_BuildId",
                table: "Comentario",
                column: "BuildId",
                principalTable: "Builds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comentario_Comentario_ParentId",
                table: "Comentario",
                column: "ParentId",
                principalTable: "Comentario",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
