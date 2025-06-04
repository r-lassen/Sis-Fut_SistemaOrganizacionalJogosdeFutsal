using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Migrations
{
    public partial class TabalaJogosEncerrados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JogosEncerrados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IdTime1 = table.Column<int>(type: "int", nullable: false),
                    IdTime2 = table.Column<int>(type: "int", nullable: false),
                    GolsTime1 = table.Column<int>(type: "int", nullable: false),
                    GolsTime2 = table.Column<int>(type: "int", nullable: false),
                    IdQuadra = table.Column<int>(type: "int", nullable: false),
                    Descricao = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DataEncerramento = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JogosEncerrados", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JogosEncerrados");
        }
    }
}
