using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class HomeModel
    {
        public string Nome { get; set; }

        public string Email { get; set; }

        public List<DadosAgendamentos> Abertos { get; set; }

        public List<DadosAgendamentos> Marcados { get; set; }
    }
}
