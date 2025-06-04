using Microsoft.EntityFrameworkCore;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;


namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        public DbSet<ContatoModel> Contatos { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }

        public DbSet<QuadrasModel> Quadras { get; set; }

        public DbSet<AgendamentosModel> Agendamentos { get; set; }

        public DbSet<JogosEncerradosModel> JogosEncerrados { get; set; }
    }

    public class Contatos
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Celular { get; set; }
    }
}

