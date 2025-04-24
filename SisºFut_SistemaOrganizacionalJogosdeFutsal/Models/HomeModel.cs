using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class HomeModel
    {
        public string Nome { get; set; }

        public string Email { get; set; }

        public int Id { get; set; }

        public List<DadosAgendamentos> Abertos { get; set; }

        public List<DadosAgendamentos> Marcados { get; set; }

        [NotMapped]
        public UsuarioModel Usuario { get; set; }

        [NotMapped]
        public QuadrasModel Quadra { get; set; }
    }
}
