using System.ComponentModel.DataAnnotations;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class ContatoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Digite o nome do contato")]
        

        public string Nome { get; set; }
        [Required(ErrorMessage = "Digite o email do contato")]
        [EmailAddress(ErrorMessage = "O email informado Não é valido")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Digite o celular do contato")]
        [Phone(ErrorMessage = "O Celular informado não é valido")]
        public string Celular { get; set; }


    }
}
