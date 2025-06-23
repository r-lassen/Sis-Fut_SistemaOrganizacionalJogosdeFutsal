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
        private readonly IQuadrasRepositorio _quadrasRepositorio;

        public CadastroPublicoController(IUsuarioRepositorio usuarioRepositorio,

            IQuadrasRepositorio quadras)
        {
            _usuarioRepositorio = usuarioRepositorio;
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
                // Busca usuário pelo login (ativo ou não)
                var usuarioLoginExistente = _usuarioRepositorio.BuscarPorLogin(cadastro.usuario.Login);

                // Busca usuário pelo e-mail (ativo ou não)
                var usuarioEmailExistente = _usuarioRepositorio.BuscarPorEmail(cadastro.usuario.Email);

                // Busca usuário pelo nome do time (ativo ou não)
                var usuarioNomeTimeExistente = _usuarioRepositorio.BuscarPorNomeTime(cadastro.usuario.Name);

                // Se já existir um usuário ativo com o login, e-mail ou nome do time, erro
                if ((usuarioLoginExistente != null && usuarioLoginExistente.st_ativo) ||
                    (usuarioEmailExistente != null && usuarioEmailExistente.st_ativo) ||
                    (usuarioNomeTimeExistente != null && usuarioNomeTimeExistente.st_ativo))
                {
                    if (usuarioLoginExistente != null && usuarioLoginExistente.st_ativo)
                        ModelState.AddModelError("usuario.Login", "Este login já está em uso.");

                    if (usuarioEmailExistente != null && usuarioEmailExistente.st_ativo)
                        ModelState.AddModelError("usuario.Email", "Este e-mail já está cadastrado.");

                    if (usuarioNomeTimeExistente != null && usuarioNomeTimeExistente.st_ativo)
                        ModelState.AddModelError("usuario.Name", "Este nome de time já está em uso.");

                    return View(cadastro);
                }

                // Se existir usuário desativado com o mesmo login, e o e-mail e nome do time não conflitam, redireciona para reativação
                if (usuarioLoginExistente != null && !usuarioLoginExistente.st_ativo)
                {
                    // Verifica se e-mail e nome do time NÃO conflitam com usuários ativos diferentes
                    bool emailConflita = usuarioEmailExistente != null && usuarioEmailExistente.st_ativo && usuarioEmailExistente.Id != usuarioLoginExistente.Id;
                    bool nomeTimeConflita = usuarioNomeTimeExistente != null && usuarioNomeTimeExistente.st_ativo && usuarioNomeTimeExistente.Id != usuarioLoginExistente.Id;

                    if (!emailConflita && !nomeTimeConflita)
                    {
                        TempData["MensagemErro"] = "Este usuário está desativado. Você pode reativá-lo abaixo.";
                        return View("~/Views/Login/ReativarUsuario.cshtml");
                    }
                    else
                    {
                        if (emailConflita)
                            ModelState.AddModelError("usuario.Email", "Este e-mail já está cadastrado por outro usuário ativo.");

                        if (nomeTimeConflita)
                            ModelState.AddModelError("usuario.Name", "Este nome de time já está em uso por outro usuário ativo.");

                        return View(cadastro);
                    }
                }

                // Aqui pode adicionar a verificação da quadra associada, se quiser — só um exemplo:
                var quadraExistente = _quadrasRepositorio.BuscarPorId(usuarioLoginExistente?.Id ?? 0);
                if (quadraExistente != null && !quadraExistente.st_ativo)
                {
                    ModelState.AddModelError("", "A quadra associada a este usuário está desativada.");
                    return View(cadastro);
                }

                // Verifica domínio do e-mail
                var emailHelper = new EmailHelper();
                bool dominioValido = await emailHelper.VerificarDominioEmailAsync(cadastro.usuario.Email);
                if (!dominioValido)
                {
                    ModelState.AddModelError("usuario.Email", "O domínio do e-mail não é válido ou não existe.");
                }

                // Verifica digitação do e-mail
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

                // Setar ativo
                cadastro.usuario.st_ativo = true;

                var usuarioCriado = _usuarioRepositorio.Adicionar(cadastro.usuario);

                var quadra = new QuadrasModel
                {
                    DS_Endereco = cadastro.quadras?.DS_Endereco ?? string.Empty,
                    NM_Quadra = cadastro.quadras?.NM_Quadra ?? string.Empty,
                    id_Time = usuarioCriado.Id,
                    st_ativo = true // aqui seta ativo para quadra
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
