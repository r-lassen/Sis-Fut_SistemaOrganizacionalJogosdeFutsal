using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface IAgendamentoRepositorio
    {
        List<AgendamentosModel> BuscarTodos();
        AgendamentosModel BuscarPorId(int id);
        AgendamentosModel Adicionar(AgendamentosModel agendamentos);
        AgendamentosModel Atualizar(AgendamentosModel agendamentos);

        List<AgendamentosModel> BuscarJogosAbertosPorIdTime1(int id);
        List<AgendamentosModel> BuscarJogosAbertosPorIdTime2(int id);

        List<AgendamentosModel> ListarAbertos();

        List<AgendamentosModel> ListarMarcados();
        bool Apagar(int id);
    }
}
