using Microsoft.EntityFrameworkCore.Migrations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Migrations
{
    public partial class AddFotoToUsuario : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Foto",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Quadra",
                table: "Agendamentos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Agendamentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioSelecionado",
                table: "Agendamentos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Agendamentos_UsuarioId",
                table: "Agendamentos",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Agendamentos_Usuarios_UsuarioId",
                table: "Agendamentos",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Agendamentos_Usuarios_UsuarioId",
                table: "Agendamentos");

            migrationBuilder.DropIndex(
                name: "IX_Agendamentos_UsuarioId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "Foto",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Quadra",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "UsuarioSelecionado",
                table: "Agendamentos");
        }
    }
}
