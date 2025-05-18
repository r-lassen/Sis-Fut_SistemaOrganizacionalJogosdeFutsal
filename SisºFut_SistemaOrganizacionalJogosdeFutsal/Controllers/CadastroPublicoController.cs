using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;

using System;
using System.IO;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class CadastroPublicoController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ITimeXQuadrasRepositorio _timeXquadrasRepositorio;
        private readonly IQuadrasRepositorio _quadrasRepositorio;

        public CadastroPublicoController(IUsuarioRepositorio usuarioRepositorio,

            ITimeXQuadrasRepositorio timeXquadras,
            IQuadrasRepositorio quadras)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _timeXquadrasRepositorio = timeXquadras;
            _quadrasRepositorio = quadras; 
        }

        [HttpGet]
        [AllowAnonymous] // Permite acesso sem login
        public IActionResult Cadastro()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Cadastro(CadastroModel cadastro)
        {
            try
            {
                // 🔍 Verificações personalizadas de dados repetidos
                if (_usuarioRepositorio.BuscarPorEmail(cadastro.usuario.Email) != null)
                {
                    ModelState.AddModelError("usuario.Email", "Este e-mail já está cadastrado.");
                }

                if (_usuarioRepositorio.BuscarPorLogin(cadastro.usuario.Login) != null)
                {
                    ModelState.AddModelError("usuario.Login", "Este login já está em uso.");
                }

                if (_usuarioRepositorio.BuscarPorNomeTime(cadastro.usuario.Name) != null)
                {
                    ModelState.AddModelError("usuario.Name", "Este nome de time já está em uso.");
                }

                // Verifica se login e senha foram preenchidos
                if (string.IsNullOrEmpty(cadastro.usuario.Login) || string.IsNullOrEmpty(cadastro.usuario.Senha))
                {
                    TempData["MensagemErro"] = "Login e senha são obrigatórios.";
                    return View(cadastro);
                }

                // ✅ Só valida depois de adicionar todos os possíveis erros
                if (!ModelState.IsValid)
                {
                    return View(cadastro);
                }

                // Se imagem enviada, converte para Base64
                if (cadastro.usuario.FotoArquivo != null && cadastro.usuario.FotoArquivo.Length > 0)
                {
                    cadastro.usuario.Foto = ConverterParaBase64(cadastro.usuario.FotoArquivo);
                }

                // Define perfil padrão
                cadastro.usuario.Perfil = Enums.PerfilEnum.Padrao;

                // Adiciona usuário ao banco
                var usuarioCriado = _usuarioRepositorio.Adicionar(cadastro.usuario);

                // Cria quadra associada ao time (usuário)
                var quadra = new QuadrasModel
                {
                    DS_Endereco = cadastro.quadras?.DS_Endereco ?? string.Empty,
                    NM_Quadra = cadastro.quadras?.NM_Quadra ?? string.Empty,
                    id_Time = usuarioCriado.Id
                };

                _quadrasRepositorio.Adicionar(quadra);

                TempData["MensagemSucesso"] = "Cadastro realizado com sucesso!";
                return RedirectToAction("Index", "Login");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar usuário. Detalhes: {erro.Message}";
                return View(cadastro);
            }
        }









        public string ConverterParaBase64(IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return null;

            using (var memoryStream = new MemoryStream())
            {
                arquivo.CopyTo(memoryStream); // versão síncrona
                var bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }




    }
}
