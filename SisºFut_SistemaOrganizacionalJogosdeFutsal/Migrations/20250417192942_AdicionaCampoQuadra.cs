using Microsoft.EntityFrameworkCore.Migrations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Migrations
{
    public partial class AdicionaCampoQuadra : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Id_Time",
                table: "Quadras",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Id_Time",
                table: "Quadras");
        }
    }
}