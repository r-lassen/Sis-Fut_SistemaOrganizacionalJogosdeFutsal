using Microsoft.EntityFrameworkCore.Migrations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Migrations
{
    public partial class AddCamposTabelas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "st_ativo",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "st_ativo",
                table: "Quadras",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Contatos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "st_ativo",
                table: "Agendamentos",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "st_ativo",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "st_ativo",
                table: "Quadras");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Contatos");

            migrationBuilder.DropColumn(
                name: "st_ativo",
                table: "Agendamentos");
        }
    }
}
