using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface IJogosEncerradosRepositorio
    {
        JogosEncerradosModel BuscarPorId(int id);
        List<JogosEncerradosModel> BuscarTodos();
        JogosEncerradosModel Adicionar(JogosEncerradosModel JogosEncerrados);

        List<JogosEncerradosModel> BuscarJogosAbertosPorIdTime1(int id);
        List<JogosEncerradosModel> BuscarJogosAbertosPorIdTime2(int id);


    }
}
