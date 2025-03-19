using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface IQuadrasRepositorio
    {
        List<QuadrasModel> BuscarTodos();
        QuadrasModel BuscarPorId(int id);
        QuadrasModel Adicionar(QuadrasModel Quadras);
        QuadrasModel Atualizar(QuadrasModel Quadras);
        bool Apagar(int id);
    }
}
