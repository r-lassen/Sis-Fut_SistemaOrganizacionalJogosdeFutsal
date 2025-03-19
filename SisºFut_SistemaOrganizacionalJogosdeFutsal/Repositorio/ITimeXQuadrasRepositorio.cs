using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface ITimeXQuadrasRepositorio
    {

            List<TimeXQuadrasModel> BuscarTodos();
            TimeXQuadrasModel BuscarPorId(int id);
            TimeXQuadrasModel BuscarPorTime(int id);
            TimeXQuadrasModel Adicionar(TimeXQuadrasModel agendamentos);
            TimeXQuadrasModel Atualizar(TimeXQuadrasModel agendamentos);
            bool Apagar(int id);
        
    }
}
