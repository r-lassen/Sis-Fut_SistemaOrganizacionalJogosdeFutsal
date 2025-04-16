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

        List<AgendamentosModel> ListarAbertos();

        List<AgendamentosModel> ListarMarcados();
        bool Apagar(int id);
    }
}
