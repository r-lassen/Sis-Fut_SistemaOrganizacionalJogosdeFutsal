using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;


namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Data
{
    public class BancoContext : DbContext
    {
        public BancoContext(DbContextOptions<BancoContext> options) : base(options)
        {
        }

        public DbSet<ContatoModel> Contatos { get; set; }
        public DbSet<UsuarioModel> Usuarios { get; set; }


    }

    public class Contatos
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

