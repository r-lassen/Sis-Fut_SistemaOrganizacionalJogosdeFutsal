using System.ComponentModel.DataAnnotations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Digite o login")]
        public string Login { get; set; }
        [Required(ErrorMessage = "Digite a senha")]
        public string Password { get; set; }    
    }
}
