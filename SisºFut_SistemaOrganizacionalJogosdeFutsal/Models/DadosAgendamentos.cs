using System;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class DadosAgendamentos
    {

        public string Time1 { get; set; }

        public string Time2 { get; set; }


        public int idTime1 { get; set; }

        public int idTime2 { get; set; }

        public int id { get; set; }

        public DateTime Data { get; set; }

        public string Hora { get; set; }
        public string Local { get; set; }

        public string DS_Descrição { get; set; }

        public string FotoTime1 { get; set; }
        public string FotoTime2 { get; set; }
    }
}
