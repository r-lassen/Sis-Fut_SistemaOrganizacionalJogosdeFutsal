using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using X.PagedList;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Controllers
{
    [PaginaParaUsuarioLogado]
    public class ContatoController : Controller
    {
        private readonly IContatoRepositorio _contatoRepositorio;
        public ContatoController(IContatoRepositorio contatoRepositorio)
        {
            _contatoRepositorio = contatoRepositorio;
        }

        public IActionResult Index(string filtro, int? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var contatos = _contatoRepositorio.BuscarTodos().AsQueryable();

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
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var contatos = _contatoRepositorio.BuscarTodos()
                                             .Where(c => string.IsNullOrEmpty(filtro) || c.Nome.Contains(filtro, StringComparison.OrdinalIgnoreCase))
                                             .OrderBy(c => c.Nome)
                                             .ToPagedList(pageNumber, pageSize);

            return PartialView("_TabelaContatos", contatos);
        }

        public IActionResult Criar()
        {
            return View();
        }

        public IActionResult Editar(int id)
        {
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
        public IActionResult Criar(ContatoModel contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //// Remove máscara do celular, deixando só números
                    //if (!string.IsNullOrEmpty(contato.Celular))
                    //{
                    //    contato.Celular = new string(contato.Celular.Where(char.IsDigit).ToArray());
                    //}

                    // Verifica se o número de celular já está cadastrado
                    var celularExistente = _contatoRepositorio.BuscarPorCelular(contato.Celular);
                    if (celularExistente != null)
                    {
                        ModelState.AddModelError("Celular", "Este número de celular já está cadastrado.");
                    }

                    // Verifica se o e-mail já está cadastrado
                    var emailExistente = _contatoRepositorio.BuscarPorEmail(contato.Email);
                    if (emailExistente != null)
                    {
                        ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                    }

                    if (!ModelState.IsValid)
                    {
                        return View(contato);
                    }

                    // Adiciona o contato
                    _contatoRepositorio.Adicionar(contato);
                    TempData["MensagemSucesso"] = "Contato cadastrado com sucesso!";
                    return RedirectToAction("Index");
                }

                return View(contato);
            }
            catch (System.Exception erro)
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


                var contatoNoBanco = _contatoRepositorio.BuscarPorId(contato.Id);

                if (contatoNoBanco == null)
                {
                    TempData["MensagemErro"] = "Contato não encontrado.";
                    return RedirectToAction("Index");
                }

                // Verifica se o celular já está em uso por outro contato
                var celularExistente = _contatoRepositorio.BuscarPorCelular(contato.Celular);
                if (celularExistente != null && celularExistente.Id != contato.Id)
                {
                    ModelState.AddModelError("Celular", "Este número de celular já está cadastrado.");
                }

                // Verifica se o e-mail já está em uso por outro contato
                var emailExistente = _contatoRepositorio.BuscarPorEmail(contato.Email);
                if (emailExistente != null && emailExistente.Id != contato.Id)
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
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
