using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class AlterarSenhaController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        public AlterarSenhaController(IUsuarioRepositorio usuarioRepositorio,
                                      ISessao sessao)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
        }

        public IActionResult Index()
        {

            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Index", "Home"); // Redireciona para login se não estiver logado
            }
            return View();
        }

        [HttpPost]
        public IActionResult Alterar(AlterarSenhaModel alterarSenhaModel)
        {
            try
            {


                UsuarioModel usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                alterarSenhaModel.Id = usuarioLogado.Id;

                if (ModelState.IsValid)
                {
                    // Buscar o usuário completo no banco para verificar a senha
                    UsuarioModel usuarioBanco = _usuarioRepositorio.BuscarPorId(usuarioLogado.Id);

                    // Verifica se a senha atual está correta
                    if (!usuarioBanco.SenhaValida(alterarSenhaModel.SenhaAtual))
                    {
                        TempData["MensagemErro"] = "Senha atual incorreta.";
                        return View("Index", alterarSenhaModel);
                    }

                    // Verifica se a nova senha é igual à atual
                    if (usuarioBanco.SenhaValida(alterarSenhaModel.NovaSenha))
                    {
                        TempData["MensagemErro"] = "A nova senha não pode ser igual à senha atual.";
                        return View("Index", alterarSenhaModel);
                    }

                    // Troca a senha
                    _usuarioRepositorio.AlterarSenha(alterarSenhaModel);
                    TempData["MensagemSucesso"] = "Senha alterada com sucesso!";
                    return RedirectToAction("Index", "MeuPerfil");
                }

                return View("Index", alterarSenhaModel);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos alterar sua senha. Tente novamente. Detalhe do erro: {erro.Message}";
                return View("Index", alterarSenhaModel);
            }
        }





    }
}
