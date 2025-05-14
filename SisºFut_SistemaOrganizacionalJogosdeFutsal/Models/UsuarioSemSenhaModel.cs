using SisºFut_SistemaOrganizacionalJogosdeFutsal.Enums;
using System.ComponentModel.DataAnnotations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class UsuarioSemSenhaModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Digite o Nome do Time")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Digite o login do Usuário")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Digite o email do usuário")]
        [EmailAddress(ErrorMessage = "O email informado não é valido!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite o perfil do usuário")]
        public PerfilEnum? Perfil { get; set; }

    }
}
