using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;


namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface IContatoRepositorio
    {

        List<ContatoModel> BuscarTodos();
        ContatoModel BuscarPorId(int id);
        ContatoModel Adicionar(ContatoModel contato);
        ContatoModel Atualizar(ContatoModel contato);

        ContatoModel BuscarPorCelularEUsuario(string celular, int usuarioId);
        ContatoModel BuscarPorEmailEUsuario(string email, int usuarioId);



        bool Apagar(int id);
        //void ResetarAutoIncrement();  // Adicionando a assinatura do método
    }
}
