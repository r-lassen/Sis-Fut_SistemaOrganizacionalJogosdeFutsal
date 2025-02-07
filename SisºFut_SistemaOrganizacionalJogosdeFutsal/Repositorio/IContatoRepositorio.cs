using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;


namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface IContatoRepositorio
    {
        ContatoModel ListarPorId(int id);
        List<ContatoModel> BuscarTodos();

        ContatoModel Adicionar(ContatoModel contato);
        ContatoModel Atualizar(ContatoModel contato);
        bool Apagar(int id);

    }
}
