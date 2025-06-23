using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Helper;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    [PaginaParaUsuarioLogado]
    public class ContatoController : Controller
    {
        private readonly IContatoRepositorio _contatoRepositorio;
        private readonly ISessao _sessao;
        public ContatoController(IContatoRepositorio contatoRepositorio,
                                  ISessao sessao)
        {
            _contatoRepositorio = contatoRepositorio;
            _sessao = sessao;
        }

        public IActionResult Index(string filtro, int? page)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                // Redireciona para login se não estiver logado
                return RedirectToAction("Login", "Conta");
            }

            int pageNumber = page ?? 1;
            int pageSize = 10;

            // Busca só os contatos do usuário logado
            var contatos = _contatoRepositorio.BuscarTodos()
                .Where(c => c.UsuarioId == usuarioLogado.Id)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
            {
                contatos = contatos
                    .Where(c => c.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase));
            }

            var contatosPaginados = contatos
                .OrderBy(c => c.Nome)
                .ToPagedList(pageNumber, pageSize);

            return View(contatosPaginados);
        }



        public IActionResult Filtrar(string filtro, int? page)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
            if (usuarioLogado == null)
            {
                // Pode redirecionar para login ou retornar erro
                return RedirectToAction("Login", "Conta");
            }

            int pageNumber = page ?? 1;
            int pageSize = 10;

            var contatos = _contatoRepositorio.BuscarTodos()
                .Where(c => c.UsuarioId == usuarioLogado.Id &&
                            (string.IsNullOrEmpty(filtro) || c.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(c => c.Nome)
                .ToPagedList(pageNumber, pageSize);

            return PartialView("_TabelaContatos", contatos);
        }

        public IActionResult Criar()
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }
            return View();
        }

        public IActionResult Editar(int id)
        {
            var usuarioLogado = _sessao.BuscarSessaoDoUsuario();

            if (usuarioLogado == null)
            {
                return RedirectToAction("Login", "Conta"); // Redireciona para login se não estiver logado
            }
            ContatoModel contato = _contatoRepositorio.BuscarPorId(id);
            return View(contato);
        }

        public IActionResult ApagarConfirmacao(int id)
        {
            ContatoModel contato = _contatoRepositorio.BuscarPorId(id);
            return View(contato);
        }

        //public IActionResult Apagar(int id)
        //{
        //    try
        //    {
        //        bool apagado = _contatoRepositorio.Apagar(id);

        //        if (apagado)
        //        {
        //            TempData["MensagemSucesso"] = "Contato Apagado com sucesso!";
        //        }
        //        else
        //        {
        //            TempData["MensagemErro"] = "Erro ao Apagar seu contato, tente novamente. detalhe do erro: ";
        //        }
        //        return RedirectToAction("Index");
        //    }
        //    catch (System.Exception erro)
        //    {
        //        TempData["MensagemErro"] = $"Não conseguimos apagar seu contato, tente novamente. detalhe do erro: {erro.Message}";
        //        return RedirectToAction("Index");

        //    }
        //}

        public IActionResult Apagar(int id)
        {
            try
            {
                // Excluindo o contato
                bool apagado = _contatoRepositorio.Apagar(id);

                if (apagado)
                {
                    // Resetando o AUTO_INCREMENT após a exclusão
                    //_contatoRepositorio.ResetarAutoIncrement();

                    TempData["MensagemSucesso"] = "Contato Apagado com sucesso!";
                }
                else
                {
                    TempData["MensagemErro"] = "Erro ao Apagar seu contato, tente novamente.";
                }

                return RedirectToAction("Index");
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Não conseguimos apagar seu contato, tente novamente. Detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }
        }

        //[HttpPost]
        //public IActionResult Criar(ContatoModel contato)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _contatoRepositorio.Adicionar(contato);
        //            TempData["MensagemSucesso"] = "Contato Cadastrado com sucesso";

        //            return RedirectToAction("Index");
        //        }

        //        return View(contato);

        //    }
        //    catch (System.Exception erro)
        //    {
        //        TempData["MensagemErro"] = $"Erro ao cadastrar seu contato, tente novamente. detalhe do erro: {erro.Message}";
        //        throw;
        //    }

        //}

        [HttpPost]
        public async Task<IActionResult> Criar(ContatoModel contato)
        {
            try
            {
                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null)
                {
                    TempData["MensagemErro"] = "Usuário não está logado.";
                    return RedirectToAction("Login", "Conta");
                }

                if (ModelState.IsValid)
                {
                    contato.UsuarioId = usuarioLogado.Id;

                    // Verifica domínio do e-mail
                    var emailHelper = new EmailHelper();
                    bool dominioValido = await emailHelper.VerificarDominioEmailAsync(contato.Email);
                    if (!dominioValido)
                    {
                        ModelState.AddModelError("Email", "O domínio do e-mail não é válido ou não existe.");
                    }

                    // Verifica digitação do e-mail (sugestão)
                    var sugestao = EmailHelper.SugerirDominioCorreto(contato.Email);
                    if (sugestao != null)
                    {
                        ModelState.AddModelError("Email", $"Domínio inválido. Você quis dizer: {sugestao}?");
                    }

                    var celularExistente = _contatoRepositorio.BuscarPorCelularEUsuario(contato.Celular, usuarioLogado.Id);
                    if (celularExistente != null)
                    {
                        ModelState.AddModelError("Celular", "Você já cadastrou esse número de celular.");
                    }

                    var emailExistente = _contatoRepositorio.BuscarPorEmailEUsuario(contato.Email, usuarioLogado.Id);
                    if (emailExistente != null)
                    {
                        ModelState.AddModelError("Email", "Você já cadastrou esse e-mail.");
                    }

                    if (!ModelState.IsValid)
                    {
                        return View(contato);
                    }

                    _contatoRepositorio.Adicionar(contato);

                    TempData["MensagemSucesso"] = "Contato cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }

                return View(contato);
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar o contato. Detalhes: {erro.Message}";
                throw;
            }
        }



        [HttpPost]
        public IActionResult Alterar(ContatoModel contato)
        {
            try
            {
                var usuarioLogado = _sessao.BuscarSessaoDoUsuario();
                if (usuarioLogado == null)
                {
                    TempData["MensagemErro"] = "Usuário não está logado.";
                    return RedirectToAction("Login", "Conta");
                }

                var contatoNoBanco = _contatoRepositorio.BuscarPorId(contato.Id);

                if (contatoNoBanco == null)
                {
                    TempData["MensagemErro"] = "Contato não encontrado.";
                    return RedirectToAction("Index");
                }

                // Verifica se o celular já está em uso por outro contato (excluindo o contato atual)
                var celularExistente = _contatoRepositorio.BuscarPorCelularEUsuario(contato.Celular, usuarioLogado.Id);
                if (celularExistente != null && celularExistente.Id != contato.Id)
                {
                    ModelState.AddModelError("Celular", "Você já cadastrou esse número de celular.");
                }

                // Verifica se o e-mail já está em uso por outro contato (excluindo o contato atual)
                var emailExistente = _contatoRepositorio.BuscarPorEmailEUsuario(contato.Email, usuarioLogado.Id);
                if (emailExistente != null && emailExistente.Id != contato.Id)
                {
                    ModelState.AddModelError("Email", "Você já cadastrou esse e-mail.");
                }

                // Se houver erro de validação, retorna à view
                if (!ModelState.IsValid)
                {
                    return View("Editar", contato);
                }

                // Se tudo estiver ok, faz a atualização no banco
                contatoNoBanco.Nome = contato.Nome;
                contatoNoBanco.Email = contato.Email;
                contatoNoBanco.Celular = contato.Celular;
                contatoNoBanco.Categoria = contato.Categoria;  // Atualizando o campo de Categoria

                _contatoRepositorio.Atualizar(contatoNoBanco);

                TempData["MensagemSucesso"] = "Contato alterado com sucesso!";
                return RedirectToAction("Index");
            }
            catch (Exception erro)
            {
                TempData["MensagemErro"] = $"Ops, erro ao atualizar o contato. Detalhe: {erro.Message}";
                return RedirectToAction("Index");
            }
        }







    }
}
