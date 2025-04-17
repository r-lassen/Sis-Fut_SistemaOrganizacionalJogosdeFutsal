using Microsoft.VisualBasic;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class AgendamentosModel
    {

        [NotMapped]
        public int UsuarioId { get; set; }
        [NotMapped]
        public DadosAgendamentos Agendar { get; set; }
        public string Quadra { get; set; }

        //public int UsuarioSelecionado { get; set; }
        public int Id { get; set; }

        public int id_Time1 { get; set; }

        public int? id_Time2{ get; set; }

        public DateTime DT_Agendamento { get; set; }

        public DateTime? DT_Atualizacao { get; set; }

        public string HR_Agendamento { get; set; }

        public int id_Quadra { get; set; }

        public string DS_Descricao { get; set; }

        [NotMapped]
        public UsuarioModel Usuario { get; set; }
    }
}
