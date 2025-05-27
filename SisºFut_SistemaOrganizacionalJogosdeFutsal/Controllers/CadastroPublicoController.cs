using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;

using System;
using System.IO;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Cadastro(CadastroModel cadastro)
        {
            try
            {
                // Verificações de dados únicos
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

                // Verifica domínio do e-mail
                var emailHelper = new EmailHelper();
                bool dominioValido = await emailHelper.VerificarDominioEmailAsync(cadastro.usuario.Email);
                if (!dominioValido)
                {
                    ModelState.AddModelError("usuario.Email", "O domínio do e-mail não é válido ou não existe.");
                }

                // Verifica digitaoção do e-mail
                var sugestao = EmailHelper.SugerirDominioCorreto(cadastro.usuario.Email);
                if (sugestao != null)
                {
                    ModelState.AddModelError("usuario.Email", $"Domínio inválido. Você quis dizer: {sugestao}?");
                }

                // Verifica se login e senha foram preenchidos
                if (string.IsNullOrEmpty(cadastro.usuario.Login) || string.IsNullOrEmpty(cadastro.usuario.Senha))
                {
                    TempData["MensagemErro"] = "Login e senha são obrigatórios.";
                    return View(cadastro);
                }

                // Valida antes de prosseguir
                if (!ModelState.IsValid)
                {
                    return View(cadastro);
                }

                // Conversão da imagem e cadastro
                if (cadastro.usuario.FotoArquivo != null && cadastro.usuario.FotoArquivo.Length > 0)
                {
                    cadastro.usuario.Foto = ConverterParaBase64(cadastro.usuario.FotoArquivo);
                }

                cadastro.usuario.Perfil = Enums.PerfilEnum.Padrao;
                var usuarioCriado = _usuarioRepositorio.Adicionar(cadastro.usuario);

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
