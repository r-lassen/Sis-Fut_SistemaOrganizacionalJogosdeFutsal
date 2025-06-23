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
        private readonly IQuadrasRepositorio _quadrasRepositorio;
        public LoginController(IUsuarioRepositorio usuarioRepositorio,
                               ISessao sessao,
                                 IQuadrasRepositorio quadras)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _sessao = sessao;
            _quadrasRepositorio = quadras;
        }
        public IActionResult Index()
        {
            //se o usuario estiver logado, redirecionar para a tela home

            if (_sessao.BuscarSessaoDoUsuario() != null) return RedirectToAction("Index", "Login");

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
                    var usuario = _usuarioRepositorio.BuscarPorLogin(loginModel.Login);

                    if (usuario == null)
                    {
                        ModelState.AddModelError("Login", "Usuário não encontrado.");
                        return View("Index");
                    }

                    // Verifica se o usuário está desativado (false ou null)
                    if (!usuario.st_ativo || usuario.st_ativo == false)
                    {
                        TempData["MensagemErro"] = "Este usuário está desativado. Você pode reativá-lo abaixo.";

                        // Redireciona para a tela de reativação, preenchendo dados conhecidos
                        return RedirectToAction("ReativarUsuario", "Login", new { login = usuario.Login, email = usuario.Email });

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


        [HttpGet]
        public IActionResult ReativarUsuario(string login, string email)
        {
            var model = new ReativarUsuarioModel
            {
                Login = login,
                Email = email
            };

            return View("~/Views/Login/ReativarUsuario.cshtml", model);
        }

        [HttpPost]
        public IActionResult ReativarUsuario(ReativarUsuarioModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Login/ReativarUsuario.cshtml", model);
            }

            // Busca usuário pelo login
            var usuario = _usuarioRepositorio.BuscarPorLogin(model.Login);

            if (usuario == null || usuario.Email.ToLower() != model.Email.ToLower())
            {
                ModelState.AddModelError("", "Login e e-mail não conferem.");
                return View("~/Views/Login/ReativarUsuario.cshtml", model);
            }

            // Verifica se já está ativo
            if (usuario.st_ativo)
            {
                ModelState.AddModelError("", "Este usuário já está ativo.");
                return View("~/Views/Login/ReativarUsuario.cshtml", model);
            }

            // Reativa o usuário
            usuario.st_ativo = true;
            _usuarioRepositorio.Atualizar(usuario);

            // Verifica se a quadra associada a esse usuário está desativada
            var quadra = _quadrasRepositorio.BuscarPorId(usuario.Id); // Usando Id como id_time
            if (quadra != null && !quadra.st_ativo)
            {
                quadra.st_ativo = true;
                _quadrasRepositorio.Atualizar(quadra);
            }

            TempData["MensagemSucesso"] = "Usuário reativado com sucesso! Faça login para continuar.";
            return RedirectToAction("Index", "Login");
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




        // GET: exibe a tela com o token vindo pela query string
        [HttpGet]
        public IActionResult DefinirNovaSenha(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                TempData["MensagemErro"] = "Token inválido.";
                return RedirectToAction("Index");
            }

            // Cria a model com o token preenchido
            var model = new DefinirNovaSenhaModel
            {
                Token = token
            };

            return View(model); // Passa a model para a view
        }

        // POST: processa o envio da nova senha
        [HttpPost]
        public IActionResult SalvarNovaSenha(DefinirNovaSenhaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("DefinirNovaSenha", model);

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
            catch (Exception ex)
            {
                TempData["MensagemErro"] = $"Ocorreu um erro ao redefinir a senha: {ex.Message}";
                return View("DefinirNovaSenha", model);
            }
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

                    string link = Url.Action("DefinirNovaSenha", "Login", new { token = token }, Request.Scheme);

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