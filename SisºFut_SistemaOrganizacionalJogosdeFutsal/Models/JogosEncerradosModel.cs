using System;
using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class JogosEncerradosModel
    {

        public List<DadosJogosEncerrados> JogosEncerrados { get; set; }
        public int Id { get; set; }
        public int IdTime1 { get; set; }
        public int IdTime2 { get; set; }
        public int GolsTime1 { get; set; }
        public int GolsTime2 { get; set; }
        public int IdQuadra { get; set; }
        public string Descricao { get; set; }
        public DateTime DataEncerramento { get; set; } = DateTime.Now;
    }
}
