using Microsoft.AspNetCore.Http;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Enums;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Models
{
    public class UsuarioModel
    {


        public int Id { get; set; }
        //--------------------------------------------------------------------
        [Required(ErrorMessage = "Digite o nome do usuário")]
        public string Name { get; set; }
        //--------------------------------------------------------------------
        [Required(ErrorMessage = "Digite o login do usuário")]
        public string Login { get; set; }
        //--------------------------------------------------------------------
        [Required(ErrorMessage = "Digite o email do usuário")]
        [EmailAddress(ErrorMessage = "O email informado não é valido!")]
        public string Email { get; set; }
        //--------------------------------------------------------------------
        [Required(ErrorMessage = "Digite o perfil do usuário")]
        public PerfilEnum? Perfil { get; set; }
        //--------------------------------------------------------------------
        [Required(ErrorMessage = "Digite a senha do usuário")]
        public string Senha { get; set; }
        //--------------------------------------------------------------------
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAtualização { get; set; }




        // CAMPOS PARA REDEFINIÇÃO DE SENHA
        public string? TokenRedefinicaoSenha { get; set; }

        public DateTime? TokenExpiracao { get; set; }

        public string Foto { get; set; }

        [NotMapped]
        public IFormFile FotoArquivo { get; set; }



        public bool SenhaValida(string senha)
        {
            return Senha == senha.GerarHash();
        }

        public void SetSenhaHash()
        {
            Senha = Senha.GerarHash();
        }

        public void SetNovaSenha(string novaSenha)
        {
            Senha = novaSenha.GerarHash();
        }

        public string GerarNovaSenha()
        {
            string novaSenha = Guid.NewGuid().ToString().Substring(0, 8);
            Senha = novaSenha.GerarHash();
            return novaSenha;
        }

    }
}
