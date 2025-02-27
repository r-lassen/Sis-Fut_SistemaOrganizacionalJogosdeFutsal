using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using System.Collections.Generic;


namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio
{
    public interface IUsuarioRepositorio
    {

        UsuarioModel BuscarPorLogin(string login);
        UsuarioModel BuscarPorEmailELogin(string email, string login);
        List<UsuarioModel> BuscarTodos();
        UsuarioModel BuscarPorId(int id);
        UsuarioModel Adicionar(UsuarioModel usuario);
        UsuarioModel Atualizar(UsuarioModel usuario);
        UsuarioModel AlterarSenha(AlterarSenhaModel alterarSenhaModel);
        bool Apagar(int id);

    }
}
