using System.ComponentModel.DataAnnotations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{

    public class DefinirNovaSenhaModel
    {
        [Required]
        public string Token { get; set; }

        [Required(ErrorMessage = "Digite a nova senha")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres")]
        public string NovaSenha { get; set; }
    }
}
