using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    public class LoginController : Controller
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly ISessao _sessao;
        public LoginController(IUsuarioRepositorio usuarioRepositorio,
                               ISessao sessao)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
        }
        public IActionResult Index()
        {
            //se o usuario estiver logado, redirecionar para a tela home

            if (_sessao.BuscarSessaoDoUsuario() != null) return RedirectToAction("Index", "Home");

            return View();
        }

        public IActionResult RedefinirSenha()
        {
            return View();
        }

        public IActionResult DefinirNovaSenha()
        {
            return View();
        }

        public IActionResult Sair()
        {
            _sessao.RemoverSessaoUsuario();
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public IActionResult Entrar(LoginModel loginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    UsuarioModel usuario = _usuarioRepositorio.BuscarPorLogin(loginModel.Login);

                    if (usuario == null)
                    {
                        ModelState.AddModelError("Login", "Usuário não encontrado.");
                        return View("Index");
                    }

                    if (!usuario.SenhaValida(loginModel.Senha))
                    {
                        ModelState.AddModelError("Senha", "Senha inválida.");
                        return View("Index");
                    }

                    _sessao.CriarSessaoDoUsuario(usuario);
                    return RedirectToAction("Index", "Home");
                }

                return View("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, não conseguimos realizar seu login. Tente novamente. Detalhes: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        //[HttpPost]
        //public IActionResult EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            UsuarioModel usuario = _usuarioRepositorio.BuscarPorEmailELogin(redefinirSenhaModel.Email, redefinirSenhaModel.Login);

        //            if (usuario != null)
        //            {
        //                // Gerar token único e expiração de 1 hora
        //                string token = Guid.NewGuid().ToString();
        //                usuario.TokenRedefinicaoSenha = token;
        //                usuario.TokenExpiracao = DateTime.Now.AddHours(1);

        //                _usuarioRepositorio.Atualizar(usuario);

        //                // Criar link com token
        //                string link = Url.Action("RedefinirSenha", "Login", new { token = token }, Request.Scheme);

        //                // Aqui você pode implementar o envio por e-mail
        //                // Por enquanto, podemos apenas mostrar no TempData (para testes)
        //                TempData["MensagemSucesso"] = $"Link para redefinir senha: <a href='{link}' target='_blank'>Clique aqui</a>";

        //                return RedirectToAction("Index", "Login");
        //            }

        //            TempData["MensagemErro"] = "Não encontramos um usuário com os dados informados.";
        //        }

        //        return View("Index");
        //    }
        //    catch (Exception erro)
        //    {
        //        TempData["MensagemErro"] = $"Erro ao processar a solicitação: {erro.Message}";
        //        return RedirectToAction("Index");
        //    }
        //}


        //[HttpGet]
        //public IActionResult RedefinirSenha(string token)
        //{
        //    var usuario = _usuarioRepositorio.BuscarPorToken(token);

        //    if (usuario == null || usuario.TokenExpiracao < DateTime.Now)
        //    {
        //        TempData["MensagemErro"] = "Token inválido ou expirado.";
        //        return RedirectToAction("Index");
        //    }

        //    var model = new DefinirNovaSenhaModel { Token = token };
        //    return View("DefinirNovaSenha", model);
        //}




        [HttpPost]
        public IActionResult SalvarNovaSenha(DefinirNovaSenhaModel model)
        {
            if (!ModelState.IsValid) return View("DefinirNovaSenha", model);

            var usuario = _usuarioRepositorio.BuscarPorToken(model.Token);

            if (usuario == null || usuario.TokenExpiracao < DateTime.Now)
            {
                TempData["MensagemErro"] = "Token inválido ou expirado.";
                return RedirectToAction("Index");
            }

            usuario.SetNovaSenha(model.NovaSenha);
            usuario.TokenRedefinicaoSenha = null;
            usuario.TokenExpiracao = null;

            _usuarioRepositorio.Atualizar(usuario);

            TempData["MensagemSucesso"] = "Senha redefinida com sucesso. Faça login com a nova senha!";
            return RedirectToAction("Index");
        }



        [HttpPost]
        public IActionResult EnviarLinkParaRedefinirSenha(RedefinirSenhaModel redefinirSenhaModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Primeiro, procura pelo login
                    UsuarioModel usuario = _usuarioRepositorio.BuscarPorLogin(redefinirSenhaModel.Login);

                    if (usuario == null)
                    {
                        ModelState.AddModelError("Login", "Login não encontrado.");
                        return View("RedefinirSenha");
                    }

                    // Em seguida, valida se o e-mail bate com o do usuário
                    if (!usuario.Email.Equals(redefinirSenhaModel.Email, StringComparison.OrdinalIgnoreCase))
                    {
                        ModelState.AddModelError("Email", "O e-mail informado não corresponde ao login.");
                        return View("RedefinirSenha");
                    }

                    // Login e email válidos — gerar token
                    string token = Guid.NewGuid().ToString();
                    usuario.TokenRedefinicaoSenha = token;
                    usuario.TokenExpiracao = DateTime.Now.AddHours(1);
                    _usuarioRepositorio.Atualizar(usuario);

                    string link = Url.Action("RedefinirSenha", "Login", new { token = token }, Request.Scheme);

                    string assunto = "Redefinição de senha - SisºFut";
                    string mensagem = $"Olá, {usuario.Name}!<br><br>" +
                                      $"Clique no link abaixo para redefinir sua senha:<br>" +
                                      $"<a href='{link}'>Redefinir Senha</a><br><br>" +
                                      $"Esse link expirará em 1 hora.";

                    bool emailEnviado = EmailHelper.Enviar(usuario.Email, assunto, mensagem);

                    if (emailEnviado)
                    {
                        TempData["MensagemSucesso"] = "Enviamos um link para seu e-mail. Verifique sua caixa de entrada!";
                    }
                    else
                    {
                        TempData["MensagemErro"] = "Não conseguimos enviar o e-mail. Tente novamente mais tarde.";
                    }

                    return RedirectToAction("Index", "Login");
                }

                return View("RedefinirSenha");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao processar a solicitação: {erro.Message}";
                return RedirectToAction("Index");
            }
        }


        //public IActionResult Criar()
        //{
        //    return View();
        //}


        //[HttpPost]
        //public IActionResult Criar(UsuarioModel usuario)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            usuario = _usuarioRepositorio.Adicionar(usuario);
        //            TempData["MensagemSucesso"] = "Usuário cadastrado com sucesso";

        //            return RedirectToAction("Index", "Login");
        //        }

        //        return View(usuario);

        //    }
        //    catch (System.Exception erro)
        //    {
        //        TempData["MensagemErro"] = $"Erro ao cadastrar seu usuário, tente novamente. Detalhe do erro: {erro.Message}";
        //        return RedirectToAction("Index");
        //    }

        //}





    }
}