using Microsoft.AspNetCore.Mvc;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Filters;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Models;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Repositorio;
using System.Collections.Generic;

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

        public IActionResult Index()
        {
            List<ContatoModel> contatos = _contatoRepositorio.BuscarTodos();

            return View(contatos);
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
                    // Adicionando o novo contato
                    _contatoRepositorio.Adicionar(contato);
                    TempData["MensagemSucesso"] = "Contato Cadastrado com sucesso!";

                    return RedirectToAction("Index");
                }

                return View(contato);
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao cadastrar seu contato, tente novamente. Detalhe do erro: {erro.Message}";
                throw;
            }
        }

        [HttpPost]
        public IActionResult Alterar(ContatoModel contato)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _contatoRepositorio.Atualizar(contato);
                    TempData["MensagemSucesso"] = "Contato Alterado com sucesso";
                    return RedirectToAction("Index");
                }
                return View("Editar", contato);
            }
            catch (System.Exception erro)
            {
                TempData["MensagemErro"] = $"Erro ao alterar o contato, tente novamente. detalhe do erro: {erro.Message}";
                return RedirectToAction("Index");
            }

        }
    }
}
