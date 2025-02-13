using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper
{
    public interface ISessao
    {
        void CriarSessaoDoUsuario(UsuarioModel usuario);
        void RemoverSessaoUsuario();
        UsuarioModel BuscarSessaoDoUsuario();
    }
}
